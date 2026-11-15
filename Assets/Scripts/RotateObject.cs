using UnityEngine;

public class RotateObject : MonoBehaviour
{
    [SerializeField] private float rotateSpeed;

    private void FixedUpdate()
    {
        transform.Rotate(0, 0, rotateSpeed);
    }
}
