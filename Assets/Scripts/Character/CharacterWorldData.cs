using System;
using DefaultNamespace;
using Unity.VisualScripting;
using UnityEngine;
using WorldObjects;

namespace Character
{
    public class CharacterWorldData: MonoBehaviour, IWorldData<CharacterForm>
    {
        public CharacterForm Form { 
            get => characterForm;
            set => characterForm = value; }
        
        public Ladder LadderOverlap { get; private set; }
        public bool IsGrounded { get; private set; }
        public bool GotHit { get; set; }
        public int Health { get; private set;}
        public bool GraveOverlap { get; private set; }
        
        public float Velocity { get; private set; }
        public int HorizontalDirection { get; private set; }
        
        public Vector3 Position { get; private set; }
        
        public int DamageBounceDirection { get; private set; }
        
        public bool TimeFinished { get; set; }

        [SerializeField] private int regularModeHealth;
        [SerializeField] private int frogModeHealth;
        [SerializeField] private CharacterForm characterForm;
        [SerializeField] private Rigidbody2D rb2d;
        [SerializeField] private float rayLengthForArmor;
        [SerializeField] private float rayLengthForFrog;
        [SerializeField] private float rayLengthForGraves;
        private GameObject _collidedObject;
        public bool Key { get; private set; }

        public bool IsDead { get; set; }
        

        public void ResetRound()
        {
            IsDead = false;
            TimeFinished = false;
            Key = false;
            LadderOverlap = null;
            IsGrounded = true;
            GotHit = false;
            if (Form == CharacterForm.Armor)
            {
                Health = regularModeHealth;
            }
            else
            {
                Health = frogModeHealth;
            }
            GraveOverlap = false;
            Position = transform.position;
        }

        public void SetForm(int currentRound)
        {
            if (currentRound == 1)
            {
                Form = CharacterForm.Armor;
            }
            else
            {
                Form = CharacterForm.Frog;
            }
        }


        void Update()
        {
            CheckForGraveForFrog();
            CheckForGround();
            MovementCheck();
        }

        private void CheckForGraveForFrog()
        {
            Vector2 origin = rb2d.position; 
            Vector2 rayCastDirection = new Vector2(transform.localScale.x,0);
            float rayLength = rayLengthForGraves;
            RaycastHit2D hitForward = Physics2D.Raycast(origin, rayCastDirection, rayLength, GameConfig.Instance.graveColliderLayer);
            RaycastHit2D hitBackward = Physics2D.Raycast(origin, -rayCastDirection, rayLength, GameConfig.Instance.graveColliderLayer);
            
             GraveIncoming = hitForward.collider != null || hitBackward.collider != null;
             
             Debug.DrawRay(origin, rayCastDirection * rayLength, hitForward.collider != null ? Color.green: Color.red);
             Debug.DrawRay(origin, -rayCastDirection * rayLength, hitBackward.collider != null ? Color.green: Color.red); 
        }

        public bool GraveIncoming { get; private set; }

        private void MovementCheck()
        {
            HorizontalDirection = transform.localScale.x > 0 ? 1 : -1;
            Velocity = rb2d.linearVelocity.x;
            Position = transform.position;
        }


        private void CheckForGround()
        {
            Vector2 origin = rb2d.position; 
            Vector2 rayCastDirection = Vector2.down;
            float rayLength = Form == CharacterForm.Frog
                ? rayLengthForFrog
                : rayLengthForArmor;
            RaycastHit2D hit = Physics2D.Raycast(origin, rayCastDirection, rayLength, GameConfig.Instance.groundLayer);
            
            IsGrounded = hit.collider != null;
            
            Debug.DrawRay(origin, rayCastDirection * rayLength, IsGrounded ? Color.green : Color.red);
        }
        

        void OnCollisionEnter2D(Collision2D collision)
        {
            if ((1 << collision.gameObject.layer & GameConfig.Instance.enemyLayer) != 0)
            {
                if (GotHit) return;
                CharacterGotHit(collision.gameObject,collision.transform.position);
            }
            

            if ((1 << collision.gameObject.layer & GameConfig.Instance.graveColliderLayer) != 0)
            {
                GraveOverlap = true;
            }
            
            if ((1 << collision.gameObject.layer & GameConfig.Instance.keyLayer) != 0)
            {
                Key = true;
            }
        }

        private void CharacterGotHit(GameObject hitObject, Vector3 hitPosition)
        {
            Debug.Log(Health);
            GotHit = true;
            Health--;
            _collidedObject = hitObject;

            Vector3 collisionPosition = hitPosition;
            Vector3 playerPosition = transform.position;

            int directionOfAttack = 0;
            if (collisionPosition.x < playerPosition.x)
            {
                directionOfAttack = -1;
            }
            else
            {
                if (HorizontalDirection == 1)
                {
                    directionOfAttack = -1;
                }
                else
                {
                    directionOfAttack = 1;
                }
                
            }
                
            DamageBounceDirection = directionOfAttack*HorizontalDirection;
        }


        void OnDrawGizmos()
        {
            if (_collidedObject != null)
            {
                Gizmos.color = Color.red; // Set the Gizmos color
                Gizmos.DrawWireSphere(_collidedObject.transform.position, 0.5f); // Draw a sphere around the collided object
            }
        }

        void OnTriggerEnter2D(Collider2D collision)
        {
            if ((1 << collision.gameObject.layer & GameConfig.Instance.ladderLayer) != 0)
            {
                
                LadderOverlap = collision.gameObject.GetComponent<Ladder>();
            }
            if((1 << collision.gameObject.layer & GameConfig.Instance.enemyWeaponLayer) != 0)
            {
                if (GotHit) return;
                CharacterGotHit(collision.gameObject,collision.transform.position);
            }
        }
        
        

        void OnTriggerExit2D(Collider2D collision)
        {
            if ((1 << collision.gameObject.layer & GameConfig.Instance.ladderLayer) != 0)
            {
                LadderOverlap = null;
            }
        }
        
        private void OnCollisionExit2D(Collision2D other)
        {
            if ((1 << other.gameObject.layer & GameConfig.Instance.graveColliderLayer) != 0)
            {
                GraveOverlap = false;
            }
        }

        
    }
    
}