using System;
using System.Collections;
using System.Collections.Generic;
using Audio;
using Character;
using DG.Tweening;
using Interfaces;
using UnityEngine;
using Random = UnityEngine.Random;


namespace Enemies.Boss
{
    public class Boss: MonoBehaviour, IEnemyEntity
    {
        [SerializeField] private Animator animatorBoss;
        [SerializeField] private CharacterContext arthur;
        [SerializeField] private Transform shootingPosition1;
        [SerializeField] private Transform shootingPosition2;
        [SerializeField] private Transform hitSpawn;
        [SerializeField] private Transform leftBorder;
        [SerializeField] private Transform rightBorder;
        [SerializeField] private Rigidbody2D bossRigidbody;
        [SerializeField] private int health;
        [SerializeField] private GameObject hitEffect;
        [SerializeField] private float delayTimeHitDisappear;
        [SerializeField] private float delayTimeDeathDisappear;
        [SerializeField] private float moveSpeed;
        [SerializeField] private float normalShootInterval;
        [SerializeField] private float rageShootInterval;
        [SerializeField] private float idleInterval;
        [SerializeField] private int maxShots;
        private int _currentHealth;
        private bool _isRaging;
        private bool _isMoving;
        private Tween _delayedCall;
        private List<Flame> activeFlames;
        public bool IsActive { get; private set; }
        public bool IsDead { get; private set; }
        
        public void Activate()
        {
            AdjustScaleToPlayer();
            IsDead = false;
            _delayedCall = DOTween.Sequence();
            IsActive = true;
            _currentHealth = health;
            StartCoroutine(BossBehavior());
        }

        public void Deactivate()
        {
            transform.localScale = Vector3.one;
            IsActive = false;
            IsDead = false;
            animatorBoss.ResetTrigger("Death");
            animatorBoss.SetBool("RageMode", false);
            StopAllCoroutines();
            _delayedCall.Kill();
            DOTween.Kill(this); 
            _isRaging = false;
            _isMoving = false;
        }
        
        public void Update(){
            if(!IsActive) return;
            CheckForHealth();
            AdjustScaleToPlayer();
        }

        

        private void CheckForHealth()
        {
            if (_currentHealth <= 0)
            {
                IsDead = true;
                IsActive = false;
                animatorBoss.SetBool("RageMode", false);
                animatorBoss.SetTrigger("Death");
                _delayedCall.Kill();
                DOVirtual.DelayedCall(delayTimeDeathDisappear, () => { gameObject.SetActive(false); });
            }
        }
        public void HitByWeapon()
        {
            if (arthur.WorldDataController.IsDead) return;
            AudioManager.Instance.Play(AudioName.EnemyHit, transform.position);
            _currentHealth--;
            var hit = Instantiate(hitEffect, hitSpawn.position, Quaternion.identity);
            DOVirtual.DelayedCall(delayTimeHitDisappear, () => { Destroy(hit); });
        }
        private IEnumerator BossBehavior()
        {
            while (IsActive)
            {
                while (!_isRaging)
                {
                    MoveRandomly();
                    yield return new WaitForSeconds(idleInterval);
                    
                    maxShots = 3;
                    StartCoroutine(ShootAtPlayer(normalShootInterval)); 
                    yield return new WaitForSeconds(normalShootInterval * maxShots);
                    float value = Random.Range(0f, 1f);
                    if (value <= 0.3f)
                    {
                        _isRaging = true;
                    }
                }
                
                EnterRageMode();
                yield return new WaitForSeconds(maxShots*rageShootInterval);
                ExitRageMode();
            }
        }

        private void MoveRandomly()
        {
            if (_isMoving) return;

            _isMoving = true;
            float randomDirection;
            if (arthur.transform.position.x <= transform.position.x)
            {
                 randomDirection = Random.value < 0.6f ? -1f : 1f;
            }
            else
            {
                randomDirection = Random.value < 0.6f ? 1f : -1f;
            }
            if (Mathf.Approximately(transform.position.x, leftBorder.position.x))
            {
               randomDirection = 1f; 
            }
            else if (Mathf.Approximately(transform.position.x, rightBorder.position.x))
            {
                randomDirection = -1f; 
            }
            float movementDistance = Random.Range(0.2f, 0.5f);
            Vector3 targetPosition = transform.position + new Vector3(randomDirection * movementDistance, 0, 0);
            targetPosition.x = Mathf.Clamp(targetPosition.x, leftBorder.position.x, rightBorder.position.x);
            float travelTime = movementDistance / moveSpeed;
            _delayedCall.Kill();
            _delayedCall = transform.DOMove(targetPosition, travelTime).SetEase(Ease.Linear).OnComplete(() => _isMoving = false);
        }

        private IEnumerator ShootAtPlayer(float interval)
        {
            int shotCount = 0;
            while (shotCount < maxShots)
            {
                float value = Random.Range(0f, 1f);
                if (value >= 0.7f)
                {
                    ShootFlame(shootingPosition1);
                }
                else if (value <= 0.3f)
                {
                    ShootFlame(shootingPosition2);
                }
                shotCount++;
                yield return new WaitForSeconds(interval);
            }
        }

        private void ShootFlame(Transform shootingPosition)
        {
            Vector2 direction = (arthur.transform.position - shootingPosition.position).normalized;
            Flame flame = FlamePool.Instance.Get();
            flame.boss = this;
            flame.Attack(shootingPosition.position, (int)Mathf.Sign(direction.x));
        }

        private void EnterRageMode()
        {
            animatorBoss.SetBool("RageMode",true);
            _delayedCall.Kill(); 
            _isMoving = false;
            StartCoroutine(RageShooting());
        }

        private IEnumerator RageShooting()
        {
            int maxShotsRage = Random.Range(3, 5);
            for (int i = 0; i < maxShotsRage; i++)
            {
                float value = Random.Range(0f, 1f);
                if (value >= 0.5f)
                {
                    ShootFlame(shootingPosition1);
                }
                else if (value <= 0.5f)
                {
                    ShootFlame(shootingPosition2);
                }
                yield return new WaitForSeconds(rageShootInterval);
            }
        }

        private void ExitRageMode()
        {
            _isRaging = false;
            animatorBoss.SetBool("RageMode",false);
        }

        private void AdjustScaleToPlayer()
        {
            // Flip scale based on player position
            Vector3 localScale = transform.localScale;
            localScale.x = arthur.transform.position.x > transform.position.x ? -Mathf.Abs(localScale.x) : Mathf.Abs(localScale.x);
            transform.localScale = localScale;
        }
    }
}