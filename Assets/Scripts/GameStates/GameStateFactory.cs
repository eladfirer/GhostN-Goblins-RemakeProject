using System;
using System.Collections.Generic;
using Interfaces;
using UnityEngine;

namespace GameStates
{
    public static class GameStateFactory
    {
        private static readonly Dictionary<GameState, IGameState> StateCache = new Dictionary<GameState, IGameState>();


        public static void AddState(GameState state, IGameState stateImplementation)
        {
            if (!StateCache.ContainsKey(state))
            {
                StateCache.Add(state, stateImplementation);
            }
            else
            {
                Debug.LogWarning($"State {state} is already added to the StateCache.");
            }
        }
        public static IGameState GetState(GameState stateName)
        {
            if (StateCache.TryGetValue(stateName, out var state))
            {
                return state;
            }

            throw new ArgumentException($"State '{stateName}' not found in StateFactory.");
        }
    }
}