using System;
using UnityEngine;

namespace WorldObjects
{
    public class Ladder : MonoBehaviour
    {
        [SerializeField] private Transform finalClimbLadder;
        [SerializeField] private Transform beforeFinalClimbLadder;
        [SerializeField] private Transform aboveLadder;
        [SerializeField] private Transform bellowLadder;
        [SerializeField] private BoxCollider2D aboveLadderGroundBoxCollider;
        public Vector3 AboveLadderPosition => aboveLadder.position;
        public Vector3 BellowLadderPosition => bellowLadder.position;
        public Vector3 FinalClimbLadderPosition => finalClimbLadder.position;
        public Vector3 BeforeFinalClimbLadder => beforeFinalClimbLadder.position;
        public LadderPositions GetClosestLadder(Vector3 characterPosition)
        {
            float distanceToUpper = Vector3.Distance(characterPosition, aboveLadder.position);
            float distanceToLower = Vector3.Distance(characterPosition, bellowLadder.position);
            
            if (distanceToUpper < distanceToLower)
            {
                return LadderPositions.Upper;
            }
            return LadderPositions.Lower;
            
        }
        
        public bool IsAboveLadder(Vector3 characterPosition)
        {
            return characterPosition.y > aboveLadder.position.y;
        }
        public bool IsBellowLadder(Vector3 characterPosition)
        {
            return characterPosition.y < bellowLadder.position.y;
        }

        public void SetCollision(CapsuleCollider2D characterCollider, bool collision)
        {
            Physics2D.IgnoreCollision(characterCollider, aboveLadderGroundBoxCollider, collision);
        }

        public bool IsAboveBeforeFinalClimb(Vector3 playerPosition)
        {
            return playerPosition.y > beforeFinalClimbLadder.position.y;
        }
    }

    public enum LadderPositions
    {
        Upper,
        Lower
    }
}