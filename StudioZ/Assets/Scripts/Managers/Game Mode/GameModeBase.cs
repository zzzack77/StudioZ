using System.Collections;
using UnityEngine;


public abstract class GameModeBase : MonoBehaviour
{
    protected float gracePeriodDuration = 10;
    protected float startCountDownDuration = 5;
    protected int levelIndex = 0;
    protected int playersAlive;
    public bool MatchInProgress {  get; protected set; }

    protected void SetMatchInProgress(bool value)
    {
        MatchInProgress = value;
    }
    protected virtual void Start()
    {
        InitialiseGame();
        StartMatch();
    }
    protected virtual void InitialiseGame()
    {
        // Game Setup
        SetMatchInProgress(false);
        SetPlayersAlive();
        //ClearLevel(); // Cant clear level yet as the first index of level prefabs is empty but needs to be filled
        //SpawnLevel();
        //FreezePlayerMovement(); 
        playersAlive = GameManager.Instance.playerGameObjects.Count;
    }
    public void StartMatch()
    {
        // Base start match functionality
        StartCoroutine(MatchStartCountdown());
    }

    public virtual void OnStartMatch()
    {
        // Extra functionality to add on children game modes
    }

    public void EndMatch()
    {
        // Base end match functionality
        SetMatchInProgress(false);
    }

    public virtual void OnEndMatch()
    {
        // Extra functionality to add on children game modes
    }

    public virtual void OnPlayerEliminated(ulong playerID, GameObject player)
    {
        // When a player gets eliminated
        // Remove them from the alive players list
    }

    private IEnumerator MatchStartCountdown()
    {
        FreezePlayerMovement();
        yield return new WaitForSeconds(gracePeriodDuration);
        // Show Timer countdown UI
        Debug.Log("Grace Period Over");
        yield return new WaitForSeconds(startCountDownDuration);
        Debug.Log("Match Start!");
        UnFreezePlayerMovement();
        SetMatchInProgress(true);

    }

    protected virtual void SpawnLevel()
    {
        GameManager.Instance.RequestLoadLevel(levelIndex);
    }

    protected virtual void ClearLevel()
    {
        GameManager.Instance.RequestLoadLevel(0);
    }

    protected virtual void FreezePlayerMovement()
    {
        NetworkPlayerMovement.OnSetCanMove?.Invoke(false);
    }

    protected virtual void UnFreezePlayerMovement()
    {
        NetworkPlayerMovement.OnSetCanMove?.Invoke(true);
    }

    protected void SetLevelIndex(int index)
    {
        levelIndex = index;
    }

    protected void SetPlayersAlive()
    {
        PlayerAliveState.OnSetAllAlive?.Invoke();
    }

    protected virtual void CheckWinCondition()
    {
        
    }
}
