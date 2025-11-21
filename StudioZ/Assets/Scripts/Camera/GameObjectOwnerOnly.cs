using Unity.Netcode;
using UnityEngine;

public class GameObjectOwnerOnly : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (!IsOwner) gameObject.SetActive(false);
    }
}
