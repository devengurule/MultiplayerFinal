using Unity.Netcode;
using UnityEngine;

public class ChangeColor : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        if (!IsClient) return;

        GetComponent<SpriteRenderer>().color = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
    }
}
