using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{

    private BasicPlayerControls controls;
    private Vector2 moveInput;
    Rigidbody2D rb;

    public float moveSpeed = 5.0f;
    
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        controls = new BasicPlayerControls();

    }
    private void OnEnable()
    {
        controls.PlayerMap.Enable();

        controls.PlayerMap.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.PlayerMap.Move.canceled += ctx => moveInput = Vector2.zero;

    }

    private void OnDisable()
    {
        controls.PlayerMap.Disable();
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = moveInput * moveSpeed;
    }
}
