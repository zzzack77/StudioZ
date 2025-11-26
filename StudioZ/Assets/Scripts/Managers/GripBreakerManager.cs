using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;

public class GripBreakerManager : NetworkBehaviour
{
    public static GripBreakerManager Instance;

    private List<HoldBreaker> holds = new List<HoldBreaker>();

    private void Awake()
    {
        Instance = this;
    }

    public int RegisterHold(HoldBreaker hold)
    {
        holds.Add(hold);
        return holds.Count - 1;
    }

    [ServerRpc(RequireOwnership = false)]
    public void RequestDisableHoldServerRpc(int holdIndex)
    {
        DisableHoldClientRpc(holdIndex);
    }

    [ClientRpc]
    private void DisableHoldClientRpc(int holdIndex)
    {
        holds[holdIndex].TriggerDisable();
    }
}
