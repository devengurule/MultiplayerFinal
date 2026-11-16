using UnityEngine;
using Unity.Netcode;

// Tracks player objects
public class PlayerManager : MonoBehaviour
{
    [SerializeField] private System.Collections.Generic.Dictionary<ulong, PlayerInfo> players = new();

    private void Awake()
    {
        
    }

    private void OnPlayerSpawned(PlayerInfo player, ulong ownerID)
    {

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

    private void OnPlayerDespawned(PlayerInfo _, ulong ownerID)
    {
        if (players.ContainsKey(ownerID)) players.Remove(ownerID);
    }
}
