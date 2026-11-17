using UnityEngine;
public class RotateObject : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float rotationMultiplier = 70f; 

    private void Awake()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }
    }

    // This is just for fun; if this needs to be a more general script, feel free to revert it.
    private void FixedUpdate()
    {
        if (rb == null) return;
        float currentSpeed = rb.linearVelocity.magnitude;
        float rotationAmount = currentSpeed * rotationMultiplier * Time.fixedDeltaTime;
        transform.Rotate(0, 0, rotationAmount);
    }
}

