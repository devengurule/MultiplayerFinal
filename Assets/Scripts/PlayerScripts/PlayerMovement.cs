using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : NetworkBehaviour
{
    [SerializeField] private float moveSpeed;
    private Controls controls;
    private Vector2 moveInput;
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        controls = new Controls();
    }
    private void FixedUpdate()
    {
        if (!IsOwner) return;

        rb.AddForce(moveInput * moveSpeed, ForceMode2D.Force);
    }

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;

        SetCameraTarget(gameObject);

        controls.PlayerMap.Enable();

        controls.PlayerMap.Move.performed += OnMove;
        controls.PlayerMap.Move.canceled += OnMoveCancelled;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner) return;

        controls.PlayerMap.Move.performed -= OnMove;
        controls.PlayerMap.Move.canceled -= OnMoveCancelled;

        controls.PlayerMap.Disable();
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }
    private void OnMoveCancelled(InputAction.CallbackContext context)
    {
        moveInput = Vector2.zero;
    }

    [Rpc(SendTo.Owner)]
    public void ApplyKnockbackRpc(Vector3 direction, float knockbackForce)
    {
        rb.AddForce(direction * knockbackForce, ForceMode2D.Impulse);
    }

    private void SetCameraTarget(GameObject target)
    {
        if (Camera.main.gameObject.GetComponent<FollowTarget>() != null)
        {
            Camera.main.gameObject.GetComponent<FollowTarget>().SetTarget(target);
        }
    }
}
