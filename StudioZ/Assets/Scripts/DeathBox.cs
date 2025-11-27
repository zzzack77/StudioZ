using UnityEngine;

public class DeathBox : MonoBehaviour
{
    private void OnTriggerEnter(Collider collision)
    {
        NetworkPlayerMovement networkPlayerMovement = collision.gameObject.transform.parent.gameObject.GetComponent<NetworkPlayerMovement>();
        
        if (networkPlayerMovement != null)
        {
            networkPlayerMovement.SpawnPlayer();
        }
    }
}
