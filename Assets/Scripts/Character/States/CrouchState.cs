using Audio;
using DefaultNamespace;
using UnityEngine;

namespace Character.States
{
    public sealed class CrouchState: IEntityState<CharacterForm>
    {
        private float _countCheck;
        private bool _crouchModeAttack;
        private float _attackDelay;
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
            
            if (_crouchModeAttack)
            {
                return AttackMode(context);
            }
            if (_countCheck >= 0)
            {
                _countCheck -= Time.deltaTime;
                return this;
            }
            if (context.InputUpdater.AttackInput)
            {
                Attack(context);
                return this;
            }
            
            
            if (context.InputUpdater.VerticalInput < 0)
            {
                return this;
            }
            return CharacterStateFactory.GetState("Move");
        }

        private void Attack(IEntityContext<CharacterForm> context)
        {
            context.Attack();
            _attackCounter++;
            AudioManager.Instance.Play(AudioName.ArthurThrow, context.WorldDataController.Position);
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
            context.AnimationController.PlayAnimationBool(context.WorldDataController.Form,"Attack",true);
            _countCheck = context.CharacterConfig.check;
            _crouchModeAttack = true;
        }

        private IEntityState<CharacterForm> AttackMode(IEntityContext<CharacterForm> context)
        {
            if (context.InputUpdater.HorizontalInput != 0)
            {
                context.ChangeScale(context.InputUpdater.HorizontalInput);
            }
            if (_currentAttackDelay >= 0)
            {
                _currentAttackDelay -= Time.deltaTime;
                return this;
            }
            
            if (context.InputUpdater.AttackInput)
            {
                Attack(context);
                return this;
            }
            
            if (_countCheck >= 0)
            {
                _countCheck -= Time.deltaTime;
                return this;
            }
            context.AnimationController.PlayAnimationBool(context.WorldDataController.Form,"Attack",false);
            return CharacterStateFactory.GetState("Move");
        }

        public void EnterState(IEntityContext<CharacterForm> context)
        {
            _attackDelay = context.CharacterConfig.delay;
            _crouchModeAttack = false;
            context.StopHorizontalMovement();
            _countCheck = context.CharacterConfig.check;
            context.AnimationController.PlayAnimationBool(context.WorldDataController.Form,"Crouch", true);
            context.ColliderController.ChangeSprite(context.WorldDataController.Form,"Crouch");
        }
        

        public void ExitState(IEntityContext<CharacterForm> context)
        {
            context.AnimationController.PlayAnimationBool(context.WorldDataController.Form,"Attack",false);
            context.AnimationController.PlayAnimationBool(context.WorldDataController.Form, "Crouch", false);
            context.InputUpdater.ResetInput();
        }
    }
}