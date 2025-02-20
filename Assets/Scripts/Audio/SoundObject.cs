using UnityEngine;
using IPoolable = Interfaces.IPoolable;

namespace Audio
{
    public class SoundObject : MonoBehaviour, IPoolable
    {
        [SerializeField] public AudioSource audioSource;

        public void Update()
        {
            if (!audioSource.isPlaying)
            {
                SoundPool.Instance.Return(this);
            }
        }

        public void Play(Sound sound, Vector3 pos)
        {
            gameObject.transform.position = pos;
            audioSource.clip = sound.clip;
            audioSource.volume = sound.volume;
            audioSource.pitch = sound.pitch;
            audioSource.loop = sound.loop;
            audioSource.spatialBlend = sound.spatialBlend;
            audioSource.Play();
        }
        
        public void StopSound()
        {
            if (audioSource.isPlaying) 
            {
                audioSource.Stop();
            }
            
        }

        public void Reset()
        {
        
        }
    }
}
