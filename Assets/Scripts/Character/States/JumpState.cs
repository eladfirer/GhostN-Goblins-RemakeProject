using Audio;
using DefaultNamespace;
using UnityEngine;

namespace Character.States
{
    public sealed class JumpState: IEntityState<CharacterForm>
    {
        private CharacterForm _currentForm;
        private float _stayJumpedTimer;
        private bool _jumpMovementMode;

        public IEntityState<CharacterForm> OnUpdate(IEntityContext<CharacterForm> context)
        {
            if (context.WorldDataController.TimeFinished)
            {
                return CharacterStateFactory.GetState("Death");
            }
            if (context.WorldDataController.GotHit)
            {
                return CharacterStateFactory.GetState("Damaged");
            }
            if (context.InputUpdater.HorizontalInput != 0)
            {
                context.ChangeScale(context.InputUpdater.HorizontalInput);
            }
            
            if (context.InputUpdater.AttackInput && context.WorldDataController.Form != CharacterForm.Frog)
            {
                AudioManager.Instance.Play(AudioName.ArthurThrow, context.WorldDataController.Position);
                context.Attack();
                context.AnimationController.PlayAnimationBool(context.WorldDataController.Form,"Attack",true);
            }
            
            if (_stayJumpedTimer >= 0)
            {
                if (_jumpMovementMode)
                {
                    context.SetHorizontalMovement(context.WorldDataController.HorizontalDirection*context.CharacterConfig.moveForwardNormalizer);   
                }
                _stayJumpedTimer -= Time.deltaTime;
                return this;
            }
            
            if (context.WorldDataController.IsGrounded)
            {
                AudioManager.Instance.Play(AudioName.ArthurLand, context.WorldDataController.Position);
                return CharacterStateFactory.GetState("Move");
            }
            return this;
        }

        public void EnterState(IEntityContext<CharacterForm> context)
        {
            AudioManager.Instance.Play(AudioName.ArthurJump, context.WorldDataController.Position);
            _jumpMovementMode = false;
            _stayJumpedTimer = context.CharacterConfig.regularJumpedTimer;
            float speed = context.WorldDataController.Velocity; 
            if (speed >= 0.05f || speed <= -0.05f)
            {
                _jumpMovementMode = true;
                if (context.WorldDataController.GraveOverlap)
                {
                    _stayJumpedTimer = context.CharacterConfig.graveCollideJumpedTimer;
                }
            }
            context.AnimationController.PlayAnimationBool(context.WorldDataController.Form,"Jump", true);
            context.Jump(context.CharacterConfig.jumpForce);
            
        }
        

        public void ExitState(IEntityContext<CharacterForm> context)
        {
            context.AnimationController.PlayAnimationBool(context.WorldDataController.Form,"Attack",false);
            context.AnimationController.PlayAnimationBool(context.WorldDataController.Form,"Jump", false);
            context.InputUpdater.ResetInput();
        }
    }
}