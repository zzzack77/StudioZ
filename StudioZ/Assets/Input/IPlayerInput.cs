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

    // Shoulder buttons
    bool BumperLPressed();

    bool BumperRPressed();

    // A, B, Y, X
    bool ButtonSouthPressed();

    bool ButtonEastPressed();

    bool ButtonNorthPressed();

    bool ButtonWestPressed();

    // Menu Buttons
    bool ButtonStartPressed();
    bool ButtonBackPressed();

    // Stick Presses
    bool StickLPressed();
    bool StickRPressed();

    // DPad
    bool DPadUpPressed();
    bool DPadDownPressed();
    bool DPadLeftPressed();
    bool DPadRightPressed();

    // Controller Vibration
    IEnumerator ActivateVibrationCoroutine();

    void ActivateVibration();
    void StopVibration();
}