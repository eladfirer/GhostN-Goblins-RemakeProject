using DefaultNamespace;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.VirtualTexturing;
using WorldObjects;

namespace Character.States
{
    
    public sealed class ClimbLadderState: IEntityState<CharacterForm>
    {
        private const float LimitFinalClimbMode = 0f;
        private const float MidFinalClimbMode = 1f;
        private const float EndFinalClimbMode = 2f;
        private float _characterDefaultGravity;
        private Ladder _ladder;
        float _currentChange;
        private bool _inFinalPosition;
        private Vector3 _playerPosition;
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
            _playerPosition = context.WorldDataController.Position;
            if (_inFinalPosition)
            {
                return HandleFinalClimb(context);
            }
            if (_ladder.IsAboveBeforeFinalClimb(_playerPosition + new Vector3(0, -context.CharacterConfig.bufferEdgeLadder, 0)))
            {
                _inFinalPosition = true;
                context.ChangePosition(_ladder.BeforeFinalClimbLadder);
                context.StopVerticalMovement();
                _currentChange = context.CharacterConfig.changeAboveBeforeFinalClimb;
                return this;
            }
            if (_ladder.IsBellowLadder(_playerPosition + new Vector3(0, context.CharacterConfig.bufferEdgeLadder, 0)))
            {
                context.ChangePosition(_ladder.BellowLadderPosition);
                return CharacterStateFactory.GetState("Move");
            }
            context.AnimationController.PlayAnimationBool(context.WorldDataController.Form,"ClimbBeforeFinal",false);
            context.AnimationController.PlayAnimationBool(context.WorldDataController.Form,"ClimbFinal",false);
            
            ControlMovement(context);
            return this;
            
        }

        private IEntityState<CharacterForm> HandleFinalClimb(IEntityContext<CharacterForm> context)
        {
            if (context.InputUpdater.VerticalInput != 0)
            {
                _currentChange += Mathf.Sign(context.InputUpdater.VerticalInput) * context.CharacterConfig.moveAdjuster;
            }
            if (_currentChange < LimitFinalClimbMode)
            {
                context.ChangePosition(_ladder.BeforeFinalClimbLadder + new Vector3(0, -context.CharacterConfig.beforeFinalClimbBuffer, 0));
                _inFinalPosition = false;
                context.AnimationController.PlayAnimationBool(context.WorldDataController.Form,"ClimbBeforeFinal",false);
                context.ColliderController.ChangeSprite(context.WorldDataController.Form,"FirstClimb");
                context.ChangePosition(context.WorldDataController.Position + new Vector3(0, context.CharacterConfig.positionChangeOutOfFinalClimb, 0));
                return this;
            }
            if (_currentChange < MidFinalClimbMode)
            {
                context.ChangePosition(_ladder.BeforeFinalClimbLadder);
                context.AnimationController.PlayAnimationBool(context.WorldDataController.Form,"ClimbFinal",false);
                context.AnimationController.PlayAnimationBool(context.WorldDataController.Form,"ClimbBeforeFinal",true);
                context.ColliderController.ChangeSprite(context.WorldDataController.Form,"ClimbBeforeFinal");
                return this;
            }
            if (_currentChange < EndFinalClimbMode)
            {
                context.ChangePosition(_ladder.FinalClimbLadderPosition);
                context.AnimationController.PlayAnimationBool(context.WorldDataController.Form,"ClimbBeforeFinal",false);
                context.AnimationController.PlayAnimationBool(context.WorldDataController.Form,"ClimbFinal",true);
                context.ColliderController.ChangeSprite(context.WorldDataController.Form,"ClimbFinal");
                return this;
            }
            context.ChangePosition(_ladder.AboveLadderPosition);
            context.AnimationController.PlayAnimationBool(context.WorldDataController.Form,"ClimbFinal",false);
            return CharacterStateFactory.GetState("Move");
        }

        private void ControlMovement(IEntityContext<CharacterForm> context, float moveAdjuster = 1.0f)
        {
            if (context.InputUpdater.VerticalInput != 0)
            {
                context.SetVerticalMovement(context.InputUpdater.VerticalInput*moveAdjuster);
                return;
            }
            context.StopVerticalMovement();
        }
        public void EnterState(IEntityContext<CharacterForm> context)
        {
            _ladder = context.WorldDataController.LadderOverlap;
            _ladder.SetCollision(context.ColliderController.CapsuleCollider,true);
            _characterDefaultGravity = context.Rigidbody2D.gravityScale;
            context.Rigidbody2D.gravityScale = 0;
            LadderPositions closestLadderPosition = 
                _ladder.GetClosestLadder(context.WorldDataController.Position);
            context.AnimationController.PlayAnimationBool(context.WorldDataController.Form,"Climb",true);
            if (closestLadderPosition == LadderPositions.Upper)
            {
                context.ChangePosition(_ladder.FinalClimbLadderPosition);
                _inFinalPosition = true;
                _currentChange = context.CharacterConfig.changeEnterIntoClimbModeFromAbove;
                context.ColliderController.ChangeSprite(context.WorldDataController.Form,"ClimbFinal");
            }
            else
            {
                context.ChangePosition(_ladder.BellowLadderPosition);
                _currentChange = 0f;
                _inFinalPosition = false;
                context.ColliderController.ChangeSprite(context.WorldDataController.Form,"FirstClimb");
            }
            
        }
        

        public void ExitState(IEntityContext<CharacterForm> context)
        {
            context.StopVerticalMovement();
            context.AnimationController.PlayAnimationBool(context.WorldDataController.Form,"Climb",false);
            context.AnimationController.PlayAnimationBool(context.WorldDataController.Form,"ClimbBeforeFinal",false);
            context.AnimationController.PlayAnimationBool(context.WorldDataController.Form,"ClimbFinal",false);
            _ladder.SetCollision(context.ColliderController.CapsuleCollider,false);
            context.Rigidbody2D.gravityScale = _characterDefaultGravity;
            context.InputUpdater.ResetInput();
        }
    }
}