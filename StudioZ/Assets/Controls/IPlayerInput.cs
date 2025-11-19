using UnityEngine;

public interface IPlayerInput
{
    Vector2 MoveL { get; }
    Vector2 MoveR { get; }

    float GripLValue { get; }
    float GripRValue { get; }

    float GripDeadZone { get; }

    // Triggers or click
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
}