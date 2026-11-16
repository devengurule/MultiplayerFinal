using System;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public struct PlayerData : INetworkSerializable, IEquatable<PlayerData>
{
    public ulong ClientId;
    public FixedString64Bytes Username;
    public Color PlayerColor;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref ClientId);
        serializer.SerializeValue(ref Username);
        serializer.SerializeValue(ref PlayerColor);
    }

    public bool Equals(PlayerData other)
    {
        return ClientId == other.ClientId &&
            Username.Equals(other.Username) &&
            PlayerColor == other.PlayerColor;
    }
}