using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
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
    private void OnEnable()
    {
        controls.PlayerMap.Enable();

        controls.PlayerMap.Move.performed += OnMove;
        controls.PlayerMap.Move.canceled += OnMoveCancelled;
    }

    private void OnDisable()
    {
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
        rb.linearVelocity = moveInput * moveSpeed;
    }
}
