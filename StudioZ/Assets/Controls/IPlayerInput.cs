using UnityEngine;

public interface IPlayerInput
{
    Vector2 MoveL { get; }
    Vector2 MoveR { get; }

    float GripLValue { get; }
    float GripRValue { get; }

    //bool JugInput();

    //bool PocketInput();

    //bool CrimpInput();
}