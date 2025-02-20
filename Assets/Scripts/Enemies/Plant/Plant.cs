using System.Collections;
using DG.Tweening;
using Interfaces;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Enemies.Plant
{
    public class Plant: MonoBehaviour, IEnemyEntity
    {
        [SerializeField] private UnityEngine.Camera mainCamera;
        [SerializeField] private Animator plantAnimator;
        [SerializeField] private Transform shootingPosition;
        [SerializeField] private float deathTimeAnimation;
        [SerializeField] private float attackWaveDelay;
        [SerializeField] private float attackDelay;
        [SerializeField] private float delayForAnimationAttack;
        [SerializeField] private BoxCollider2D plantCollider;
        [SerializeField] private Transform plantTransform;
        public bool IsActive { get; private set; }
        public CapsuleCollider2D playerCollider;
        public Transform playerTransform;
        private bool _isDead;

        public void Activate()
        {
            plantTransform.localScale = new Vector3(-1, 1, 1);
            plantAnimator.ResetTrigger("Dead");
            plantAnimator.SetBool("Attack", false);
            GameConfig.Instance.ControlObjectsCollision(playerCollider,plantCollider,true);
            gameObject.SetActive(true);
            IsActive = false;
            _isDead = false;
        }

        public void Deactivate()
        {
            StopAllCoroutines();
            plantAnimator.ResetTrigger("Dead");
            plantAnimator.SetBool("Attack", false);
            GameConfig.Instance.ControlObjectsCollision(playerCollider,plantCollider,false);
            IsActive = false;
            _isDead = false;
        }
        
        private void OnBecameVisible()
        {
            IsActive = true;
            StartCoroutine(AttackMode());
        }

        private void OnBecameInvisible()
        {
            IsActive = false;
            StopAllCoroutines();
        }

        private IEnumerator AttackMode()
        {
            int numberOfAttacks = 1;
            while (IsActive)
            {
                for (int i = 0; i < numberOfAttacks; i++)
                {
                    Attack();
                    yield return new WaitForSeconds(attackDelay);
                }

                numberOfAttacks = numberOfAttacks == 1 ? 2 : 1;
                yield return new WaitForSeconds(attackWaveDelay);
            }
        }

        private void AdjustToPlayerPosition()
        {
            Vector3 localScale = transform.localScale;
            localScale.x = playerTransform.position.x > transform.position.x ? -Mathf.Abs(localScale.x) : Mathf.Abs(localScale.x);
            transform.localScale = localScale;
        }

        private void Attack()
        {
            AdjustToPlayerPosition();
            plantAnimator.SetBool("Attack", true);
            DOVirtual.DelayedCall(delayForAnimationAttack,
                () =>
                {
                    if (IsActive)
                    {
                        Vector2 direction = (playerTransform.position - shootingPosition.position).normalized;
                        PlantWeapon weapon = PlantWeaponPool.Instance.Get();
                        weapon.PlayerPosition = playerTransform.position;
                        weapon.Attack(shootingPosition.position, (int)Mathf.Sign(direction.x));
                        plantAnimator.SetBool("Attack",false);
                    }
                });
            
        }


        public void HitByWeapon()
        { 
           IsActive = false;
           _isDead = true;
            StopAllCoroutines();
            GameConfig.Instance.ControlObjectsCollision(playerCollider,plantCollider,false);
           plantAnimator.SetTrigger("Dead");
           DOVirtual.DelayedCall(deathTimeAnimation, () => { gameObject.SetActive(false); });
        }
        
        
        private void Update()
        {
            if (!mainCamera) return;

            Vector3 viewPos = mainCamera.WorldToViewportPoint(transform.position);
            
            bool isVisible = (viewPos.z > 0f)
                             && (viewPos.x >= 0f && viewPos.x <= 1f)
                             && (viewPos.y >= 0f && viewPos.y <= 1f);
            
            if (isVisible && !IsActive && !_isDead)
            {
                IsActive = true;
                StartCoroutine(AttackMode());
            }
            else if (!isVisible && IsActive && !_isDead)
            {
                IsActive = false;
                StopAllCoroutines();
            }
        }

    }
}