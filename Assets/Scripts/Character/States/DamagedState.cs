using System;
using Audio;
using DefaultNamespace;
using DG.Tweening;
using UnityEngine;

namespace Character.States
{
    public class DamagedState: IEntityState<CharacterForm>
    {
        private float _timeJump;
        private TweenCallback _invulnerabilityCallback;
        private TweenCallback _stopflickerCallback;
        private Tween _invulnerabilityTween;
        private Tween _stopFlickerTween;

        public IEntityState<CharacterForm> OnUpdate(IEntityContext<CharacterForm> context)
        {
            if (_timeJump >= 0)
            {
                _timeJump -= Time.deltaTime;
                context.SetHorizontalMovement(context.WorldDataController.DamageBounceDirection);
                return this;
            }
            
            if (context.WorldDataController.IsGrounded)
            {
                if (context.WorldDataController.Health <= 0)
                {
                    _invulnerabilityTween.Kill();
                    _invulnerabilityTween = null;
                    _stopFlickerTween.Kill();
                    _stopFlickerTween = null;
                    _stopflickerCallback.Invoke();
                    return CharacterStateFactory.GetState("Death");
                }
                return CharacterStateFactory.GetState("Move");
            }
            return this;

        }

        public void EnterState(IEntityContext<CharacterForm> context)
        {
            AudioManager.Instance.Play(AudioName.ArthurHit, context.WorldDataController.Position);
            GameConfig.Instance.ControlLayersCollision(GameConfig.Instance.enemyLayer,GameConfig.Instance.playerLayer,false);
            GameConfig.Instance.ControlLayersCollision(GameConfig.Instance.playerLayer,GameConfig.Instance.enemyWeaponLayer,false);
            context.StopHorizontalMovement();
            context.StopVerticalMovement();
            
            
            if (context.WorldDataController.Form == CharacterForm.Armor)
            {
                context.CharacterConfig.CreateDamageArmor(context.WorldDataController.Position);
            }
            
            context.AnimationController.PlayAnimationBool(context.WorldDataController.Form,"Damaged", true);
            
            context.Jump(context.CharacterConfig.damagedJumpForce);
            _timeJump = context.CharacterConfig.damagedTimeJump;
            _stopflickerCallback = context.ColliderController.Flicker();
            _invulnerabilityCallback = () =>
            {
                GameConfig.Instance.ControlLayersCollision(GameConfig.Instance.enemyLayer,
                    GameConfig.Instance.playerLayer, true);
                GameConfig.Instance.ControlLayersCollision(GameConfig.Instance.enemyWeaponLayer,
                    GameConfig.Instance.playerLayer, true);
            };
            _stopFlickerTween = DOVirtual.DelayedCall(context.CharacterConfig.invulnerableTime, _stopflickerCallback);
            _invulnerabilityTween = DOVirtual.DelayedCall(context.CharacterConfig.invulnerableTime, _invulnerabilityCallback);
            
        }

        public void ExitState(IEntityContext<CharacterForm> context)
        {
            context.WorldDataController.GotHit = false;
            if (context.WorldDataController.Form == CharacterForm.Armor)
            {
                context.WorldDataController.Form = CharacterForm.NoArmor;
            }
            context.AnimationController.PlayAnimationBool(context.WorldDataController.Form,"Damaged", false);
        }
    }
}