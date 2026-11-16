using System;
using Unity.Netcode;
using UnityEngine;

public class PlayerInfo : NetworkBehaviour
{
    public static Action<PlayerInfo, ulong> PlayerObjectSpawned;

    private void Awake()
    {
        PlayerObjectSpawned?.Invoke(this, OwnerClientId);
    }
}
