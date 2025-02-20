using System;
using UnityEngine;
using WorldObjects;

namespace DefaultNamespace
{
    public interface IWorldData<T> where T : struct, Enum
    {
        public T Form { get; set; }
        public bool IsGrounded { get; }
        public bool GotHit { get; set;}
        public int Health { get; }
        public float Velocity { get;  }
        public int HorizontalDirection { get; }
        
        public Vector3 Position { get; }
        
        
        public Ladder LadderOverlap { get; }
        public bool GraveOverlap{ get; }

        public int DamageBounceDirection { get; }

        public bool Key { get; }
        bool IsDead { get; set; }
        
        public bool TimeFinished { get; set; }
        
        public void SetForm(int CurrentRound);

        public bool GraveIncoming { get; }

        public void ResetRound();
    }
}