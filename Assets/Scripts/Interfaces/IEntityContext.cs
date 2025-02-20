using System;
using Character;
using Interfaces;
using UnityEngine;

namespace DefaultNamespace
{
    public interface IEntityContext<T>  where T : struct, Enum
    {
        public IAnimationController<T> AnimationController { get; }
        public IColliderController<T> ColliderController { get; }

        public IInputUpdater InputUpdater { get; }
        public IWorldData<T> WorldDataController { get; }
        
        public Rigidbody2D Rigidbody2D { get; }
        
        public CharacterConfig CharacterConfig { get; }
        
        public void SetHorizontalMovement(float horizontalMovement);
        public void StopHorizontalMovement();
        public void SetVerticalMovement(float verticalMovement);
        public void StopVerticalMovement();
        

        public void Jump(float jumpForce);
        public void ChangePosition(Vector3 position);
        public void ChangeScale(float horizontalInput);

        public bool Attack();

        public void StopGravity();

        public void GravityBack();
        


    }
}