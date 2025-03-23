using UnityEngine;

public class Dragonfly : Enemy
{
    [Header("Dragonfly Patrol")]
    public Transform pointA;
    public Transform pointB;
    public float speed = 3f;

    private Transform targetPoint;
    private bool isChasing = false;

    private void Awake()
    {
        targetPoint = pointA;
    }

    protected override void Update()
    {
        EnemyUpdate();

        if (playerDetected)
        {
            if (!isChasing)
            {
                isChasing = true; 
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
            Patrol();
        }
    }

    private void Patrol()
    {
        if (isChasing) return; 

        if (Vector2.Distance(transform.position, targetPoint.position) < 0.1f)
        {
            targetPoint = (targetPoint == pointA) ? pointB : pointA;
        }

        MoveTowards(targetPoint.position);
    }

    private void ChasePlayer()
    {
        MoveTowards(player.position);
    }

    private void MoveTowards(Vector2 target)
    {
        Vector2 direction = (target - (Vector2)transform.position).normalized;
        rb.velocity = direction * speed;

        FlipSprite(direction); 
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
        isDead = true;

        rb.bodyType = RigidbodyType2D.Dynamic; 

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

        transform.rotation = Quaternion.Euler(0f, 0f, 180f);

        BossActivator.Instance.AddEnemyStomped();

        this.enabled = false;
    }


    private void OnDisable()
    {
        rb.velocity = Vector2.zero;
    }
}
