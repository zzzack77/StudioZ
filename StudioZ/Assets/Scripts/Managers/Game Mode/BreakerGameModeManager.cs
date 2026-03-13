using System;
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

        // Adds the dead players to a list
        // The first player would be the 1st to die, so rank 0 would be last place etc
        playerRank.Add(player);

        ulong playerID = player.GetComponent<RegisterPlayer>().NetworkObject.OwnerClientId;

        Debug.Log(playerID);
    }
}
