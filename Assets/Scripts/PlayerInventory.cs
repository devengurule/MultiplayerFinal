using UnityEngine;

public class PlayerInventory
{
    const int MAX_BOMBS = 3;
    System.Collections.Generic.List<ushort> bombs = new(MAX_BOMBS);

    public ulong playerID { get; private set; }

    public PlayerInventory(ulong playerID)
    {
        this.playerID = playerID;
    }

    public bool IsFull => bombs.Count >= MAX_BOMBS;

    public bool IsEmpty => bombs.Count == 0;

    public void Add(ushort bomb)
    {
        if (bombs.Count >= MAX_BOMBS) return;

        bombs.Add(bomb);
    }

    public void Remove(ushort bomb)
    {
        bombs.Remove(bomb);
    }

    public bool Contains(ushort bomb)
    {
        return bombs.Contains(bomb);
    }

    public void Clear()
    {
        bombs.Clear();
    }

    public ushort[] AsArray()
    {
        return bombs.ToArray();
    }
}
