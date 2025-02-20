using System;
using DG.Tweening;
using UnityEngine;

namespace DefaultNamespace
{
    
    
    public interface IColliderController<T>  where T : struct, Enum
    {
        public CapsuleCollider2D CapsuleCollider { get; }
        public void ChangeSprite(T form, string spriteName);
        public TweenCallback Flicker();
    }
    
    
    
}