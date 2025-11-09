using UnityEngine;

public class AnguarDrives : MonoBehaviour
{
    public ConfigurableJoint joint;
    public float contractedAngle = 45f;
    public float relaxedAngle = 0f;
    public float muscleStrength = 100f;
    public float contractionForce;
    public float relaxationForce;
    public float damper = 10f;

    void Start()
    {
        if (joint == null)
        {
            joint = GetComponent<ConfigurableJoint>();
        }

        // Set initial state
        RelaxMuscle();
    }
    // Expose prepared JointDrive values as read-only properties for clarity and reuse
    public JointDrive ContractionDrive
    {
        get
        {
            return new JointDrive
            {
                positionSpring = contractionForce,
                positionDamper = damper,
                maximumForce = muscleStrength
            };
        }
    }

    public JointDrive RelaxationDrive
    {
        get
        {
            return new JointDrive
            {
                positionSpring = relaxationForce,
                positionDamper = damper,
                maximumForce = muscleStrength
            };
        }
    }

    // Example functions to simulate muscle contraction/relaxation
    public void ContractMuscle()
    {
        joint.angularYZDrive = ContractionDrive;
        joint.targetRotation = Quaternion.Euler(0, 0, contractedAngle);
    }

    public void RelaxMuscle()
    {
        joint.angularYZDrive = RelaxationDrive;
        joint.targetRotation = Quaternion.Euler(0, 0, relaxedAngle);
    }

    // Example: change state on key press (update method)
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ContractMuscle();
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            RelaxMuscle();
        }
    }
}
