using UnityEngine;
using WorldObjects;

namespace Character
{
    [CreateAssetMenu(fileName = "CharacterConfig", menuName = "Ghosts n' Goblins/CharacterConfig")]
    public class CharacterConfig : ScriptableObject
    {
        public float check = 0.2f;
        public float delay = 0.2f;
        public float moveForwardNormalizer = 1f;
        public float graveCollideJumpedTimer = 0.25f;
        public float crouchCheck = 0.3f;
        public float changeAboveBeforeFinalClimb = 0.5f;
        public float changeEnterIntoClimbModeFromAbove = 1.5f;
        public float positionChangeOutOfFinalClimb = -0.02f;
        public float bufferEdgeLadder = 0.01f;
        public float beforeFinalClimbBuffer = 0.05f;
        public float moveAdjuster = 0.005f;
        public float regularJumpedTimer = 0.2f;
        public float jumpForce = 2.8f;
        
        public float damagedJumpForce = 2.7f;
        public float damagedTimeJump = 0.3f;

        public float invulnerableTime = 3f;
        
        public DamageArmor damageArmorGameObject;
        public float delayTimeDeathAnimation;
        public float frogJumpWalkForce;

        public void CreateDamageArmor( Vector3 position)
        {
            var damageArmor =  Instantiate(damageArmorGameObject, position, Quaternion.identity );
            damageArmor.Activate();
        }

    }
}
