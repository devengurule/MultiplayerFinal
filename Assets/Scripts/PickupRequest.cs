using System;
using UnityEngine;

[System.Serializable]
public class PickupRequest
{
    public ushort BombID { get; private set; }
    public ulong PlayerID { get; private set; }
    public System.DateTime Timestamp { get; private set; }

    public PickupRequest(ushort bombID, ulong playerID)
    {
        BombID = bombID;
        PlayerID = playerID;
        Timestamp = DateTime.UtcNow;
    }
}
