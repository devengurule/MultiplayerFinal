
using System;
using Unity.Netcode;


public class InventoryManager : NetworkBehaviour
{
    private System.Collections.Generic.Dictionary<ulong, PlayerInventory> _inventories = new();

    // Invoked when the Server has sent an updated version of the client's inventory
    public Action<ushort[]> ReceivedUpdatedInventory;

    private void Awake()
    {
        NetworkManager.OnClientConnectedCallback += AddPlayer;
        NetworkManager.OnClientDisconnectCallback += RemovePlayer;
        PickupRequestHandler.PickupRequestApproved += OnPickupRequestApproved;
    }

    public override void OnDestroy()
    {
        base.OnDestroy();

        NetworkManager.OnClientConnectedCallback -= AddPlayer;
        NetworkManager.OnClientDisconnectCallback -= RemovePlayer;
        PickupRequestHandler.PickupRequestApproved -= OnPickupRequestApproved;
    }

    private void AddPlayer(ulong ownerID)
    {
        if (_inventories.ContainsKey(ownerID))
        {
            // Does nothing if the Inventory already exists
        }
        else
        {
            _inventories.Add(ownerID, new PlayerInventory(ownerID));
            SendInventory(ownerID);
        }
    }

    private void RemovePlayer(ulong ownerID)
    {
        _inventories.Remove(ownerID);
        SendInventory(ownerID);
    }

    public void Clear(ulong ownerID)
    {
        _inventories[ownerID].Clear();
        SendInventory(ownerID);
    }

    private void AddBomb(ulong ownerID, ushort bomb)
    {
        _inventories[ownerID].Add(bomb);
        SendInventory(ownerID);
    }

    private void RemoveBomb(ulong ownerID, ushort bomb)
    {
        _inventories[ownerID].Remove(bomb);
        SendInventory(ownerID);
    }

    public bool HasBomb(ulong ownerID, ushort bomb)
    {
        if (!_inventories.ContainsKey(ownerID)) return false;

        return _inventories[ownerID].Contains(bomb);
    }

    
    private void SendInventory(ulong receiverID)
    {
        SendInventoryRPC(_inventories[receiverID].AsArray(), RpcTarget.Single(receiverID, RpcTargetUse.Temp));
    }

    [Rpc(SendTo.SpecifiedInParams, AllowTargetOverride = true)]
    private void SendInventoryRPC(ushort[] bombs, RpcParams rpcParams = default)
    {
        ReceivedUpdatedInventory?.Invoke(bombs);
    }

    private void OnPickupRequestApproved(PickupRequest request)
    {
        _inventories[request.PlayerID].Add(request.BombID);
    }

    public bool IsFull(ulong id) 
    {
        return _inventories[id].IsFull;
    }
}
