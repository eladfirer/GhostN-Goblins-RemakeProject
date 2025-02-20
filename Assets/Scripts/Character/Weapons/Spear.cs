
using Interfaces;
using UnityEngine;
using WorldObjects;


namespace Character.Weapons
{
    public sealed class Spear: MonoBehaviour, IWeapon, IPoolable
    {
        private int _direction;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private float speed;
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private float weaponDistanceX;
        [SerializeField] private float weaponDistanceY;
        [SerializeField] private  Vector3 originalScale;
        [SerializeField] private SparkGraveCollision sparkPrefab;
        [SerializeField] private float spawnTimeSpark = 0.3f;
        [SerializeField] private float randomSparkOffSet;
        [SerializeField] private float maxDistanceFromArthur = 2f;
        public Transform ArthurTransform { get; set; }

        private GameObject _collidedObject;
        private bool _firstEnemy;


        public bool Attack(Vector3 position, int direction)
        {
            transform.position = position + new Vector3(weaponDistanceX * direction, weaponDistanceY , 0);
            originalScale = transform.localScale;
            transform.localScale = new Vector3(direction * originalScale.x,
                originalScale.y, originalScale.z);
            _direction = direction;
            rb.linearVelocity = new Vector2(_direction * speed, rb.linearVelocity.y);
            return true;
        }

        
        private void Update()
        {
            
            Vector3 pos = transform.position;
            float distance = Vector3.Distance(pos, ArthurTransform.position);
            if (distance > maxDistanceFromArthur)
            {
                SpearPool.Instance.Return(this);
            }
        }

        void OnTriggerEnter2D(Collider2D collision)
        {
            _collidedObject = collision.gameObject;
            if ((1 << collision.gameObject.layer & GameConfig.Instance.sparkLayer) != 0)
            {
                Bounds bounds = spriteRenderer.bounds;
                Vector3 corner = (_direction == -1) 
                    ? new Vector3(bounds.min.x, bounds.max.y, bounds.center.z) 
                    : new Vector3(bounds.max.x, bounds.max.y, bounds.center.z);

                randomSparkOffSet = 0.05f;
                float randomOffsetX = (_direction == -1)
                    ? Random.Range(0, randomSparkOffSet)  
                    : Random.Range(-randomSparkOffSet, 0);
                float randomOffsetY = Random.Range(-randomSparkOffSet,0);
                Vector3 randomizedPosition = corner + new Vector3(randomOffsetX, randomOffsetY, 0);
                var sparkInstance = Instantiate(sparkPrefab, randomizedPosition, Quaternion.identity);
                Destroy(sparkInstance.gameObject, spawnTimeSpark);
            }

            if ((1 << collision.gameObject.layer & GameConfig.Instance.enemyLayer) != 0
                && _firstEnemy)
            {
                IEnemyEntity enemy = collision.transform.parent?.GetComponent<IEnemyEntity>();
                if (enemy != null)
                {
                    if (!enemy.IsActive)
                    {
                        return;
                    }
                    enemy.HitByWeapon();
                    _firstEnemy = false;
                }
            }
            SpearPool.Instance.Return(this);
        }

        void OnDrawGizmos()
        {
            if (_collidedObject != null)
            {
                Gizmos.color = Color.red; // Set the Gizmos color
                Gizmos.DrawWireSphere(_collidedObject.transform.position, 0.5f); // Draw a sphere around the collided object
            }
        }
        public void Reset()
        {
            _firstEnemy = true;
            transform.localScale = new Vector3(originalScale.x,
                originalScale.y, originalScale.z);
        }
    }
}