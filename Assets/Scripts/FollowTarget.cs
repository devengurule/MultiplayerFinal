using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    [SerializeField] private float trackSpeed;
    private GameObject target;
    private Vector2 velocity;

    private void FixedUpdate()
    {
        if (target == null) return;

        Vector2 targetPos = target.transform.position;

        Vector2 newPos = Vector2.SmoothDamp(transform.position, targetPos, ref velocity, trackSpeed);

        transform.position = new(newPos.x, newPos.y, transform.position.z);
    }

    public void SetTarget(GameObject target)
    {
        this.target = target;
    }
}
