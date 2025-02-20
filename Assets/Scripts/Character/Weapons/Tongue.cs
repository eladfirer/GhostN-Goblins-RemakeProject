using System.Collections;
using Interfaces;
using UnityEngine;

namespace Character.Weapons
{
    public sealed class Tongue: MonoBehaviour, IWeapon
    {
        [SerializeField] private float weaponDistanceX;
        [SerializeField] private float weaponDistanceY;
        [SerializeField] private float endSpriteOffsetY;
        [SerializeField] private SpriteRenderer spriteRenderer; 
        [SerializeField] private BoxCollider2D boxCollider; 
        [SerializeField] private Sprite[] tongueSprites; 
        [SerializeField] private Vector2[] colliderSizes; 
        [SerializeField] private Vector2[] colliderOffsets;
        [SerializeField] private float spriteChangeDelay = 0.1f;
        private Coroutine _attackCoroutine;
        private int _direction;
        private Vector3 _originalScale;
        public bool Attack(Vector3 position, int direction)
        {
            if (_attackCoroutine != null)
            {
                return false;
            }
            gameObject.SetActive(true);
            transform.position = position + new Vector3(weaponDistanceX * direction, weaponDistanceY , 0);
            _originalScale = transform.localScale;
            transform.localScale = new Vector3(direction * _originalScale.x,
                _originalScale.y, _originalScale.z);
            _direction = direction;
            _attackCoroutine = StartCoroutine(PerformAttack());
            return true;
        }

        void OnTriggerEnter2D(Collider2D collision)
        {
            if ((1 << collision.gameObject.layer & GameConfig.Instance.enemyLayer) != 0)
            {
                IEnemyEntity enemy = collision.transform.parent?.GetComponent<IEnemyEntity>();
                if (enemy != null)
                {
                    if (!enemy.IsActive)
                    {
                        return;
                    }
                    enemy.HitByWeapon();
                }
            }
        }
        private IEnumerator PerformAttack()
        {
            for (int i = 0; i < tongueSprites.Length; i++)
            {
                Vector3 lastSpriteSize = spriteRenderer.bounds.size;
                spriteRenderer.sprite = tongueSprites[i];
    
               
                boxCollider.size = colliderSizes[i];
                boxCollider.offset = colliderOffsets[i];
                
                Vector3 currentSpriteSize = spriteRenderer.bounds.size;
                // Update the position offset
                Vector3 positionAdjustment = new Vector3(
                    ((currentSpriteSize.x - lastSpriteSize.x) / 2) * _direction,
                    0, 
                    0
                );
                if (i == 2)
                {
                    positionAdjustment += new Vector3(0, endSpriteOffsetY, 0);
                }
                transform.position += positionAdjustment;
                
                yield return new WaitForSeconds(spriteChangeDelay);
            }
            Reset();
        }

        private void Reset()
        {
            gameObject.SetActive(false);
            spriteRenderer.sprite = tongueSprites[0];
            boxCollider.size = colliderSizes[0];
            _attackCoroutine = null;
            _direction = 0;
            transform.localScale = new Vector3(_originalScale.x,
            _originalScale.y, _originalScale.z);
        }
        
    }
}