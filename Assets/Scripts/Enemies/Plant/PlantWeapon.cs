using Interfaces;
using UnityEngine;
using IPoolable = Interfaces.IPoolable;

namespace Enemies.Plant
{
    public class PlantWeapon: MonoBehaviour, IWeapon, IPoolable
    {
        public Vector3 PlayerPosition { get; set; }
        [SerializeField] private float speed;
        [SerializeField] private Rigidbody2D rb;
        public void Reset()
        {
            
        }

        public bool Attack(Vector3 position, int direction)
        {
            transform.position = position;
            Vector2 toPlayer = PlayerPosition - position;
            float angleDeg = Mathf.Atan2(toPlayer.y, toPlayer.x) * Mathf.Rad2Deg;
            if (angleDeg < 0f) angleDeg += 360f;
            float sector = Mathf.Round(angleDeg / 22.5f);
            float finalAngle = sector * 22.5f;
            
            float angleRad = finalAngle * Mathf.Deg2Rad;
            Vector2 dir = new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
            rb.linearVelocity = dir * speed;
            return true;
        }
        
        void OnTriggerEnter2D(Collider2D collision)
        {
            if ((1 << collision.gameObject.layer & GameConfig.Instance.playerLayer) != 0)
            {
                PlantWeaponPool.Instance.Return(this);
            }
            
        }
    }
}