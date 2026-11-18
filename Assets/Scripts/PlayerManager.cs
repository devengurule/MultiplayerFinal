using UnityEngine;
using Unity.Netcode;

// Tracks player objects
public class PlayerManager : NetworkBehaviour
{
    [SerializeField] private System.Collections.Generic.Dictionary<ulong, PlayerInfo> players = new();

    private void Awake()
    {
        PlayerInfo.PlayerObjectSpawned += OnPlayerSpawned;
        PlayerInfo.PlayerObjectDestroyed += OnPlayerDestroyed;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnPlayerDisconnect;
    }

    public override void OnDestroy()
    {
        base.OnDestroy();

        PlayerInfo.PlayerObjectSpawned -= OnPlayerSpawned;
        PlayerInfo.PlayerObjectDestroyed -= OnPlayerDestroyed;
        NetworkManager.Singleton.OnClientDisconnectCallback -= OnPlayerDisconnect;
    }

    private void OnPlayerSpawned(PlayerInfo player, ulong ownerID)
    {
        AddPlayer(player, ownerID);
    }

    private void AddPlayer(PlayerInfo player, ulong ownerID)
    {
        if (players.ContainsKey(ownerID))
        {
            if (players[ownerID] == null)
            {
                players.Add(ownerID, player);
            }
        }
        else
        {
            players.Add(ownerID, player);
        }
    }

    private void OnPlayerDestroyed(PlayerInfo _, ulong ownerID)
    {
        if (players.ContainsKey(ownerID)) players[ownerID] = null;
    }

    private void OnPlayerDisconnect(ulong ownerID)
    {
        if (players.ContainsKey(ownerID)) players.Remove(ownerID);
    }
}
