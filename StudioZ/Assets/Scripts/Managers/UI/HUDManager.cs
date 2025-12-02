using System;
using UnityEngine;
using UnityEngine.UIElements;

public class HUDManager : MonoBehaviour
{
    [SerializeField] private UILevelManager levelManager;
    [SerializeField] private TimerHandeler timerHandeler;
    private VisualElement root;

    private Label level;
    private Label BTD;
    private Label bestTime;
    private Label continuePopUp;
    private Label time;
    private Label timeDifference;

    private void OnEnable()
    {
        root = GetComponent<UIDocument>().rootVisualElement;

        level = root.Q<Label>("Level");
        level.text = "Level #" + GameManager.Instance.GetCurrentLevel();
        if (PlayerDataManager.Instance.GetSingleLevelTime(12) > 0)
        {
            BTD = root.Q<Label>("BTD");
            TimeSpan time = TimeSpan.FromSeconds(levelManager.BTDTime[GameManager.Instance.GetCurrentLevel()]);
            BTD.text = $"BTD! - {time.Minutes:D2}:{time.Seconds:D2}";
        }
        bestTime = root.Q<Label>("BestTime");
        TimeSpan bestT = TimeSpan.FromSeconds(GameManager.Instance.GetCurrentLevelBestTime());
        bestTime.text = $"Best Time - {bestT.Minutes:D2}:{bestT.Seconds:D2}.{bestT.Milliseconds / 10:D2}";

        continuePopUp = root.Q<Label>("Continue");
        continuePopUp.text = "";

        time = root.Q<Label>("Time");

        timeDifference = root.Q<Label>("TimeDifference");
        timeDifference.text = "";
    }
    private void Update()
    {
        TimeSpan currentTime = TimeSpan.FromSeconds(timerHandeler.timeElapsed);
        time.text = $"{currentTime.Minutes:D2}:{currentTime.Seconds:D2}.{currentTime.Milliseconds / 10:D2}";
    }

    public void OnFinish(float timeDiff, float bestTime)
    {
        continuePopUp = root.Q<Label>("Continue");
        continuePopUp.text = "Press ( A ) To Continue...";

        if (bestTime == 0) return;

        TimeSpan time = TimeSpan.FromSeconds(timeDiff);
        bool isNegative = timeDiff < 0;

        if (isNegative)
        {
            timeDifference.style.color = Color.green;
            time = time.Duration(); // absolute value of the timespan
        }
        else
        {
            timeDifference.style.color = Color.red;
        }
        if (time.TotalMinutes >= 1)
        {
            timeDifference.text = $"{(isNegative ? "-" : "+")}{time.Minutes:D2}:{time.Seconds:D2}.{time.Milliseconds / 10:D2}";
        }
        else
        {
            timeDifference.text = $"{(isNegative ? "-" : "+")}{time.Seconds:D2}.{time.Milliseconds / 10:D2}";
        }
    }

    public void ResetHUD()
    {
        if (root == null) return;
        level = root.Q<Label>("Level");
        level.text = "Level #" + GameManager.Instance.GetCurrentLevel();

        bestTime = root.Q<Label>("BestTime");
        TimeSpan bestT = TimeSpan.FromSeconds(GameManager.Instance.GetCurrentLevelBestTime());
        bestTime.text = $"Best Time - {bestT.Minutes:D2}:{bestT.Seconds:D2}.{bestT.Milliseconds / 10:D2}";

        timeDifference = root.Q<Label>("TimeDifference");
        timeDifference.text = "";

        continuePopUp = root.Q<Label>("Continue");
        continuePopUp.text = "";
    }
}
