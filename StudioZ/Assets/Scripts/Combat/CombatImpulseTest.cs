using UnityEngine;

public class CombatImpulseTest : MonoBehaviour
{
    
    public Rigidbody BodyRB;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CheckImpulse();
        
    }

    public void ApplyImpulse(Vector3 forceDirection, float forceAmount)
    {
        BodyRB.AddForce(forceDirection * forceAmount, ForceMode.Impulse);
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
        Rigidbody otherRb = other.GetComponent<Rigidbody>();

        Vector3 LVOfAttacker = other.attachedRigidbody.linearVelocity;
        Vector3 AttackDirection = LVOfAttacker.normalized;

        if (otherRb != null)
        {
            Vector3 punch = AttackDirection * 50f;
            otherRb.AddForce(punch, ForceMode.Impulse);
            Debug.Log("HitPlayer");
        }
    }

    void ApplyPunch(
    Rigidbody target,
    Vector3 attackerHandVelocity,
    Vector3 targetVelocity,
    Vector3 hitNormal,
    float effectiveMass = 4f
)
    {
        // 1. Relative velocity along the line of impact
        float relativeVel = Vector3.Dot(attackerHandVelocity - targetVelocity, hitNormal);

        // If the fist isn't moving into the target, no impulse
        if (relativeVel <= 0f)
            return;

        // 2. Momentum (Impulse)
        float impulseMag = effectiveMass * relativeVel;

        // 3. Apply to target
        Vector3 impulse = hitNormal * impulseMag;
        target.AddForce(impulse, ForceMode.Impulse);
    }

}
