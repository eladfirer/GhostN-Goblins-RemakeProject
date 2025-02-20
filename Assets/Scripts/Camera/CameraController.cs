using DefaultNamespace;
using Unity.Cinemachine;
using UnityEngine;

namespace Camera
{
    public class CameraController: MonoSingleton<CameraController>
    {
        [SerializeField] private CinemachineCamera gameplayCamera;
        [SerializeField] private CinemachineCamera cutCamera;
        [SerializeField] private CinemachineCamera bossFightCamera;
        [SerializeField] private CinemachineCamera startMenuCamera;
        

        private void ActivateCamera(CinemachineCamera camera)
        {
            foreach (var cam in new[] { gameplayCamera, cutCamera, bossFightCamera, startMenuCamera })
            {
                cam.gameObject.SetActive(cam == camera);
            }
            
        }

        public void SetGameplayCamera() => ActivateCamera(gameplayCamera);
        public void SetCutCamera() => ActivateCamera(cutCamera);
        public void SetBossFightCamera() => ActivateCamera(bossFightCamera);
        public void SetStartMenuCamera() => ActivateCamera(startMenuCamera);
        
        
    }
}