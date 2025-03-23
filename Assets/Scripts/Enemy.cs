using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    [Header("Enemy Info")]
    public string enemyName = "Enemy";
    public float health = 10f;
    public float damageAmount = 1f;

    [Header("Detection")]
    protected Transform player;
    protected bool playerDetected = false;

    [Header("Knockback")]
    public float knockbackForce = 5f;
    protected Rigidbody2D rb;
    protected Animator animator;

    [Header("Damage Cooldown")]
    public float damageCooldown = 0.5f;
    [HideInInspector] public float lastDamageTime = -Mathf.Infinity;

    [Header("Stomp Kill Settings")]
    public float deathLaunchForce = 5f;
    protected bool isDead = false;

    protected virtual void Start()
    {
        if(rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    protected virtual void Update()
    {
        EnemyUpdate();
    }


    protected virtual void EnemyUpdate()
    {
        if (player != null)
        {
            float dist = Vector2.Distance(transform.position, player.position);
            playerDetected = dist < 10f; 
        }
    }

    public virtual void TakeDamage(float amount, Vector2 hitSource)
    {
        health -= amount;
        Debug.Log($"{enemyName} took {amount} damage. Remaining health: {health}");

        if (animator != null)
            animator.SetTrigger("Hit");

        ApplyKnockback(hitSource);

        if (health <= 0f)
            Die();
    }

    public virtual void ApplyKnockback(Vector2 hitSource)
    {
        if (rb != null)
        {
            Vector2 dir = ((Vector2)transform.position - hitSource).normalized;
            rb.velocity = Vector2.zero;
            rb.AddForce(dir * knockbackForce, ForceMode2D.Impulse);
        }
    }

    protected virtual void Die()
    {
        Debug.Log($"{enemyName} has died.");
        Destroy(gameObject);
    }


    public virtual void PlayerEnteredDetectionZone(Transform playerTransform)
    {
        player = playerTransform;
        playerDetected = true;
    }

    public virtual void PlayerExitedDetectionZone()
    {
        playerDetected = false;
        player = null;
    }

    protected void FlipSprite(Vector2 moveDirection)
    {
        if (moveDirection.x == 0) return;

        Vector3 localScale = transform.localScale;
        localScale.x = Mathf.Abs(localScale.x) * (moveDirection.x > 0 ? 1 : -1);
        transform.localScale = localScale;
    }

    public virtual void Stomped()
    {
        if (isDead) return;

        isDead = true;

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

        transform.rotation = Quaternion.Euler(0f, 0f, 180f);

        BossActivator.Instance.AddEnemyStomped();

        this.enabled = false;
    }
}
