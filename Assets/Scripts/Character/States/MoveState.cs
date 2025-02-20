using Audio;
using DefaultNamespace;
using UnityEngine;
using WorldObjects;

namespace Character.States
{
    public sealed class MoveState : IEntityState<CharacterForm>
    {
        private float _attackDelay;
        private float _countCheck;
        private bool _attackMode;
        private int _attackCounter;
        private float _currentAttackDelay;

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
            if (_attackMode)
            {
                int horizontalDirection = context.WorldDataController.HorizontalDirection;
                return AttackMode(context, horizontalDirection);
            }

            if (context.WorldDataController.Form != CharacterForm.Frog)
            {
                if (context.WorldDataController.LadderOverlap
                    && context.InputUpdater.VerticalInput > 0
                    && context.WorldDataController.LadderOverlap.GetClosestLadder(
                        context.WorldDataController.Position) == LadderPositions.Lower)
                {
                    context.StopHorizontalMovement();
                    return CharacterStateFactory.GetState("ClimbLadder");
                }

                if (context.WorldDataController.LadderOverlap
                    && context.InputUpdater.VerticalInput < 0
                    && context.WorldDataController.LadderOverlap.GetClosestLadder(
                        context.WorldDataController.Position) == LadderPositions.Upper)
                {
                    context.StopHorizontalMovement();
                    return CharacterStateFactory.GetState("ClimbLadder");
                }

            }
            if (_countCheck >= 0)
            {
                _countCheck -= Time.deltaTime;
                return this;
            }

            if (context.InputUpdater.AttackInput)
            {
                if (context.WorldDataController.Form != CharacterForm.Frog)
                {
                    Attack(context);
                }
                else if (context.WorldDataController.IsGrounded && context.WorldDataController.Velocity < 0.1f)
                {
                    Attack(context);
                }
                return this;
            }
            

            if (context.InputUpdater.JumpInput)
            {
                if (context.WorldDataController.Form == CharacterForm.Frog)
                {
                    return CharacterStateFactory.GetState("Jump");
                }
                if (context.WorldDataController.IsGrounded)
                {
                    return CharacterStateFactory.GetState("Jump");
                }
            }

            if (context.WorldDataController.Form != CharacterForm.Frog && context.InputUpdater.CrouchInput)
            {
                return CharacterStateFactory.GetState("Crouch");
            }

            ControlMovement(context);

            return this;
        }


        private IEntityState<CharacterForm> AttackMode(IEntityContext<CharacterForm> context, float horizontalDirection)
        {
            if (!Mathf.Approximately(context.InputUpdater.HorizontalInput, Mathf.Sign(horizontalDirection))
                && context.InputUpdater.HorizontalInput != 0)
            {
                _attackMode = false;
                context.AnimationController.PlayAnimationBool(context.WorldDataController.Form,"Attack",false);
                return this;
            }
            
            if (_currentAttackDelay >= 0)
            {
                _currentAttackDelay -= Time.deltaTime;
                return this;
            }
            
            
            if (context.InputUpdater.AttackInput)
            {
                context.AnimationController.PlayAnimationBool(context.WorldDataController.Form,"Attack",false);
                Attack(context);
                return this;
            }
            
            
            if (_countCheck >= 0)
            {
                _countCheck -= Time.deltaTime;
                return this;
            }
            context.AnimationController.PlayAnimationBool(context.WorldDataController.Form,"Attack",false);

            _attackCounter = 0;
            _attackMode = false;
            return this;
        }

        private void Attack(IEntityContext<CharacterForm> context)
        {
            if (context.Attack())
            {
                AudioManager.Instance.Play(AudioName.ArthurThrow, context.WorldDataController.Position);
                _attackCounter++;
                context.ColliderController.ChangeSprite(context.WorldDataController.Form, "ArthurThrow");
                context.AnimationController.PlayAnimationBool(context.WorldDataController.Form,"Attack",true);
                if (_attackCounter >= 3)
                {
                    _attackDelay = context.CharacterConfig.delay * 2;
                    _currentAttackDelay = _attackDelay;
                    _attackCounter = 0;
                }
                else
                {
                    _attackDelay = context.CharacterConfig.delay;
                    _currentAttackDelay = _attackDelay;
                }
                _countCheck = 0.1f;
                _attackMode = true;
            }
        }

        private void ControlMovement(IEntityContext<CharacterForm> context)
        {
            float horizontalInput = context.InputUpdater.HorizontalInput;
            if (horizontalInput >= 0.05 || horizontalInput <= -0.05)
            {
                context.ChangeScale(horizontalInput);
                context.SetHorizontalMovement(horizontalInput); 
                if (context.WorldDataController.Form == CharacterForm.Frog
                    && context.WorldDataController.IsGrounded &&
                    !context.WorldDataController.GraveIncoming)
                {
                    context.Jump(context.CharacterConfig.frogJumpWalkForce);
                    context.SetHorizontalMovement(horizontalInput*1.5f);
                    
                }
                else if(context.WorldDataController.GraveIncoming)
                {
                    context.SetHorizontalMovement(horizontalInput); 
                }
            }
            else
            {
                context.StopHorizontalMovement();
            }
        }

        public void EnterState(IEntityContext<CharacterForm> context)
        {
            _attackCounter = 0;
            context.AnimationController.PlayAnimationBool(context.WorldDataController.Form,"Idle",true);
            context.AnimationController.PlayAnimationBool(context.WorldDataController.Form,"Idle",false);
            _attackMode = false;
            context.ColliderController.ChangeSprite(context.WorldDataController.Form, "Idle");
        }


        public void ExitState(IEntityContext<CharacterForm> context)
        {
            context.AnimationController.PlayAnimationBool(context.WorldDataController.Form,"Attack",false);
            context.InputUpdater.ResetInput();
        }
    }
}