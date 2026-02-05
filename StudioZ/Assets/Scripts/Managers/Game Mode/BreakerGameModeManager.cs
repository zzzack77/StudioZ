using System.Collections;
using UnityEngine;

public class BreakerGameModeManager : GameModeBase
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        SetLevelIndex(13);
       base.Start(); 
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected override void CheckWinCondition()
    {
        base.CheckWinCondition();

        // Check to see if all players are dead
        foreach (var player in GameManager.Instance.playerGameObjects)
        {

        }
    }
}
