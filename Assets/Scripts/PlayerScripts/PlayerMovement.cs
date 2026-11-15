using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : NetworkBehaviour
{
    [SerializeField] private float moveSpeed = 0.5f;
    private Controls controls;
    private Vector2 moveInput;
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        controls = new Controls();
    }

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;

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

    private void FixedUpdate()
    {
        if (!IsOwner) return;

        rb.linearVelocity = moveInput * moveSpeed;
    }
}
