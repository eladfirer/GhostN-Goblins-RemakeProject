using DG.Tweening;
using UnityEngine;

namespace WorldObjects
{
    public class DamageArmor: MonoBehaviour
    {
        [SerializeField] private GameObject[] spritesForArmor; // Assign your sprite prefabs here
        [SerializeField] private float diagonalDistance = 0.5f; // Distance to move in diagonal
        [SerializeField] private float moveDuration = 1f;
        [SerializeField] private float objectTime = 1f;
        
        public void Activate()
        {
            gameObject.SetActive(true);
            Vector3 currentPosition = transform.position;
            
            Vector3[] spawnPositions =
            {
                currentPosition + new Vector3(diagonalDistance, diagonalDistance, 0),  // Top-right
                currentPosition + new Vector3(diagonalDistance, -diagonalDistance, 0), // Bottom-right
                currentPosition + new Vector3(-diagonalDistance, -diagonalDistance, 0), // Bottom-left
                currentPosition + new Vector3(-diagonalDistance, diagonalDistance, 0)  // Top-left
            };
            
            for (int i = 0; i < spritesForArmor.Length; i++)
            {
                var sprite = Instantiate(spritesForArmor[i], currentPosition, Quaternion.identity);

                Vector3 targetPosition = spawnPositions[i] + (spawnPositions[i] - currentPosition).normalized * diagonalDistance;
                
                sprite.transform.DOMove(targetPosition, moveDuration).OnComplete(() =>
                {
                    Destroy(sprite);
                });
            }
            DOVirtual.DelayedCall(objectTime, () => { gameObject.SetActive(false); });
        }
    }
}