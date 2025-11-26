using UnityEngine;

public class ReserveRotation : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
    }
}
