using UnityEngine;

public class PlayerInputTesting : MonoBehaviour
{
    private IPlayerInput input;

    private void Awake()
    {
        input = GetComponent<IPlayerInput>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Action Buttons
        if (input.ButtonSouthPressed()) Debug.Log("A Pressed");
        if (input.ButtonEastPressed()) Debug.Log("B Pressed");
        if (input.ButtonNorthPressed()) Debug.Log("Y Pressed");
        if (input.ButtonWestPressed()) Debug.Log("X Pressed");

        // Menu Buttons
        if (input.ButtonStartPressed()) Debug.Log("Start Pressed");
        if (input.ButtonBackPressed()) Debug.Log("Back Pressed");

        // Stick Press
        if (input.StickLPressed()) Debug.Log("L Stick Pressed");
        if (input.StickRPressed()) Debug.Log("R Stick Pressed");

        // Dpad
        if (input.DPadUpPressed()) Debug.Log("Dpad up Pressed");
        if (input.DPadDownPressed()) Debug.Log("Dpad Down Pressed");
        if (input.DPadLeftPressed()) Debug.Log("Dpad left Pressed");
        if (input.DPadRightPressed()) Debug.Log("Dpad right Pressed");

        // Vibration
        if (input.ButtonSouthPressed()) StartCoroutine(input.ActivateVibration());
    }
}
