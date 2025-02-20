using System.Collections;
using System.Collections.Generic;
using Audio;
using Camera;
using DG.Tweening;
using GameStates;
using Interfaces;
using UnityEngine;

namespace UI
{
    public class CutSceneState: MonoBehaviour, IGameState
    {
        [Header("Movement Settings")]
        [SerializeField] private Vector3 targetPosition; 
        [SerializeField] private float moveDuration = 2f; 

        [Header("Flicker Settings")]
        [SerializeField] private SpriteRenderer spriteRenderer; 
        [SerializeField] private float flickerInterval = 0.1f; 
        
        private Vector3 _originalPosition; 
        private Coroutine _flickerCoroutine;
        [SerializeField] private float resetTimeDelay;
        [SerializeField] private float timeWaitForCamera;
        [SerializeField] private Transform characterStag1Position;
        [SerializeField] private Transform characterStageBossPosition;
        [SerializeField] private Transform characterTransform;
        [SerializeField] private SpriteRenderer characterSpriteRenderer;
        [SerializeField] private float delayTimeAtStartOfCutScene;
        private bool _goBackToGame;
        public PlayerControls PlayerControls { get; set; }
        
        public void EnterState(GameManager gameManager)
        {
            _goBackToGame = false;
            _originalPosition = transform.position;
            if (gameManager.StateToMoveTo == GameStateFactory.GetState(GameState.Stage1))
            {
                characterTransform.position = characterStag1Position.position;
            }
            else
            {
                characterTransform.position = characterStageBossPosition.position;
            }
            StartMove();
        }
        public void StartMove()
        {
            // Start moving the object and flickering the sprites
            StartCoroutine(MoveToTarget());
        }

        private IEnumerator MoveToTarget()
        {
            CameraController.Instance.SetCutCamera();
            UiManager.Instance.SetBlackScreenActive(true);
            yield return new WaitForSeconds(timeWaitForCamera);
            UiManager.Instance.SetBlackScreenActive(false);
            _flickerCoroutine = StartCoroutine(FlickerSprites());
            yield return new WaitForSeconds(delayTimeAtStartOfCutScene);
            AudioManager.Instance.Play(AudioName.CutScene, transform.position);
            yield return transform.DOMove(targetPosition, moveDuration).SetEase(Ease.Linear).WaitForCompletion();

            
            transform.position = targetPosition;
            
            
            StopCoroutine(_flickerCoroutine);
            spriteRenderer.DOFade(1f, flickerInterval);
            characterSpriteRenderer.DOFade(1f, flickerInterval);

            yield return new WaitForSeconds(resetTimeDelay);
            _goBackToGame = true;
        }

        private IEnumerator FlickerSprites()
        {
            while (true)
            {
                spriteRenderer.DOFade(0, flickerInterval);
                characterSpriteRenderer.DOFade(0, flickerInterval);
                yield return new WaitForSeconds(flickerInterval);
                spriteRenderer.DOFade(1, flickerInterval);
                characterSpriteRenderer.DOFade(1, flickerInterval);
                yield return new WaitForSeconds(flickerInterval);
            }
        }


        public IGameState OnUpdate(GameManager gameManager)
        {
            if (_goBackToGame)
            {
                return gameManager.StateToMoveTo;
            }
            return this;
        }

        public void ExitState()
        {
            transform.position = _originalPosition;
        }
    }
}
