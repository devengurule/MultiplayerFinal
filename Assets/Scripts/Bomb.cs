using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Bomb : NetworkBehaviour
{
    [field: SerializeField] public int damage { get; private set; }
    [field: SerializeField] public float knockbackForce { get; private set; }
    [field: SerializeField] public float aoeRadius { get; private set; }

    private Controls controls;
    private void Awake()
    {
        controls = new Controls();
    }

    public override void OnNetworkSpawn()
    {
        controls.PlayerMap.Enable();

        controls.PlayerMap.ActivateBombs.performed += Explode;
    }

    public override void OnNetworkDespawn()
    {
        controls.PlayerMap.ActivateBombs.performed -= Explode;
        
        controls.PlayerMap.Disable();
    }

    private void Explode(InputAction.CallbackContext context)
    {
        if (!IsServer) return;
        Collider2D[] collider = Physics2D.OverlapCircleAll(transform.position, aoeRadius);

        foreach (Collider2D col in collider)
        {
            Vector2 knockBackDirection = (col.gameObject.transform.position - transform.position).normalized;
            col.gameObject.GetComponent<PlayerMovement>().ApplyKnockbackRpc(knockBackDirection, knockbackForce);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, aoeRadius);
    }
}
