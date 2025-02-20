using Audio;
using Unity.VisualScripting;
using UnityEngine;

namespace Enemies.Boss
{
    public class SoundWalkingBoss: MonoBehaviour
    {
        [SerializeField] private Boss boss;
        public void ActivateWalkingSound()
        {
            if(boss.IsActive)
            {
                AudioManager.Instance.Play(AudioName.BossWalk, transform.position);
            }
        }
    }
}