using Character.States;
using Character.Weapons;
using DefaultNamespace;
using UnityEngine;

namespace Character
{
    public sealed class CharacterContext: MonoBehaviour, IEntityContext<CharacterForm>
    {
        
        
        private IEntityState<CharacterForm> ActiveState { get; set; }
        
        [SerializeField] private CharacterColliderController colliderManager;
        [SerializeField] private CharacterAnimationController animationManager;
        private CharacterInputUpdater _inputUpdater;
        [SerializeField] private CharacterWorldData worldData;
        public PlayerControls PlayerControls { get; set; }
        [SerializeField] private Tongue frogWeapon;
        
        [SerializeField] private float moveSpeedHorizontal;
        [SerializeField] private float moveSpeedVertical;
        [SerializeField] private CharacterConfig config;
        public IColliderController<CharacterForm> ColliderController => colliderManager;
        public IAnimationController<CharacterForm> AnimationController => animationManager;
        public IInputUpdater InputUpdater => _inputUpdater;
        public IWorldData<CharacterForm> WorldDataController => worldData;
        
        
        public Rigidbody2D Rigidbody2D { get; private set; }
        public CharacterConfig CharacterConfig => config;

        void Awake()
        {
            Rigidbody2D = GetComponent<Rigidbody2D>();
            _inputUpdater = new CharacterInputUpdater(PlayerControls);
            ActiveState = CharacterStateFactory.GetState("Move"); 
        }
        

        public void ResetArthur(int currentRound)
        {
            WorldDataController.SetForm(currentRound);
            WorldDataController.ResetRound();
            ActiveState.ExitState(this);
            ActiveState = CharacterStateFactory.GetState("Move");
            ActiveState.EnterState(this);
            transform.localScale = Vector3.one;
            
        }

        void Update()
        {
            _inputUpdater.UpdateInput();
            
            var nextState = ActiveState.OnUpdate(this);
            if (nextState != ActiveState)
            {
                ActiveState.ExitState(this);
                nextState.EnterState(this);
                ActiveState = nextState;
            }
            
            
        }

        public bool Attack()
        {
            if (WorldDataController.Form == CharacterForm.Frog)
            {
                return frogWeapon.Attack(transform.position,WorldDataController.HorizontalDirection) &&
                       WorldDataController.IsGrounded;
            }
            Spear weapon = SpearPool.Instance.Get();
            weapon.ArthurTransform = transform;
            if (ActiveState == CharacterStateFactory.GetState("Crouch"))
            {
                return weapon.Attack(new Vector3(transform.position.x,
                    transform.position.y - 0.05f,
                    transform.position.z)
                    ,WorldDataController.HorizontalDirection); 
            }
            return weapon.Attack(transform.position,WorldDataController.HorizontalDirection); 
        
        }
        
        public void SetHorizontalMovement(float horizontalInput)
        {
            Rigidbody2D.linearVelocity = new Vector2(horizontalInput * moveSpeedHorizontal, Rigidbody2D.linearVelocity.y);
            animationManager.SetFloat("HorizontalSpeed", Mathf.Abs(horizontalInput * moveSpeedHorizontal));
            
        }

        public void ChangeScale(float horizontalInput)
        {
            var localScale = transform.localScale;
            transform.localScale = new Vector3(
                Mathf.Sign(horizontalInput) * Mathf.Abs(localScale.x),
                localScale.y,
                localScale.z
            );
        }

        public void StopHorizontalMovement()
        {
            Rigidbody2D.linearVelocity = new Vector2(0, Rigidbody2D.linearVelocity.y);
            animationManager.SetFloat("HorizontalSpeed", 0);
        }
        
        public void StopVerticalMovement()
        {
            Rigidbody2D.linearVelocity = new Vector2( Rigidbody2D.linearVelocity.x, 0);
            animationManager.SetFloat("VerticalSpeed", 0);
        }

        public void StopGravity()
        {
            Rigidbody2D.gravityScale = 0;
        }

        public void GravityBack()
        {
            Rigidbody2D.gravityScale = 1;
        }

        public void SetVerticalMovement(float verticalInput)
        {
            Rigidbody2D.linearVelocity = new Vector2(Rigidbody2D.linearVelocity.x , verticalInput * moveSpeedVertical);
            animationManager.SetFloat("VerticalSpeed", Mathf.Abs(verticalInput * moveSpeedVertical));
        }
        
        public void Jump(float jumpForce)
        {
            Rigidbody2D.linearVelocity = new Vector2(Rigidbody2D.linearVelocity.x, jumpForce);
        }

        public void ChangePosition(Vector3 position)
        {
            transform.position = position;
        }
        
    }
}