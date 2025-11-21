using UnityEngine;

public class CombatImpulseTest : MonoBehaviour
{
    
    public Rigidbody BodyRB;
    public Rigidbody HandRB;
    public Vector3 previousVelocity;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CheckImpulse();
        //ForceChecker();
    }

    public void ApplyImpulse(Vector3 forceDirection, float forceAmount)
    {
        BodyRB.AddForce(forceDirection * forceAmount, ForceMode.Impulse);
    }

    public void CheckImpulse()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Vector3 LVAttacker = BodyRB.GetComponent<Rigidbody>().linearVelocity;
            Vector3 AttackDirection = LVAttacker.normalized;
            ApplyImpulse(AttackDirection, 50f);
            
        }
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("TriggerEnter");
        Rigidbody otherRb = other.GetComponent<Rigidbody>();

        ////Vector3 LVOfopponet = other.attachedRigidbody.linearVelocity;
        ////Vector3 AttackDirection = LVOfAttacker.normalized;
        //Vector3 LVAttacker = BodyRB.GetComponent<Rigidbody>().linearVelocity;
        //Vector3 AttackDirection = LVAttacker.normalized;

        if (otherRb != null)
        {
            Vector3 punch = Vector3.right * 50f;
            otherRb.AddForce(punch, ForceMode.Impulse);
            Debug.Log("HitPlayer");
        }
    }

    //void OnCollisionEnter(Collision collision)
    //{
    //    Debug.Log("CollisionEnter");
    //    Rigidbody otherRb = collision.rigidbody;

    //    //Vector3 LVOfopponet = other.attachedRigidbody.linearVelocity;
    //    //Vector3 AttackDirection = LVOfAttacker.normalized;
    //    Vector3 LVAttacker = otherRb.GetComponent<Rigidbody>().linearVelocity;
    //    Vector3 AttackDirection = LVAttacker.normalized;

    //    if (otherRb != null)
    //    {
    //        Vector3 punch = AttackDirection * 50f;
    //        otherRb.AddForce(punch, ForceMode.Impulse);
    //        Debug.Log("Hit");
    //    }
    //}
    public void ForceChecker()
    {
        Vector3 acceleration = (BodyRB.linearVelocity - previousVelocity) / Time.fixedDeltaTime;
        Vector3 netForce = BodyRB.mass * acceleration;
        previousVelocity = BodyRB.linearVelocity;
        Debug.Log("Net Force on BodyRB: " + (netForce.x, netForce.y, netForce.z));
    }


}
