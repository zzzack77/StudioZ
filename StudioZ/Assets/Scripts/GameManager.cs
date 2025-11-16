using NUnit.Framework;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public static GameManager instance;

    public List<GameObject> playerGameObjects = new List<GameObject>();
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else Destroy(gameObject);

    }

    public void RegisterPlayer(GameObject player)
    {
        if (!playerGameObjects.Contains(player))
        {
            playerGameObjects.Add(player);
        }
    }

    public void UnregisterPlayer(GameObject player)
    {
        if (playerGameObjects.Contains(player))
        {
            playerGameObjects.Remove(player);
        }
    }
}
