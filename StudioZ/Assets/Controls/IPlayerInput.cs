using UnityEngine;

public interface IPlayerInput
{
    Vector2 MoveL { get; }
    Vector2 MoveR { get; }

    float GripLValue { get; }
    float GripRValue { get; }

    float GripDeadZone { get; }

    bool GripLPressed();

    bool GripRPressed();

    bool GripLNotPressed();

    bool GripRNotPressed();
    

    bool JugInput();

    bool PocketInput();

    bool CrimpInput();
}