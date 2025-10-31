using UnityEngine;

public class IKTargetClamp : MonoBehaviour
{
    [SerializeField] private Transform limbStartPoint; // The root of your IK chain
    private float maxDistance; // The arm's max reach

    private void Start()
    {
        // Calculates the length of the arm (from the start of the upper arm to the hand)
        maxDistance = Vector3.Distance(limbStartPoint.position, transform.position);
    }
    private void LateUpdate()
    {
        // Calculates the direction and distance using the start of the arm and the end
        Vector3 direction = transform.position - limbStartPoint.position;
        float distance = direction.magnitude;

        if (distance > maxDistance)
        {
            // Clamp the target to the max distance
            transform.position = limbStartPoint.position + direction.normalized * maxDistance;
        }

        // This ensures that the collider on the hand does not move away from the hand so the mouse will always be able to move it
    }
}
