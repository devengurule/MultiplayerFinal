using System;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerInfo : NetworkBehaviour
{
    public static Action<PlayerInfo, ulong> PlayerObjectSpawned;
    public static Action<PlayerInfo, ulong> PlayerObjectDestroyed;

    void Awake()
    {
        PlayerObjectSpawned?.Invoke(this, OwnerClientId);
    }

    public override void OnDestroy()
    {
        base.OnDestroy();

        PlayerObjectDestroyed?.Invoke(this, OwnerClientId);
    }

}
