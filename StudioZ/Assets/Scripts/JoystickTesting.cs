using UnityEngine;
using UnityEngine.InputSystem;

public class JoystickTesting : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var gamepad = Gamepad.current;
        if (gamepad == null)
        {
            Debug.Log("No controller connected.");
            return;
        }

        // Example sticks
        Vector2 leftStick = gamepad.leftStick.ReadValue();
        Vector2 rightStick = gamepad.rightStick.ReadValue();

        // Example triggers and buttons
        float leftTrigger = gamepad.leftTrigger.ReadValue();
        float rightTrigger = gamepad.rightTrigger.ReadValue();
        float leftShoulder = gamepad.leftShoulder.ReadValue();

        if (gamepad.buttonSouth.wasPressedThisFrame) Debug.Log("A / Cross pressed");
        if (gamepad.buttonEast.wasPressedThisFrame) Debug.Log("B / Circle pressed");
        if (gamepad.buttonNorth.wasPressedThisFrame) Debug.Log("Y / Triangle pressed");
        if (gamepad.buttonWest.wasPressedThisFrame) Debug.Log("X / Square pressed");

        Debug.Log($"LeftStick: {leftStick}, RightStick: {rightStick}, LT: {leftTrigger:F2}, RT: {rightTrigger:F2}, LS: {leftShoulder:F2}");
    }
}

