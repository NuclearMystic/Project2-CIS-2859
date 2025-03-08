using UnityEngine;

public class PlatformMover2D : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;
    public float speed = 2f;

    private Rigidbody2D rb;
    private Transform currentTarget;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentTarget = pointB;
    }

    void FixedUpdate()
    {
        if (pointA == null || pointB == null) return;

        Vector2 newPosition = Vector2.MoveTowards(rb.position, currentTarget.position, speed * Time.fixedDeltaTime);
        rb.MovePosition(newPosition);

        if (Vector2.Distance(rb.position, currentTarget.position) < 0.1f)
        {
            currentTarget = (currentTarget == pointA) ? pointB : pointA;
        }
    }
}
