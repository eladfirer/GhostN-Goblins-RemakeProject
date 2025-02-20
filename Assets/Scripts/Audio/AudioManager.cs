using System;
using DefaultNamespace;
using UnityEngine;

namespace Audio
{
    public class AudioManager : MonoSingleton<AudioManager>
    {
        public Sound[] sounds;
    
        public SoundObject Play(AudioName name, Vector3 pos)
        {
            Sound s = Array.Find(sounds, sound => sound.name == name);
            if (s == null)
            {
                Debug.LogWarning("Sound: " + name + " not found!");
                return null;
            }

       

            SoundObject soundObject = SoundPool.Instance.Get();
        
            soundObject.Play(s,pos);
            return soundObject;
        }
    
    }

    public enum AudioName
    {
        ArthurDeath = 0,
        ArthurHit = 1,
        ArthurJump = 2,
        ArthurLand = 3,
        BossFirstDeath = 4,
        BossSecondDeath = 5,
        BossWalk = 6,
        EnemyHit = 7,
        EnemyDeath = 8,
        GameOver = 9,
        GameStart = 10,
        Timer = 11,
        ZombieSpawn = 12,
        BossStage = 13,
        Stage1 = 14,
        CutScene = 15,
        IllusionScene = 16,
        WinScene = 17,
        Key = 18,
        StageBossStart = 19,
        ArthurThrow = 20,
        FrogMove = 21
        
        
    }
}