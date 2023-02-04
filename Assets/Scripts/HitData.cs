using UnityEngine;

public struct HitData
{
    public static HitData Empty => new HitData();

    public Team Team;

    public int Damage;

    public ushort PlayerSourceId;
}
