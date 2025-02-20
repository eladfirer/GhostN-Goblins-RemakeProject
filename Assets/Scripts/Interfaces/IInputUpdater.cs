namespace DefaultNamespace
{
    public interface IInputUpdater
    {
        public bool AttackInput { get; }
        public bool JumpInput { get; }
        public bool CrouchInput { get; }
        public float HorizontalInput { get;}
        public float VerticalInput { get; }


        public void UpdateInput();
        public void ResetInput();
    }
}