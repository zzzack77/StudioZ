using System.Collections;
using UnityEngine;

public interface IPlayerInput
{
    Vector2 StickL { get; }
    Vector2 StickR { get; }

    float TriggerLValue { get; }
    float TriggerRValue { get; }

    float GripDeadZone { get; }
    float TriggerDeadZone { get; }

    bool VibrationEnabled { get; set; }
    // Triggers
    bool TriggerLPressed();

    bool TriggerRPressed();

    bool TriggerLPressedThisFrame();

    bool TriggerRPressedThisFrame();
    
    bool TriggerLReleasedThisFrame();
    bool TriggerRReleasedThisFrame();

    // Shoulder buttons
    bool BumperLPressed();

    bool BumperRPressed();

    bool BumperLPressedThisFrame();

    bool BumperRPressedThisFrame();

    // A, B, Y, X
    bool ButtonSouthPressed();

    bool ButtonEastPressed();

    bool ButtonNorthPressed();

    bool ButtonWestPressed();

    bool ButtonSouthPressedThisFrame();

    bool ButtonEastPressedThisFrame();

    bool ButtonNorthPressedThisFrame();

    bool ButtonWestPressedThisFrame();

    // Menu Buttons
    bool ButtonStartPressed();
    bool ButtonBackPressed();

    bool ButtonStartPressedThisFrame();
    bool ButtonBackPressedThisFrame();

    // Stick Presses
    bool StickLPressed();
    bool StickRPressed();

    bool StickLPressedThisFrame();
    bool StickRPressedThisFrame();

    // DPad
    bool DPadUpPressed();
    bool DPadDownPressed();
    bool DPadLeftPressed();
    bool DPadRightPressed();

    bool DPadUpPressedThisFrame();
    bool DPadDownPressedThisFrame();
    bool DPadLeftPressedThisFrame();
    bool DPadRightPressedThisFrame();

    // Controller Vibration
    IEnumerator ActivateVibrationCoroutine();

    void ActivateVibration();
    void StopVibration();
}