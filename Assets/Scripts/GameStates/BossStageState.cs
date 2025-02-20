using System.Collections;
using Audio;
using Camera;
using Character;
using DG.Tweening;
using Enemies.Boss;
using Interfaces;
using UI;
using Unity.VisualScripting;
using UnityEngine;

namespace GameStates
{
    public class BossStageState: MonoBehaviour, IGameState
    {
        [SerializeField] private Transform bossSpawnPoint;
        [SerializeField] private Transform arthurSpawnPoint;
        [SerializeField] private Transform princessSpawnPoint;
        [SerializeField] private Transform princessBattlePoint;
        [SerializeField] private Transform princessEndPoint;
        [SerializeField] private Transform arthurEndPoint;
        [SerializeField] private GameObject princess;
        [SerializeField] private Animator princessAnimator;
        [SerializeField] private Boss boss;
        [SerializeField] private CharacterContext arthur;
        [SerializeField] private float delayTime;
        [SerializeField] private float resolveYourBattleFirstDelay;
        [SerializeField] private float resolveYourBattleSecondDelay;
        [SerializeField] private float resolveYourBattleThirdDelay;

        [SerializeField] private float timeToEndPositionArthur;
        [SerializeField] private float timeToEndPositionPrincess;
        [SerializeField] private float timeForPlayerToReachGround;
        [SerializeField] private float timeOfEndSubs;
        [SerializeField] private float timeOfHappyEndingSubs;
        [SerializeField] private float timeOfReturnToStartpoint;
        [SerializeField] private float timeDelayAfterDeathTracker;
        [SerializeField] private float timeDelayAfterGameOverMessage;
        [SerializeField] private float arthurTransformIntoKnightTime;
        
        private float _timeElapsed;
        private bool _stopTime;
        private bool _oneEntry;
        private bool _endRound1;
        private bool _startMenu;
        private bool _bossStage;
        private SoundObject _soundtrackStage;
        public PlayerControls PlayerControls { get; set; }
        public void EnterState(GameManager gameManager)
        {
            _stopTime = true;
            _oneEntry = false;
            _endRound1 = false;
            _startMenu = false;
            _bossStage = false;
            _timeElapsed = 0f;
            arthur.ResetArthur(gameManager.CurrentRound);
            PlayerControls.Disable();
            CameraController.Instance.SetBossFightCamera();
            UiManager.Instance.SetBlackScreenActive(true);
            DOVirtual.DelayedCall(delayTime,() => StartCoroutine(SpawnStage(gameManager)));
        }

        private IEnumerator SpawnStage(GameManager gameManager)
        {
            SetCharacters(gameManager);
            UiManager.Instance.SetBlackScreenActive(false);
            UiManager.Instance.SetResolveYourBattleActive(true);
            _soundtrackStage = AudioManager.Instance.Play(AudioName.StageBossStart, arthur.WorldDataController.Position);
            yield return new WaitForSeconds(resolveYourBattleFirstDelay);
            princessAnimator.SetBool("Move", true);
            princess.transform.DOMove(princessBattlePoint.transform.position,resolveYourBattleSecondDelay).SetEase(Ease.Linear);
            yield return new WaitForSeconds(resolveYourBattleSecondDelay);
            princessAnimator.SetBool("Move", false);
            UiManager.Instance.SetBlackScreenActive(true);
            UiManager.Instance.SetResolveYourBattleActive(false);
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
            yield return new WaitForSeconds(resolveYourBattleThirdDelay);
            UiManager.Instance.SetBlackScreenActive(false);
            PlayerControls.Enable();
            _stopTime = false;
            boss.Activate();
            _soundtrackStage.StopSound();
            _soundtrackStage = AudioManager.Instance.Play(AudioName.BossStage, arthur.WorldDataController.Position);
        }

        private void SetCharacters(GameManager gameManager)
        {
            arthur.transform.position = arthurSpawnPoint.position;
            boss.transform.position = bossSpawnPoint.position;
            princess.transform.position = princessSpawnPoint.position;
            princess.SetActive(true);
            arthur.gameObject.SetActive(true);
            boss.gameObject.SetActive(true);
        }

        public IGameState OnUpdate(GameManager gameManager)
        {
            if (_bossStage)
            {
                gameManager.StateToMoveTo = this;
                return GameStateFactory.GetState(GameState.CutScene);
            }
            if (_endRound1)
            {
                gameManager.StateToMoveTo =  GameStateFactory.GetState(GameState.Stage1);
                return GameStateFactory.GetState(GameState.CutScene);
            }

            if (_startMenu)
            {
                return GameStateFactory.GetState(GameState.StartMenu);
            }

            if (arthur.WorldDataController.IsDead && !_oneEntry)
            {
                _oneEntry = true;
                StartCoroutine(ArthurDead(gameManager));
            }
            if (boss.IsDead && !_oneEntry)
            {
                _oneEntry = true;
                _stopTime = true;
                _soundtrackStage.StopSound();
                if (gameManager.CurrentRound == 1)
                {
                    AudioManager.Instance.Play(AudioName.BossFirstDeath, boss.transform.position);
                    gameManager.CurrentRound++;
                    StartCoroutine(IllusionRound());
                }
                else
                {
                    AudioManager.Instance.Play(AudioName.BossSecondDeath, boss.transform.position);
                    StartCoroutine(WinRound());
                }
                
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
            return this;
        }

        private IEnumerator ArthurDead(GameManager gameManager)
        {
            boss.Deactivate();
            _stopTime = true;
            _soundtrackStage.StopSound();
            gameManager.CurrentPlayerLifes--;
            if (gameManager.CurrentPlayerLifes == 0)
            {
                AudioManager.Instance.Play(AudioName.GameOver, transform.position);
                UiManager.Instance.SetGameOverActive(true);
                yield return new WaitForSeconds(timeDelayAfterGameOverMessage);
                _startMenu = true;
            }
            else
            {
                AudioManager.Instance.Play(AudioName.ArthurDeath, arthur.transform.position);
                gameManager.StateToMoveTo = this;
                yield return new WaitForSeconds(timeDelayAfterDeathTracker);
                _bossStage = true;
            }
            
        }

        private IEnumerator WinRound()
        {
            PlayerControls.Disable();
            arthur.transform.localScale = Vector3.one;
            _soundtrackStage = AudioManager.Instance.Play(AudioName.WinScene, arthur.WorldDataController.Position);
            yield return new WaitForSeconds(timeForPlayerToReachGround);
            arthur.transform.DOMove(arthurEndPoint.position, timeToEndPositionArthur).SetEase(Ease.Linear);
            princess.transform.DOMove(princessEndPoint.position, timeToEndPositionPrincess).SetEase(Ease.Linear);
            princessAnimator.SetBool("Move", true);
            yield return new WaitForSeconds(timeToEndPositionPrincess);
            princessAnimator.SetBool("Move", false);
            UiManager.Instance.SetBlackScreenActive(true);
            arthur.WorldDataController.SetForm(1);
            arthur.ColliderController.ChangeSprite(arthur.WorldDataController.Form, "Idle");
            arthur.AnimationController.PlayAnimationTrigger(arthur.WorldDataController.Form,"Win");
            yield return new WaitForSeconds(arthurTransformIntoKnightTime);
            UiManager.Instance.SetBlackScreenActive(false);
            princessAnimator.SetBool("Kiss", true);
            UiManager.Instance.SetCongratulationsActive(true);
            UiManager.Instance.SetStoryHappyEndingActive(true);
            yield return new WaitForSeconds(timeOfHappyEndingSubs);
            UiManager.Instance.SetCongratulationsActive(false);
            UiManager.Instance.SetStoryHappyEndingActive(false);
            TimeUpdater.ResetTimerAndRemoveUI();
            UiManager.Instance.SetLife1Active(false);
            UiManager.Instance.SetLife2Active(false);
            UiManager.Instance.SetTongueActive(false);
            UiManager.Instance.SetSpearActive(false);
            UiManager.Instance.SetWeaponBoxActive(false);
            UiManager.Instance.SetEndSubActive(true);
            yield return new WaitForSeconds(timeOfEndSubs);
            UiManager.Instance.SetEndSubActive(false);
            UiManager.Instance.SetBlackScreenActive(true);
            princessAnimator.SetBool("Kiss", false);
            UiManager.Instance.SetReturnToStartpointActive(true);
            yield return new WaitForSeconds(timeOfReturnToStartpoint);
            _startMenu = true;
        }

        private IEnumerator IllusionRound()
        {
            PlayerControls.Disable();
            yield return new WaitForSeconds(timeForPlayerToReachGround);
            _soundtrackStage = AudioManager.Instance.Play(AudioName.IllusionScene, arthur.WorldDataController.Position);
            UiManager.Instance.SetBlackScreenActive(true);
            UiManager.Instance.SetTrapTextActive(true);
            yield return new WaitForSeconds(_soundtrackStage.audioSource.clip.length);
            _endRound1 = true;
        }

        public void ExitState()
        {
            _soundtrackStage.StopSound();
            TimeUpdater.ResetTimerAndRemoveUI();
            UiManager.Instance.SetGameOverActive(false);
            UiManager.Instance.SetLife1Active(false);
            UiManager.Instance.SetLife2Active(false);
            UiManager.Instance.SetTongueActive(false);
            UiManager.Instance.SetSpearActive(false);
            UiManager.Instance.SetWeaponBoxActive(false);
            UiManager.Instance.SetTrapTextActive(false);
            UiManager.Instance.SetReturnToStartpointActive(false);
            UiManager.Instance.SetBlackScreenActive(false);
            arthur.gameObject.SetActive(false);
            princess.gameObject.SetActive(false);
            boss.Deactivate();
            boss.gameObject.SetActive(false);
        }
    }
}