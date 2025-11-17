using Unity.Netcode;
using UnityEngine;

public class PlayerCombatHandler : NetworkBehaviour
{
    private Rigidbody lHandRB;
    private Rigidbody RHandRB;
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (!IsOwner) enabled = false;
    }


}
