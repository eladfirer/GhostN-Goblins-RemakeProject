using Audio;
using DefaultNamespace;
using DG.Tweening;

namespace Character.States
{
    public sealed class DeathState: IEntityState<CharacterForm>
    {
        private IEntityState<CharacterForm> _entityState;
        public IEntityState<CharacterForm> OnUpdate(IEntityContext<CharacterForm> context)
        {
            return this;
        }

        
        public void EnterState(IEntityContext<CharacterForm> context)
        {
            context.StopGravity();
            _entityState = this;
            GameConfig.Instance.ControlLayersCollision(GameConfig.Instance.enemyLayer,GameConfig.Instance.playerLayer,false);
            context.StopHorizontalMovement();
            context.StopVerticalMovement();
            context.AnimationController.PlayAnimationBool(context.WorldDataController.Form,"DeathPart1",true);
            context.WorldDataController.IsDead = true;
            // AudioManager.Instance.Play(AudioName.ArthurDeath, context.WorldDataController.Position);
            DOVirtual.DelayedCall(context.CharacterConfig.delayTimeDeathAnimation, () =>
            {
                context.ColliderController.ChangeSprite(context.WorldDataController.Form,"DeathPart2");
                context.AnimationController.PlayAnimationBool(context.WorldDataController.Form, "DeathPart2", true);
                // DOVirtual.DelayedCall(context.CharacterConfig.delayTimeDeathAnimation,
                //     () =>
                //     {
                //         context.WorldDataController.IsDead = true;
                //         AudioManager.Instance.Play(AudioName.ArthurDeath, context.WorldDataController.Position);
                //     });
            });
            

        }

        public void ExitState(IEntityContext<CharacterForm> context)
        {
            GameConfig.Instance.ControlLayersCollision(GameConfig.Instance.enemyWeaponLayer,GameConfig.Instance.playerLayer,true);
            GameConfig.Instance.ControlLayersCollision(GameConfig.Instance.enemyLayer,GameConfig.Instance.playerLayer,true);
            context.AnimationController.PlayAnimationBool(context.WorldDataController.Form,"DeathPart1",false);
            context.AnimationController.PlayAnimationBool(context.WorldDataController.Form, "DeathPart2", false);
            context.GravityBack();
        }
    }
}