using System;
using Interfaces;
using UnityEngine;
using UnityEngine.PlayerLoop;
using IPoolable = Interfaces.IPoolable;

namespace Enemies.Boss
{
    public class Flame: MonoBehaviour, IWeapon, IPoolable
    {
        [SerializeField] private float weaponDistanceX;
        [SerializeField] private float weaponDistanceY;
        [SerializeField] private  Vector3 originalScale;
        [SerializeField] private float speed;
        [SerializeField] private Rigidbody2D rb;
        public Boss boss;
        [SerializeField] private float slope;

        public bool Attack(Vector3 position, int direction)
        {
            Debug.Log($"LocalScale: {transform.localScale}");
            transform.position = position + new Vector3(weaponDistanceX * direction, weaponDistanceY , 0);
            originalScale = transform.localScale;
            transform.localScale = new Vector3(direction * originalScale.x,
                originalScale.y, originalScale.z);
            Vector2 velocity = new Vector2(speed * direction, -speed * slope); // Diagonal down movement
            rb.linearVelocity = velocity;
            return true;
        }

        void OnTriggerEnter2D(Collider2D collision)
        {
            Debug.Log($"OnTriggerEnter2D: {collision.gameObject.name}");
            FlamePool.Instance.Return(this);
        }
        public void Reset()
        {
            rb.linearVelocity = Vector2.zero;
            transform.localScale = new Vector3(originalScale.x,
                originalScale.y, originalScale.z);
        }

        public void Update()
        {
            if (boss.IsDead)
            {
                FlamePool.Instance.Return(this);
            }
        }
    }
}