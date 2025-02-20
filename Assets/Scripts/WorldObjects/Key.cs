
using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

namespace WorldObjects
{
    public class Key : MonoBehaviour
    {
        [SerializeField] private float delayTime = 1f; 
        [SerializeField] private float fallSpeed = 0.5f; 
        [SerializeField] private float flickerFrequency = 0.1f; 
        [SerializeField] private Rigidbody2D rb;
        

        private Coroutine _flickerCoroutine;

        private void Start()
        {
            StartCoroutine(FallAndFlickerSequence());
        }

        private IEnumerator FallAndFlickerSequence()
        {
            
            yield return new WaitForSeconds(delayTime);
            
            rb.gravityScale = fallSpeed;
            
            _flickerCoroutine = StartCoroutine(Flicker());
        }

        private IEnumerator Flicker()
        {
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

            while (true)
            {
                spriteRenderer.color = new Color(
                    spriteRenderer.color.r,
                    spriteRenderer.color.g,
                    spriteRenderer.color.b,
                    Mathf.Approximately(spriteRenderer.color.a, 1f) ? 0.5f : 1f
                );

                yield return new WaitForSeconds(flickerFrequency);
            }
        }
        

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if ((1 << collision.gameObject.layer & GameConfig.Instance.groundLayer) != 0)
            {
               
                if (_flickerCoroutine != null)
                {
                    StopCoroutine(_flickerCoroutine);
                    _flickerCoroutine = null;
                }
                
                var spriteRenderer = GetComponent<SpriteRenderer>();
                if (spriteRenderer != null)
                {
                    spriteRenderer.color = new Color(
                        spriteRenderer.color.r,
                        spriteRenderer.color.g,
                        spriteRenderer.color.b,
                        1f
                    );
                }
            }

            if ((1 << collision.gameObject.layer & GameConfig.Instance.playerLayer) != 0)
            {
                Destroy(gameObject);
            }
        }
    }
}
