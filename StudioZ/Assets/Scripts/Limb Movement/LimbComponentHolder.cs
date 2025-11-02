using UnityEngine;
using UnityEngine.U2D.IK;

public class LimbComponentHolder : MonoBehaviour
{
    // This class holds references to each part of the limb
    public GameObject upperLimb; // Bicep or thigh for example
    public GameObject lowerLimb; // Forearm or shin
    public GameObject endLimb; // hands or feet

    public GameObject limbSolver;
}
