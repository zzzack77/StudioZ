using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;


public abstract class GameModeBase : MonoBehaviour
{
    private Dictionary<ulong, GameObject> playersInGame = new Dictionary<ulong, GameObject>(); 

    protected virtual void Start()
    {
        InitialiseGame();
    }
    protected virtual void InitialiseGame()
    {
        // Get the dictionary of players from the game manager
        InitialisePlayerDictionary();
    }
    public void StartMatch()
    {
        // Base start match functionality
    }

    public virtual void OnStartMatch()
    {
        // Extra functionality to add on children game modes
    }

    public void EndMatch()
    {
        // Base end match functionality
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

    protected void InitialisePlayerDictionary()
    {
        playersInGame.Clear();
        playersInGame = GameManager.Instance.playerGameObjects;
    }
}
