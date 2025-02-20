using System.Collections;
using Camera;
using Interfaces;
using UI;
using UnityEngine;

namespace GameStates
{
    public class StartScreenState: MonoBehaviour, IGameState
    {
        public PlayerControls PlayerControls { get; set; }
        [SerializeField] private int playerLifes;
        [SerializeField] private float timeWaitForCamera;

        public void EnterState(GameManager gameManager)
        {
            PlayerControls.Disable();
            gameManager.CurrentRound = 1;
            gameManager.CurrentPlayerLifes = playerLifes; 
            StartCoroutine(StartMove());
        }

        private IEnumerator StartMove()
        {
            UiManager.Instance.SetBlackScreenActive(true);
            CameraController.Instance.SetStartMenuCamera();
            yield return new WaitForSeconds(timeWaitForCamera);
            UiManager.Instance.SetBlackScreenActive(false);
            UiManager.Instance.SetLogoActive(true);
            UiManager.Instance.SetCreditsActive(true);
            UiManager.Instance.SetPressStartActive(true);
            PlayerControls.Enable();
        }

        public IGameState OnUpdate(GameManager gameManager)
        {
            if (PlayerControls.Arthur.Start.triggered)
            {
                gameManager.firstPlay = true;
                return GameStateFactory.GetState(GameState.Stage1);
            }

            return this;
        }

        public void ExitState()
        {
            UiManager.Instance.SetLogoActive(false);
            UiManager.Instance.SetCreditsActive(false);
            UiManager.Instance.SetPressStartActive(false);
        }

    }
}