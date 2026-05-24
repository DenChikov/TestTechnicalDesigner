using System;

[Serializable]
public struct PhaseEnum
{
    public enum Phase
    {
        Idle,
        Shooting,
        Hand,

    }

    public Phase Value;
}
