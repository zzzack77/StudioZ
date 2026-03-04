using UnityEngine;

public class Weapon : MonoBehaviour
{
    private Rigidbody rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

   
    public void AttachWeapon(Transform HoldPoint)
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.useGravity = false;
        rb.isKinematic = true;
        rb.detectCollisions = false;
        transform.SetPositionAndRotation(HoldPoint.position, transform.rotation); 
        
        transform.SetParent(HoldPoint, true);
        
    }
    public void DeattachWeapon()
    {
        Transform parentTransform = transform.parent; ;
        transform.SetParent(null, true);
        
        rb.isKinematic = false;
        rb.useGravity = true;
        rb.detectCollisions = true;
        
        rb.AddForce(parentTransform.right * (100f * rb.mass), ForceMode.Impulse);
        
    }
}
