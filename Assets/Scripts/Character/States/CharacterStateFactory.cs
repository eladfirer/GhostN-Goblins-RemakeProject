using System;
using System.Collections.Generic;
using DefaultNamespace;

namespace Character.States
{
    public static class CharacterStateFactory
    {
        // Dictionary to store all available states
        private static readonly Dictionary<string, IEntityState<CharacterForm>> StateCache = new Dictionary<string, IEntityState<CharacterForm>>
        {
            { "Move", new MoveState() },
            { "Jump", new JumpState() },
            { "Crouch", new CrouchState() },
            { "ClimbLadder", new ClimbLadderState() },
            { "Damaged", new DamagedState() },
            { "Death", new DeathState() }
            
        };
        
        public static IEntityState<CharacterForm> GetState(string stateName)
        {
            if (StateCache.TryGetValue(stateName, out var state))
            {
                return state;
            }

            throw new ArgumentException($"State '{stateName}' not found in StateFactory.");
        }
    }
}