using System.Collections;
using UnityEngine;

public class Snake : Enemy
{
    [Header("Snake Patrol")]
    public Transform pointA;
    public Transform pointB;
    public float moveSpeed = 2f;
    public float idleDuration = 1f;
    public float patrolPointTolerance = 0.2f;

    private Transform targetPoint;
    private bool isIdling = false;
    private bool isChasing = false;

    [Header("Ground Alignment")]
    public Transform groundCheckTransform;
    public float groundCheckRadius = 0.1f;
    public LayerMask groundLayer;
    public float rotationSmoothness = 10f;


    private void Awake()
    {
        targetPoint = pointA;
    }

    protected override void Update()
    {
        EnemyUpdate(); 

        if (playerDetected)
        {
            isChasing = true;
            StopAllCoroutines(); 
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

        AlignWithGround();

    }

    private void Patrol()
    {
        Vector2 direction = (targetPoint.position - transform.position).normalized;
        direction.y = 0f;

        rb.velocity = new Vector2(direction.x * moveSpeed, rb.velocity.y);
        FlipSprite(direction);

        if (Vector2.Distance(transform.position, targetPoint.position) < patrolPointTolerance)
        {
            StartCoroutine(IdleBeforeTurnAround());
        }
    }

    private IEnumerator IdleBeforeTurnAround()
    {
        isIdling = true;

        yield return new WaitForSeconds(idleDuration);

        targetPoint = (targetPoint == pointA) ? pointB : pointA;
        isIdling = false;
    }

    private void ChasePlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        direction.y = 0f; 

        rb.velocity = new Vector2(direction.x * moveSpeed, rb.velocity.y);
        FlipSprite(direction);
    }

    private Transform GetClosestPatrolPoint()
    {
        float distToA = Vector2.Distance(transform.position, pointA.position);
        float distToB = Vector2.Distance(transform.position, pointB.position);
        return distToA < distToB ? pointA : pointB;
    }

    private void OnDisable()
    {
        if (rb != null)
            rb.velocity = Vector2.zero;
    }

    private void AlignWithGround()
    {
        RaycastHit2D hit = Physics2D.Raycast(groundCheckTransform.position, Vector2.down, groundCheckRadius + 0.2f, groundLayer);
        if (!hit) return;

        float angle = Mathf.Clamp(Mathf.Atan2(hit.normal.y, hit.normal.x) * Mathf.Rad2Deg - 90f, -30f, 30f);
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, angle), Time.deltaTime * rotationSmoothness);
    }

}
