using System;
using UnityEngine;

public enum AliveState
{
    Alive,
    KnockedOut,
    Dead
}
public class PlayerAliveState : MonoBehaviour
{
    public AliveState aliveState;

    public static Action<GameObject> OnPlayerAlive;
    public static Action OnSetAllAlive;

    public static Action<GameObject> OnPlayerDead;
    public static Action OnSetAllDead;

    public static Action<GameObject> OnPlayerKnockedOut;
    public static Action OnSetAllKnockedOut;

    private void OnEnable()
    {
        OnPlayerAlive += SetPlayerAlive;
        OnSetAllAlive += SetAlive;

        OnPlayerDead += SetPlayerDead;
        OnSetAllDead += SetDead;

        OnPlayerKnockedOut += SetPlayerKnockedOut;
        OnSetAllKnockedOut += SetKnockedOut;
    }

    private void OnDisable()
    {
        OnPlayerAlive -= SetPlayerAlive;
        OnSetAllAlive -= SetAlive;

        OnPlayerDead -= SetPlayerDead;
        OnSetAllDead -= SetDead;

        OnPlayerKnockedOut -= SetPlayerKnockedOut;
        OnSetAllKnockedOut -= SetKnockedOut;
    }
    
    // Sets specific player to alive
    private void SetPlayerAlive(GameObject player)
    {
        if (player == this.gameObject)
        {
            aliveState = AliveState.Alive;
        }
    }

    // Set alive without the need of checking for specific player
    // Used when all players alive state needs to be changed
    private void SetAlive()
    {
        aliveState = AliveState.Alive;
    }

    // Sets specific player to dead
    private void SetPlayerDead(GameObject player)
    {
        if (player == this.gameObject)
        {
            aliveState = AliveState.Dead;
        }
    }

    // Set dead without the need of checking for specific player
    // Used when all players alive state needs to be changed
    private void SetDead()
    {
        aliveState = AliveState.Dead;
    }

    // Sets specific player to knocked out
    private void SetPlayerKnockedOut(GameObject player)
    {
        if (player == this.gameObject)
        {
            aliveState = AliveState.KnockedOut;
        }
    }

    // Set dead without the need of checking for specific player
    // Used when all players alive state needs to be changed
    private void SetKnockedOut()
    {
        aliveState = AliveState.KnockedOut;
    }

}
