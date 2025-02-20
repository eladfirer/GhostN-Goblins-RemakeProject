using Audio;
using DefaultNamespace;
using DefaultNamespace.Enemies;
using Interfaces;
using UnityEngine;

namespace Enemies.Zombie
{
    public class Zombie: MonoBehaviour, IPoolable, IEnemyEntity
    {
        [SerializeField] private Rigidbody2D rigidbody;
        [SerializeField] private Animator animator;
        [SerializeField] private float speed;
        [SerializeField] private CapsuleCollider2D zombieCollider;
        [SerializeField] private BoxCollider2D headZombieCollider;
        [SerializeField] private float zombieDelaySpawn;
        [SerializeField] private float zombieDelayBackToGroundLimit;
        [SerializeField] private float zombieDelayBackToGroundMax;
        [SerializeField] private float zombieDelayDeathAnimation;
        [SerializeField] private Transform zombieTransform;
        private Collider2D _playerCollider;
        public bool IsActive { get; private set; }
        private int _direction;
        private bool _isGrounded;
        [SerializeField] private float rayLength;

        public void ActivateZombie(int direction, Collider2D playerCollider2D)
        {
            animator.SetBool("Spawn", true);
            CancelInvoke(nameof(Activate));
            CancelInvoke(nameof(ReturnToPool));
            CancelInvoke(nameof(ReturnToGround));
            _playerCollider = playerCollider2D;
            GameConfig.Instance.ControlObjectsCollision(_playerCollider,headZombieCollider,false);
            GameConfig.Instance.ControlObjectsCollision(_playerCollider,zombieCollider,false);
            _direction = direction;
            Invoke(nameof(Activate),zombieDelaySpawn);
            AudioManager.Instance.Play(AudioName.ZombieSpawn, transform.position);
            float randomDelay = UnityEngine.Random.Range(zombieDelayBackToGroundLimit, zombieDelayBackToGroundMax);
            Invoke(nameof(ReturnToGround),randomDelay);
        }

        public void ActivateZombieWithoutSpawn(int direction, Collider2D playerCollider2D)
        {
            _playerCollider = playerCollider2D;
            _direction = direction;
            Activate();
            AudioManager.Instance.Play(AudioName.ZombieSpawn, transform.position);
            float randomDelay = UnityEngine.Random.Range(zombieDelayBackToGroundLimit, zombieDelayBackToGroundMax);
            Invoke(nameof(ReturnToGround),randomDelay);
        }
        private void ReturnToGround()
        {
            rigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
            rigidbody.linearVelocity = Vector2.zero;
            animator.SetBool("Move", false);
            GameConfig.Instance.ControlObjectsCollision(_playerCollider,headZombieCollider,false);
            GameConfig.Instance.ControlObjectsCollision(_playerCollider,zombieCollider,false);
            IsActive = false;
            animator.SetBool("Despawn", true);
            Invoke(nameof(ReturnToPool),zombieDelaySpawn);
        }

        private void ReturnToPool()
        {
            ZombieSpawner.ActivatedZombies.Remove(this);
            ZombiePool.Instance.Return(this);
        }

        public void Update()
        {
            if (IsActive)
            {
                CheckForGround();
                if (!_isGrounded)
                {
                    ReturnToGround();
                }
                rigidbody.linearVelocity = new Vector2(_direction * speed, rigidbody.linearVelocity.y);
            }
        }


        private void Activate()
        {
            animator.SetBool("Spawn", false);
            rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
            rigidbody.linearVelocity = Vector2.zero;
            animator.SetBool("Move", true);
            GameConfig.Instance.ControlObjectsCollision(_playerCollider,zombieCollider,true);
            GameConfig.Instance.ControlObjectsCollision(_playerCollider,headZombieCollider,true);
            IsActive = true;
        }
        
        public void DeActivate()
        {
            CancelInvoke(nameof(Activate));
            CancelInvoke(nameof(ReturnToPool));
            CancelInvoke(nameof(ReturnToGround));
            rigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
            rigidbody.linearVelocity = Vector2.zero;
            animator.SetBool("Move", false);
            GameConfig.Instance.ControlObjectsCollision(_playerCollider,headZombieCollider,false);
            GameConfig.Instance.ControlObjectsCollision(_playerCollider,zombieCollider,false);
            IsActive = false;
            animator.enabled = false;
        }
        

        public void HitByWeapon()
        {
            AudioManager.Instance.Play(AudioName.EnemyDeath, transform.position);
            CancelInvoke(nameof(ReturnToGround));
            rigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
            rigidbody.linearVelocity = Vector2.zero;
            animator.SetBool("Move", false);
            GameConfig.Instance.ControlObjectsCollision(_playerCollider,headZombieCollider,false);
            GameConfig.Instance.ControlObjectsCollision(_playerCollider,zombieCollider,false);
            IsActive = false;
            animator.SetTrigger("Die");
            Invoke(nameof(ReturnToPool),zombieDelayDeathAnimation);
        }


        public void Reset()
        {
            rigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
            rigidbody.linearVelocity = Vector2.zero;
            zombieTransform.localPosition = Vector3.zero;
            animator.enabled = true;
            animator.ResetTrigger("Die");
            animator.SetBool("Despawn", false);
            animator.SetBool("Move", false);
            animator.SetBool("Spawn", false);
            IsActive = false;
            rigidbody.linearVelocity = Vector2.zero;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if ((1 << collision.gameObject.layer & GameConfig.Instance.borderLayer) != 0)
            {
                CancelInvoke(nameof(ReturnToGround));
                ReturnToGround();
            }
        }
        
        private void CheckForGround()
        {
            Vector2 origin = rigidbody.position; 
            Vector2 rayCastDirection = Vector2.down; 
            
            RaycastHit2D hit = Physics2D.Raycast(origin, rayCastDirection, rayLength, GameConfig.Instance.groundLayer);
            
            _isGrounded = hit.collider != null;
            
            Debug.DrawRay(origin, rayCastDirection * rayLength, _isGrounded ? Color.green : Color.red);
        }
        
    }
    
}