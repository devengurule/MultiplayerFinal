using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Bomb : NetworkBehaviour
{
    [field: SerializeField] public int damage { get; private set; }
    [field: SerializeField] public float knockbackForce { get; private set; }
    [field: SerializeField] public float aoeRadius { get; private set; }
    [field: SerializeField] public float timer { get; private set; } = 3f;

    Rigidbody2D rb;

    private Controls controls;
    private void Awake()
    {
        controls = new Controls();
        rb = GetComponent<Rigidbody2D>();
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer) { Invoke(nameof(Explode), timer); }
    }

    // InitializeAndThrow is the method used for making instances of bombs.
    public void InitializeAndThrow(Vector2 throwVelocity, int dmg = 1, float kb = 15f, float radius = 1f)
    {
        damage = dmg;
        knockbackForce = kb;
        aoeRadius = radius;

        if (rb != null)
        {
            rb.linearVelocity = throwVelocity;
        }
    }

    private void Explode()
    {
        if (!IsServer) return;
        Collider2D[] collider = Physics2D.OverlapCircleAll(transform.position, aoeRadius);

        foreach (Collider2D col in collider)
        {
            if (col.TryGetComponent<PlayerMovement>(out var movement) &&
                col.TryGetComponent<HPController>(out var hp))
            {
                Vector2 knockBackDirection = (col.transform.position - transform.position).normalized;
                movement.ApplyKnockbackRpc(knockBackDirection, knockbackForce);
                hp.TakeDamageRpc(damage);
            }
        }
        NetworkObject.Despawn(true);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, aoeRadius);
    }
}
