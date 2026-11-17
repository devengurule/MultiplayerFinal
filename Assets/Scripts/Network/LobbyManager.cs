using System;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class LobbyManager : NetworkBehaviour
{
    public NetworkList<PlayerData> Players;
    public static event Action<LobbyManager> Spawned;

    private void Awake()
    {
        Players = new NetworkList<PlayerData>();
    }

    public override void OnNetworkSpawn()
    {
        Spawned?.Invoke(this);
        // Only the server needs to know when players connect and disconnect
        // This prevents clients from creating their own list of connected players
        if (IsServer)
        {
            // Is called anytime a player connects
            NetworkManager.OnClientConnectedCallback += PlayerConnected;
            // Is called anytime a player disconnects
            NetworkManager.OnClientDisconnectCallback += PlayerDisconnected;

            // Register players incase callbacks happen before we are subscribed
            foreach (ulong id in NetworkManager.ConnectedClientsIds)
            {
                PlayerConnected(id);
            }
        }
    }

    public override void OnNetworkDespawn()
    {
        if (IsServer)
        {
            NetworkManager.OnClientConnectedCallback -= PlayerConnected;
            NetworkManager.OnClientDisconnectCallback -= PlayerDisconnected;
        }
    }

    private void PlayerConnected(ulong id)
    {
        // Check for an existing entry to prevent duplicates
        foreach (PlayerData player in Players)
        {
            if (player.ClientId == id) return;
        }
        // Add player to players list
        PlayerData newPlayer = new PlayerData
        {
            ClientId = id,
            Username = new FixedString64Bytes($"Player {id}"),
            PlayerColor = Color.white
        };

        Players.Add(newPlayer);
    }

    private void PlayerDisconnected(ulong id)
    {
        // Deletes client from players list
        // Loop backwards to catch all possible entries without shifting the list
        for (int i = Players.Count - 1; i >= 0; i--)
        {
            if (Players[i].ClientId == id) Players.RemoveAt(i);
        }
    }
}
