using System.Collections.Generic;
using System.Linq;
using DefaultNamespace.Enemies;
using DG.Tweening;
using UnityEngine;

namespace Enemies.Zombie
{
    public class ZombieSpawner: MonoBehaviour
    {
        [SerializeField] private Transform playerPosition;
        [SerializeField] private float delayTimeSpawn;
        [SerializeField] private float spawnInterval;
        [SerializeField] private int numberOfZombies;
        [SerializeField] private Collider2D playerCollider2D;
        [SerializeField] private float spawnZombieDelayTimeMin;
        [SerializeField] private float spawnZombieDelayTimeMax;
        [SerializeField] private Transform upFloorLeftPlatfrom;
        [SerializeField] private Transform upFloorRightPlatfrom;
        [SerializeField] private Transform downFloorPlatfrom;
        [SerializeField] private Transform noZombieSpawnBorder;
        [SerializeField] private float spawnRadius;
        [SerializeField] private float upperYPosition;
        [SerializeField] private float lowerYPosition;
        
        [SerializeField] private Transform zombieWalking1SpawnPoint;
        [SerializeField] private Transform zombieWalking2SpawnPoint;
        [SerializeField] private Transform zombieSpawningSpawnPoint;
        [SerializeField] private float walkingZombiesDelayTimeSpawn;
        [SerializeField] private float spawnZombieDelayTimeSpawn;
        private bool _isActive;
        public static readonly List<Zombie> ActivatedZombies = new List<Zombie>();

        public void ActivateSpawnZombies()
        {
            _isActive = true;
            void ZombieWalikingCallback()
            {
                var zombieWalking1 = ZombiePool.Instance.Get();
                var zombieWalking2 = ZombiePool.Instance.Get();
                ActivatedZombies.Add(zombieWalking1);
                ActivatedZombies.Add(zombieWalking2);
                int direction1 = AdjustPositionToPlayer(zombieWalking1,zombieWalking1SpawnPoint.position);
                int direction2 = AdjustPositionToPlayer(zombieWalking2,zombieWalking2SpawnPoint.position);
                zombieWalking1.ActivateZombieWithoutSpawn(direction1, playerCollider2D);
                zombieWalking2.ActivateZombieWithoutSpawn(direction2, playerCollider2D);
            }

            void ZombieSpawnCallback()
            {
                var zombieSpawning = ZombiePool.Instance.Get();
                ActivatedZombies.Add(zombieSpawning);
                int direction3 = AdjustPositionToPlayer(zombieSpawning,zombieSpawningSpawnPoint.position);
                zombieSpawning.ActivateZombie(direction3, playerCollider2D);
            }
            DOVirtual.DelayedCall(walkingZombiesDelayTimeSpawn,ZombieWalikingCallback);
            DOVirtual.DelayedCall(spawnZombieDelayTimeSpawn,ZombieSpawnCallback);
            InvokeRepeating(nameof(SpawnZombie), delayTimeSpawn, spawnInterval);
        }

        public void DeactivateSpawnZombies()
        {
            _isActive = false;
            CancelInvoke(nameof(SpawnZombie));
        }

        public void DestroyAllZombies()
        {
            foreach (Zombie zombie in ActivatedZombies.ToList())
            { 
              ZombiePool.Instance.Return(zombie);  
              ActivatedZombies.Remove(zombie);
            }
        }
        private void SpawnZombie()
        {

            for (int i = 0; i < numberOfZombies; i++)
            {
                float randomDelay = UnityEngine.Random.Range(spawnZombieDelayTimeMin, spawnZombieDelayTimeMax);
                Invoke(nameof(GenerateZombie),randomDelay);
            }
            
        }

        private void GenerateZombie()
        {
            if (!_isActive) return;
            Zombie zombie = ZombiePool.Instance.Get();
            ActivatedZombies.Add(zombie);
            int direction = AdjustPositionToPlayer(zombie,GetPosition());
            zombie.ActivateZombie(direction,playerCollider2D);
        }

        private Vector3 GetPosition()
        {
            bool isAboveUpperFloor = playerPosition.position.y > upFloorRightPlatfrom.position.y;
            
           
            float leftBound, rightBound, yPosition;

            if (isAboveUpperFloor)
            {
                leftBound = upFloorLeftPlatfrom.position.x - upFloorLeftPlatfrom.localScale.x / 2 + 0.05f;
                rightBound = upFloorRightPlatfrom.position.x + upFloorRightPlatfrom.localScale.x / 2 - 0.05f;
                yPosition = upperYPosition;
            }
            else
            {
                leftBound = downFloorPlatfrom.position.x - downFloorPlatfrom.localScale.x / 2 + 0.05f;
                rightBound = noZombieSpawnBorder.position.x - 0.05f; 
                yPosition = lowerYPosition;
            }
            
            float clampedLeftBound = Mathf.Max(playerPosition.position.x - spawnRadius, leftBound);
            float clampedRightBound = Mathf.Min(playerPosition.position.x + spawnRadius, rightBound);
            
            float randomXPosition = UnityEngine.Random.Range(clampedLeftBound, clampedRightBound);
            
            return new Vector3(randomXPosition, yPosition, 0f);
        }


        private int AdjustPositionToPlayer(Zombie zombie, Vector3 position)
        {
            zombie.transform.position = position;
            int direction = position.x > playerPosition.position.x ? -1 : 1;

            Vector3 localScale = zombie.transform.localScale;
            zombie.transform.localScale = new Vector3(direction * Mathf.Abs(localScale.x), localScale.y, localScale.z);
            return direction;
        }

        public void StopAllZombiesFromMoving()
        {
            foreach (Zombie zombie in ActivatedZombies)
            {
                zombie.DeActivate();
            }
        }
        
    }
}