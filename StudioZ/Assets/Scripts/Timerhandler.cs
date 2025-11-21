using System.Globalization;
using Unity.Netcode;
using UnityEngine;

public class TimerHandeler : NetworkBehaviour
{
    [SerializeField] public bool isTimerRunning = false;
    [SerializeField] public float timeElapsed;
    [SerializeField] public float totalTime;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (!IsOwner) enabled = false;
    }
    private void Update()
    {
        UpdateTimer();
    }
    private void UpdateTimer()
    {
        if (isTimerRunning)
        {
            timeElapsed += Time.deltaTime;
        }
        else if (totalTime != timeElapsed)
        {
            totalTime = timeElapsed;
        }
    }
}
