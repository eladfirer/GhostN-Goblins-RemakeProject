using DefaultNamespace;
using UnityEngine;

namespace Character
{
    public class CharacterInputUpdater: IInputUpdater
    {
        private readonly PlayerControls _playerControls;
        public bool AttackInput { get; set;}
        public bool JumpInput { get; set;}
        public bool CrouchInput { get; set; }
        public float HorizontalInput { get; private set; }
        public float VerticalInput { get; private set; }
        
        
        public void UpdateInput()
        {
            Vector2 moveInput = _playerControls.Arthur.Move.ReadValue<Vector2>();
            HorizontalInput = moveInput.x;
            VerticalInput = moveInput.y;
            AttackInput = _playerControls.Arthur.Attack.triggered;
            JumpInput = _playerControls.Arthur.Jump.triggered;
            CrouchInput = _playerControls.Arthur.Crouch.triggered;
        }

        public CharacterInputUpdater(PlayerControls playerControls)
        {
            _playerControls = playerControls;
        }

        public void ResetInput()
        {
            HorizontalInput = 0;
            VerticalInput = 0;
            AttackInput = false;
            JumpInput = false;
            CrouchInput = false;
        }
    }
}