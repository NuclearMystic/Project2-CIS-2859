using UnityEngine;

public class SideScrollerController : MonoBehaviour
{
    [SerializeField] private float speed = 4.5f;
    [SerializeField] private float jumpForce = 12.0f;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float rotationSmoothness = 10f;

    private Rigidbody2D body;
    private Animator anim;
    public Transform groundCheckTransform;

    private void Start()
    {
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        bool grounded = isGrounded();

        anim.SetBool("isGrounded", grounded);
        anim.SetFloat("speed", Mathf.Abs(Input.GetAxis("Horizontal")));

        if (grounded && Input.GetButtonDown("Jump"))
            anim.SetTrigger("jump");

        Move();
        if (grounded) AlignWithGround();
    }

    private void Move()
    {
        float deltaX = Input.GetAxis("Horizontal") * speed;
        body.velocity = new Vector2(deltaX, body.velocity.y);

        if (deltaX != 0)
            transform.localScale = new Vector3(Mathf.Sign(deltaX), 1, 1);
    }

    public void ApplyJumpForce() =>
        body.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

    private bool isGrounded() =>
        Physics2D.OverlapCircle(groundCheckTransform.position, groundCheckRadius, groundLayer);

    private void AlignWithGround()
    {
        RaycastHit2D hit = Physics2D.Raycast(groundCheckTransform.position, Vector2.down, groundCheckRadius + 0.2f, groundLayer);
        if (!hit) return;

        float angle = Mathf.Clamp(Mathf.Atan2(hit.normal.y, hit.normal.x) * Mathf.Rad2Deg - 90f, -30f, 30f);
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, angle), Time.deltaTime * rotationSmoothness);
    }

    private void OnDrawGizmos()
    {
        if (!groundCheckTransform) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheckTransform.position, groundCheckRadius);

        Gizmos.color = Color.green;
        Gizmos.DrawRay(groundCheckTransform.position, Vector2.down * (groundCheckRadius + 0.1f));
    }
}
