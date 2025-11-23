using UnityEngine;

public class BasicCombatLogic : MonoBehaviour
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
        // Commented this out as was throwing tones of errors because the the tag Arm...
        // Also remove uneccissary debug logs which spam the console before pushing to main please 
        // - Zack


        //Debug.Log("TriggerEnter");
        //OpponentBodyRB = other.GetComponent<Rigidbody>();

        //if (OpponentBodyRB != null)
        //{
        //    if (other.gameObject.CompareTag("Arm"))
        //    {
        //        Debug.Log("HitArm");
        //    }
        //    else
        //    {

        //        Debug.Log("HitPlayer");
        //        forceDirection = (other.transform.position - transform.position).normalized;
        //        OpponentBodyRB.AddForce(forceDirection * 30f, ForceMode.Impulse);
        //        Debug.Log("ForceApplied");
        //    }

        //}
    }
}

