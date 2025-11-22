using Unity.VisualScripting;
using UnityEngine;

public class CombatImpulseTest : MonoBehaviour
{
    
    public Rigidbody BodyRB;
    private Rigidbody OpponentBodyRB;

    private Vector3 previousVelocity;
    private Vector2 forceDirection;

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
        //BodyRB.AddForce(forceDirection * forceAmount, ForceMode.Impulse);
    }

    public void CheckImpulse()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            
            ApplyImpulse(Vector3.right, 50f);
            
        }
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("TriggerEnter");
        OpponentBodyRB = other.GetComponent<Rigidbody>();

        if (OpponentBodyRB != null)
        {
            if(other.gameObject.CompareTag("Arm"))
            {
                Debug.Log("HitArm");
            }
            else
            {
                
                Debug.Log("HitPlayer");
                forceDirection = (other.transform.position - transform.position).normalized;
                OpponentBodyRB.AddForce(forceDirection * 30f, ForceMode.Impulse);
                Debug.Log("ForceApplied");
            }
           
        }
    }


















    //void OnTriggerEnter(Collider other)
    //{
    //    Debug.Log("TriggerEnter");
    //    OpponentBodyRB = other.GetComponent<Rigidbody>();


    //    ////Vector3 LVOfopponet = other.attachedRigidbody.linearVelocity;
    //    ////Vector3 AttackDirection = LVOfAttacker.normalized;
    //    //Vector3 LVAttacker = BodyRB.GetComponent<Rigidbody>().linearVelocity;
    //    //Vector3 AttackDirection = LVAttacker.normalized;

    //    if (OpponentBodyRB != null)
    //    {

    //        Vector3 punch = Vector3.right * 50f;
    //        OpponentBodyRB.AddForce(punch, ForceMode.Impulse);
    //        Debug.Log("HitPlayer");
    //    }
    //}

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
