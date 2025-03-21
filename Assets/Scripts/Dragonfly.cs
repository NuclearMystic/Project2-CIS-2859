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

    private void Update()
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

    private void OnDisable()
    {
        rb.velocity = Vector2.zero;
    }
}
