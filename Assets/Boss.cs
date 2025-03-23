using UnityEngine;

public class Boss : Enemy
{
    [Header("Boss Settings")]
    [SerializeField] private int maxBossHP = 3;

    [Header("Movement")]
    public Transform pointA;
    public Transform pointB;
    public float moveSpeed = 2f;
    public float idleTime = 1f;
    public float pointRangeTolerance = 0.1f;

    private int currentHP;
    private bool isIdling = false;
    private bool isChasing = false;
    private Transform targetPoint;

    protected void Awake()
    {
        currentHP = maxBossHP;
        targetPoint = pointA;

        if (rb == null)
            rb = GetComponent<Rigidbody2D>();
    }

    protected override void Update()
    {
        EnemyUpdate();

        if (isDead) return;

        if (playerDetected)
        {
            if (!isChasing)
            {
                isChasing = true;
                StopAllCoroutines();
            }

            ChasePlayer();
        }
        else
        {
            if (isChasing)
            {
                isChasing = false;
                targetPoint = GetClosestPatrolPoint();
            }

            if (!isIdling)
                Patrol();
        }

        if (animator != null)
        {
            animator.SetFloat("speed", Mathf.Abs(rb.velocity.x));
        }
    }

    private void Patrol()
    {
        Vector2 direction = (targetPoint.position - transform.position).normalized;
        rb.velocity = new Vector2(direction.x * moveSpeed, rb.velocity.y);
        FlipSprite(direction);

        if (Vector2.Distance(transform.position, targetPoint.position) < pointRangeTolerance)
        {
            rb.velocity = Vector2.zero;
            StartCoroutine(IdleBeforeTurningAround());
        }
    }

    private void ChasePlayer()
    {
        if (player == null) return;

        Vector2 direction = (player.position - transform.position).normalized;
        rb.velocity = new Vector2(direction.x * moveSpeed, rb.velocity.y);
        FlipSprite(direction);
    }

    private System.Collections.IEnumerator IdleBeforeTurningAround()
    {
        isIdling = true;
        yield return new WaitForSeconds(idleTime);
        targetPoint = (targetPoint == pointA) ? pointB : pointA;
        isIdling = false;
    }

    private Transform GetClosestPatrolPoint()
    {
        return (Vector2.Distance(transform.position, pointA.position) < Vector2.Distance(transform.position, pointB.position))
            ? pointA
            : pointB;
    }

    public override void Stomped()
    {
        if (isDead) return;

        currentHP--;

        if (animator != null)
            animator.SetTrigger("Hit");

        if (currentHP <= 0)
        {
            isDead = true;
            MusicChanger.Instance.PlayEndMusic();
            UIController.Instance.EnableEndScreen();
            rb.bodyType = RigidbodyType2D.Dynamic;

            // Disable damage colliders
            foreach (Collider2D col in GetComponentsInChildren<Collider2D>())
            {
                col.enabled = false;
            }

            // Launch upward
            if (rb != null)
            {
                rb.velocity = Vector2.zero;
                rb.AddForce(Vector2.up * deathLaunchForce, ForceMode2D.Impulse);
                rb.gravityScale = 1f;
            }

            this.enabled = false;

            transform.rotation = Quaternion.Euler(0f, 0f, 180f);
            Debug.Log("Boss defeated!");
            return;
        }

        // Show feedback on non-lethal stomp
        rb.velocity = Vector2.zero;
        rb.AddForce(Vector2.up * deathLaunchForce, ForceMode2D.Impulse);
        Debug.Log($"Boss hit! Remaining HP: {currentHP}");
    }
}
