using System.Collections;
using UnityEngine;

public interface IPlayerInput
{
    Vector2 MoveL { get; }
    Vector2 MoveR { get; }

    float GripLValue { get; }
    float GripRValue { get; }

    float GripDeadZone { get; }

    // Triggers
    bool GripLPressed();

    bool GripRPressed();

    // Shoulder buttons
    bool CrimpLPressed();

    bool CrimpRPressed();

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