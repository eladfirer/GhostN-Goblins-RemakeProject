using System.Collections;
using System.Collections.Generic;
using Audio;
using Camera;
using Character;
using DG.Tweening;
using Enemies.Plant;
using Enemies.Zombie;
using Interfaces;
using UI;
using UnityEngine;
using WorldObjects;

namespace GameStates
{
    public class Stage1State: MonoBehaviour, IGameState
    {
        [SerializeField] private CharacterContext arthur;
        [SerializeField] private Transform arthurSpawnPoint;
        [SerializeField] private ZombieSpawner zombieSpawner;
        [SerializeField] private Plant plant1;
        [SerializeField] private Plant plant2;
        [SerializeField] private List<Bird> birds;
        [SerializeField] private Transform noZombieSpawnBorder;
        [SerializeField] private float delayTime;
        [SerializeField] private Transform spawnKeyPointTrigger;
        [SerializeField] private Transform spawnKeyPoint;
        [SerializeField] private Key keyPrefab;
        [SerializeField] private float timeDelayAfterDeath;
        [SerializeField] private float timeDelayAfterGameOverMessage;
        [SerializeField] private float timeDelayAfterWin;
        [SerializeField] private float delayZombieDestroy;
        private bool _deactivtedZombies;
        private bool _deactivtedKey;
        private bool _stopTime;
        private float _timeElapsed;
        private bool _gameMenu;
        private float _timeDelayAfterDeathTracker;
        private bool _bossStage;
        private bool _oneEntry;
        private SoundObject _soundtrackStage;
        private bool _playerZeroHealth;
        public PlayerControls PlayerControls { get; set; }

        
        

        public void EnterState(GameManager gameManager)
        {
            PlayerControls.Disable();
            arthur.transform.position = arthurSpawnPoint.position;
            arthur.gameObject.SetActive(true);
            arthur.ResetArthur(gameManager.CurrentRound);
            _oneEntry = false;
            _bossStage = false;
            _playerZeroHealth = false;
            _timeDelayAfterDeathTracker = timeDelayAfterDeath;
            _gameMenu = false;
            _stopTime = true;
            _deactivtedZombies = false;
            _deactivtedKey = false;
            CameraController.Instance.SetGameplayCamera();
            UiManager.Instance.SetBlackScreenActive(true);
            StartCoroutine(SpawnStage(gameManager));
        }
        

        private IEnumerator SpawnStage(GameManager gameManager)
        {
            yield return new WaitForSeconds(delayTime);
            SpawnEnemies();
            TimeUpdater.SetTimeStart();
            UiManager.Instance.SetWeaponBoxActive(true);
            if (arthur.WorldDataController.Form == CharacterForm.Frog)
            {
                UiManager.Instance.SetTongueActive(true);
            }
            else
            {
                UiManager.Instance.SetSpearActive(true);
            }

            if (gameManager.CurrentPlayerLifes == 3)
            {
                UiManager.Instance.SetLife1Active(true);
                UiManager.Instance.SetLife2Active(true);
            }
            else if (gameManager.CurrentPlayerLifes == 2)
            {
                UiManager.Instance.SetLife1Active(true);
            }
            UiManager.Instance.SetBlackScreenActive(false);
            _timeElapsed = 0f;
            _stopTime = false;
            PlayerControls.Enable();
            if (gameManager.firstPlay)
            {
                gameManager.firstPlay = false;
                _soundtrackStage = AudioManager.Instance.Play(AudioName.GameStart, transform.position);
                yield return new WaitForSeconds(_soundtrackStage.audioSource.clip.length - 0.5f); 
            }
            
            
            _soundtrackStage = AudioManager.Instance.Play(AudioName.Stage1, arthur.WorldDataController.Position);
            
        }

        private void SpawnEnemies()
        {
            foreach (var bird in birds)
            {
                bird.Activate();
            }
            plant1.Activate();
            plant2.Activate();
            zombieSpawner.ActivateSpawnZombies();
        }

        private void StopStage()
        {
            zombieSpawner.DestroyAllZombies();
            foreach (var bird in birds)
            {
                bird.Deactivate();
            }
            plant1.Deactivate();
            plant2.Deactivate();
            
        }

        public IGameState OnUpdate(GameManager gameManager)
        {
            if (_bossStage)
            {
                PlayerControls.Enable();
                gameManager.StateToMoveTo = GameStateFactory.GetState(GameState.StageBoss);
                return GameStateFactory.GetState(GameState.CutScene);
            }
            if (_gameMenu)
            {
                UiManager.Instance.SetGameOverActive(false);
                return GameStateFactory.GetState(GameState.StartMenu);
            }
            if (arthur.WorldDataController.IsDead)
            {
                _soundtrackStage.StopSound();
                return HandleDeath(gameManager);
            }
            if (arthur.WorldDataController.Key && arthur.WorldDataController.IsGrounded 
                && !_oneEntry)
            {
                _oneEntry = true;
                PlayerControls.Disable();
                arthur.AnimationController.PlayAnimationTrigger(arthur.WorldDataController.Form,"Win");
                DOVirtual.DelayedCall(timeDelayAfterWin,(() => _bossStage = true));
            }
            if (!_stopTime)
            {
                _timeElapsed += Time.deltaTime;
                
                if (_timeElapsed >= 1f)
                {
                    int timeUpdate = TimeUpdater.TimeUpdate();
                    if (timeUpdate == 15)
                    {
                        _soundtrackStage.StopSound();
                        _soundtrackStage = AudioManager.Instance.Play(AudioName.Timer, arthur.WorldDataController.Position);
                    }

                    if (timeUpdate == 0)
                    {
                        arthur.WorldDataController.TimeFinished = true;
                        return this;
                    }
                    
                    
                    _timeElapsed = 0f;
                }
            }

            
            if (!_deactivtedZombies && arthur.transform.position.x > noZombieSpawnBorder.position.x)
            {
                zombieSpawner.DeactivateSpawnZombies();
                _deactivtedZombies = true;
            }

            if (!_deactivtedKey && arthur.transform.position.x > spawnKeyPointTrigger.position.x)
            {
                zombieSpawner.DestroyAllZombies();
                plant1.Deactivate();
                plant2.Deactivate();
                _soundtrackStage.StopSound();
                AudioManager.Instance.Play(AudioName.Key, arthur.WorldDataController.Position);
                Instantiate(keyPrefab, spawnKeyPoint.position, Quaternion.identity);
                UiManager.Instance.SetTakeAKeyActive(true);
                _deactivtedKey = true;
                _stopTime = true;
            }

            return this;

        }

        private IGameState HandleDeath(GameManager gameManager)
        {
            if (!_oneEntry)
            {
                _stopTime = true;
                gameManager.CurrentPlayerLifes--;
                zombieSpawner.DeactivateSpawnZombies();
                plant1.Deactivate();
                plant2.Deactivate();
                foreach (var bird in birds)
                {
                    bird.Deactivate();
                }
                Debug.Log("Deactives spaen zombies");
                zombieSpawner.StopAllZombiesFromMoving();
                _oneEntry = true;
                if (gameManager.CurrentPlayerLifes == 0)
                {
                    AudioManager.Instance.Play(AudioName.GameOver, transform.position);
                    UiManager.Instance.SetGameOverActive(true);
                    _playerZeroHealth = true;
                    DOVirtual.DelayedCall(timeDelayAfterGameOverMessage,(() => _gameMenu = true));
                }
                else
                {
                    AudioManager.Instance.Play(AudioName.ArthurDeath, arthur.transform.position);
                }
                gameManager.StateToMoveTo = this;
                return this;
            }
            _timeDelayAfterDeathTracker -= Time.deltaTime;
            if (!_playerZeroHealth && _timeDelayAfterDeathTracker <= 0)
            {
                return GameStateFactory.GetState(GameState.CutScene);
            }
            return this;
        }

        public void ExitState()
        {
            StopStage();
            TimeUpdater.ResetTimerAndRemoveUI();
            UiManager.Instance.SetGameOverActive(false);
            UiManager.Instance.SetTakeAKeyActive(false);
            UiManager.Instance.SetLife1Active(false);
            UiManager.Instance.SetLife2Active(false);
            UiManager.Instance.SetTongueActive(false);
            UiManager.Instance.SetSpearActive(false);
            UiManager.Instance.SetWeaponBoxActive(false);
            plant1.gameObject.SetActive(false);
            plant2.gameObject.SetActive(false);
            foreach (var bird in birds)
            {
                bird.gameObject.SetActive(false);
            }
            arthur.gameObject.SetActive(false);
        }
    }
}