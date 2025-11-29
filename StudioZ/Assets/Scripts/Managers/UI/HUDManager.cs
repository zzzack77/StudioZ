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
    private Label time;

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
        bestTime.text = $"Best Time - {bestT.Minutes:D2}:{bestT.Seconds:D2}.{bestT.Milliseconds:D2}";

        time = root.Q<Label>("Time");
    }
    private void Update()
    {
        TimeSpan currentTime = TimeSpan.FromSeconds(timerHandeler.timeElapsed);
        time.text = $"{currentTime.Minutes:D2}:{currentTime.Seconds:D2}.{currentTime.Milliseconds / 10:D2}";
    }
}
