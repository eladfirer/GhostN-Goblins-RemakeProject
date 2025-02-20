using UI;

public class TimeUpdater
{
    private static int _minutes = 2; 
    private static int _seconds = 0;

    
    public static void SetTimeStart()
    {
        UiManager.Instance.SetMinutesNumber(2);
        UiManager.Instance.SetSecondsPart1Number(0);
        UiManager.Instance.SetSecondsPart2Number(0);
        UiManager.Instance.SetMinutesNumberActive(true);
        UiManager.Instance.SetSecondsPart1NumberActive(true);
        UiManager.Instance.SetSecondsPart2NumberActive(true);
        UiManager.Instance.SetTimeActive(true); 
        UiManager.Instance.SetTimeSeparateActive(true);
    }

    public static void ResetTimerAndRemoveUI()
    {
        UiManager.Instance.SetMinutesNumberActive(false);
        UiManager.Instance.SetSecondsPart1NumberActive(false);
        UiManager.Instance.SetSecondsPart2NumberActive(false);
        UiManager.Instance.SetTimeActive(false); 
        UiManager.Instance.SetTimeSeparateActive(false);
        _minutes = 2;
        _seconds = 0;
    }
    public static int TimeUpdate()
    {
        
        if (_seconds == 0)
        {
            if (_minutes > 0)
            {
                _minutes--;
                _seconds = 59;
            }
            else
            {
                UiManager.Instance.SetMinutesNumber(0);
                UiManager.Instance.SetSecondsPart1Number(0);
                UiManager.Instance.SetSecondsPart2Number(0);
                return 0;
            }
        }
        else
        {
            _seconds--;
        }
        
        UpdateTimeUI();

        return _minutes * 60 + _seconds;
    }

    private static void UpdateTimeUI()
    {
        UiManager.Instance.SetMinutesNumber(_minutes);
        
        int tens = _seconds / 10; 
        int ones = _seconds % 10; 
        UiManager.Instance.SetSecondsPart1Number(tens);
        UiManager.Instance.SetSecondsPart2Number(ones);
    }
}