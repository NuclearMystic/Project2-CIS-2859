using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    [Header("Enemy Info")]
    public string enemyName = "Enemy";
    public float health = 10f;
    public float damageAmount = 1f;

    [Header("Detection")]
    public float detectionRadius = 5f;
    protected Transform player;
    protected bool playerDetected = false;

    [Header("Knockback")]
    public float knockbackForce = 5f;

    protected Animator animator;
    protected Rigidbody2D rb;
    private Collider2D enemyCollider;

    [Header("Damage Cooldown")]
    public float damageCooldown = 0.5f;

    private float lastDamageTime = -Mathf.Infinity;


    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        enemyCollider = GetComponent<Collider2D>();
    }

    private void Update()
    {
        EnemyUpdate(); 
    }


    protected void EnemyUpdate()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);
        playerDetected = distance <= detectionRadius;
    }

    public virtual void TakeDamage(float amount, Vector2 hitSource)
    {
        health -= amount;
        Debug.Log($"{enemyName} took {amount} damage. Remaining: {health}");

        if (animator != null)
        {
            animator.SetTrigger("Hit");
        }

        ApplyKnockback(hitSource);

        if (health <= 0)
        {
            Die();
        }
    }

    protected void ApplyKnockback(Vector2 hitSource)
    {
        if (rb != null)
        {
            Vector2 direction = (transform.position - (Vector3)hitSource).normalized;
            rb.velocity = Vector2.zero;
            rb.AddForce(direction * knockbackForce, ForceMode2D.Impulse);
        }
    }

    protected virtual void Die()
    {
        Debug.Log($"{enemyName} has been defeated.");
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            Player playerScript = collision.collider.GetComponent<Player>();
            if (playerScript != null)
            {
                playerScript.TakeDamage(damageAmount, transform.position);

                ApplyKnockback(collision.transform.position);
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            Player playerScript = collision.collider.GetComponent<Player>();
            if (playerScript != null)
            {
                if (Time.time - lastDamageTime >= damageCooldown)
                {
                    lastDamageTime = Time.time;

                    playerScript.TakeDamage(damageAmount, transform.position);

                    ApplyKnockback(collision.transform.position);
                }
            }
        }
    }

    protected void FlipSprite(Vector2 moveDirection)
    {
        if (moveDirection.x == 0) return; 

        Vector3 localScale = transform.localScale;
        localScale.x = Mathf.Abs(localScale.x) * (moveDirection.x > 0 ? 1 : -1);
        transform.localScale = localScale;
    }

}
