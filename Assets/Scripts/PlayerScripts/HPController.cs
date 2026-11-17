using Unity.Netcode;
using UnityEngine;

public class HPController : NetworkBehaviour
{
    [SerializeField] private int MaxHp;

    private int hp;

    private void Awake()
    {
        hp = MaxHp;
    }

    [Rpc(SendTo.Owner)]
    public void TakeDamageRpc(int amount)
    {
        hp -= amount;
    }

    [Rpc(SendTo.Owner)]
    public void HealRpc(int amount)
    {
        hp += amount;
    }
}