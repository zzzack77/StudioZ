using System.Collections;
using UnityEngine;

public class BreakerGameModeManager : GameModeBase
{
    private void OnEnable()
    {
        PlayerAliveState.OnPlayerDead += CheckWinCondition;
    }

    private void OnDisable()
    {
        PlayerAliveState.OnPlayerDead -= CheckWinCondition;
    }
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

    protected override void CheckWinCondition(GameObject player)
    {
        base.CheckWinCondition(player);

        playerRank.Add(player);
        Debug.Log(player);
    }

    


}
