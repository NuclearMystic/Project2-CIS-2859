using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public Transform upPosition;
    public Transform downPosition;
    public float moveSpeed = 2f;

    private Rigidbody2D rb;
    private bool playerOnPlatform = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        Vector3 targetPos = playerOnPlatform ? upPosition.position : downPosition.position;
        transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.fixedDeltaTime);
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerOnPlatform = true;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerOnPlatform = false;
        }
    }
}
