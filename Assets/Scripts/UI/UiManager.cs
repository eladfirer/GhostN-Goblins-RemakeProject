
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UiManager: MonoSingleton<UiManager>
    {
        [SerializeField] private GameObject blackScreen;
        [SerializeField] private GameObject weaponBox;
        [SerializeField] private GameObject spear;
        [SerializeField] private GameObject tongue;
        [SerializeField] private GameObject life1;
        [SerializeField] private GameObject life2;
        [SerializeField] private GameObject logo;
        [SerializeField] private GameObject pressStart;
        [SerializeField] private GameObject credits;
        [SerializeField] private GameObject time;
        [SerializeField] private GameObject storyHappyEnding;
        [SerializeField] private GameObject gameOver;
        [SerializeField] private GameObject congratulations;
        [SerializeField] private GameObject endsub;
        [SerializeField] private GameObject returnToStartpoint;
        [SerializeField] private GameObject trapText;
        [SerializeField] private GameObject takeAkeyPart1;
        [SerializeField] private GameObject takeAkeyPart2;
        [SerializeField] private GameObject takeAkeyPart3;
        [SerializeField] private GameObject resolveYourBattle;
        [SerializeField] private GameObject minutesNumber;
        [SerializeField] private GameObject secondsNumberPart1;
        [SerializeField] private GameObject secondsNumberPart2;
        [SerializeField] private GameObject separateTime;
        [SerializeField] private List<Sprite> numberSprites;
        
        public void SetBlackScreenActive(bool isActive) => blackScreen?.SetActive(isActive);
        public void SetWeaponBoxActive(bool isActive) => weaponBox?.SetActive(isActive);
        
        public void SetTongueActive(bool isActive) => tongue?.SetActive(isActive);
        public void SetSpearActive(bool isActive) => spear?.SetActive(isActive);
        public void SetLife1Active(bool isActive) => life1?.SetActive(isActive);
        public void SetLife2Active(bool isActive) => life2?.SetActive(isActive);
        public void SetLogoActive(bool isActive) => logo?.SetActive(isActive);
        public void SetPressStartActive(bool isActive) => pressStart?.SetActive(isActive);
        public void SetCreditsActive(bool isActive) => credits?.SetActive(isActive);
        public void SetTimeActive(bool isActive) => time?.SetActive(isActive);
        public void SetStoryHappyEndingActive(bool isActive) => storyHappyEnding?.SetActive(isActive);
        public void SetGameOverActive(bool isActive) => gameOver?.SetActive(isActive);
        public void SetCongratulationsActive(bool isActive) => congratulations?.SetActive(isActive);
        public void SetEndSubActive(bool isActive) => endsub?.SetActive(isActive);
        public void SetReturnToStartpointActive(bool isActive) => returnToStartpoint?.SetActive(isActive);
        public void SetTrapTextActive(bool isActive) => trapText?.SetActive(isActive);
        public void SetTakeAKeyActive(bool isActive)
        {
            takeAkeyPart1?.SetActive(isActive);
            takeAkeyPart2?.SetActive(isActive);
            takeAkeyPart3?.SetActive(isActive);
        }
        public void SetResolveYourBattleActive(bool isActive) => resolveYourBattle?.SetActive(isActive);
        
        public void SetMinutesNumberActive(bool isActive) => minutesNumber?.SetActive(isActive);
        
        public void SetSecondsPart1NumberActive(bool isActive) => secondsNumberPart1?.SetActive(isActive);
        public void SetSecondsPart2NumberActive(bool isActive) => secondsNumberPart2?.SetActive(isActive);
        public void SetTimeSeparateActive(bool isActive) => separateTime?.SetActive(isActive);
        
        public void SetMinutesNumber(int number) => minutesNumber.GetComponent<Image>().sprite = numberSprites[number];
        public void SetSecondsPart1Number(int number) => secondsNumberPart1.GetComponent<Image>().sprite = numberSprites[number];
        public void SetSecondsPart2Number(int number) => secondsNumberPart2.GetComponent<Image>().sprite = numberSprites[number];

        
        
    }
}