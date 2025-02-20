using System;
using Character;
using GameStates;
using Interfaces;
using UI;
using UnityEngine;

public class GameManager: MonoBehaviour
{
    private PlayerControls _playerControls;
    [SerializeField] private Stage1State stage1State;
    [SerializeField] private StartScreenState startScreenState;
    [SerializeField] private BossStageState bossStageState;
    [SerializeField] private CutSceneState cutSceneState;
    [SerializeField] private CharacterContext arthur;
    
    public int CurrentPlayerLifes { get; set; }
    public int CurrentRound { get; set; }
    private IGameState _activeState;
    public bool firstPlay;

    public IGameState StateToMoveTo { get; set; }

    private void Awake()
    {
        _playerControls = new PlayerControls();
        arthur.PlayerControls = _playerControls;
        startScreenState.PlayerControls = _playerControls;
        stage1State.PlayerControls = _playerControls;
        cutSceneState.PlayerControls = _playerControls;
        bossStageState.PlayerControls = _playerControls;
        GameStateFactory.AddState(GameState.Stage1,stage1State);
        GameStateFactory.AddState(GameState.StartMenu,startScreenState);
        GameStateFactory.AddState(GameState.StageBoss,bossStageState);
        GameStateFactory.AddState(GameState.CutScene,cutSceneState);
        _activeState = startScreenState;
        _activeState.EnterState(this);
        StateToMoveTo = _activeState;
    }

    
    void OnDisable()
    {
        _playerControls.Disable();
    }

    private void Update()
    {
        var nextState = _activeState.OnUpdate(this);
        if (nextState != _activeState)
        {
            _activeState.ExitState();
             nextState.EnterState(this);
            _activeState = nextState;
        }
    }
    
}

public enum GameState
{
    StartMenu,
    Stage1,
    StageBoss,
    CutScene
}