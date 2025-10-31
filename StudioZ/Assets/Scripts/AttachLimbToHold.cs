using UnityEngine;

public class AttachLimbToHold : MonoBehaviour
{
    private HingeJoint2D hinge;

    //public Rigidbody2D testRigidBody;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.attachedRigidbody != null)
        {
            GrabHold(collision.attachedRigidbody);
        }
    }

    public void GrabHold(Rigidbody2D rb)
    {
        hinge = GetComponent<HingeJoint2D>();
        hinge.connectedBody = rb;
    }
}
