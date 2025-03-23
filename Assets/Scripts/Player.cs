using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private GameObject stompZone;
    [SerializeField] private float minStompVelocity = -1.0f;

    public float health = 10f;
    public float knockbackForce = 5f;
    public float hitAnimationDuration = 0.2f;

    private Animator animator;
    private Rigidbody2D rb;

    private int coinTotal = 0;

    [SerializeField]
    private AudioSource playerSFX;
    public AudioClip coinSFX;

    public GameObject goldenPlatform;

    public bool playerDead = false;

    private void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        bool isFalling = rb.velocity.y < minStompVelocity && playerDead == false;
        if (stompZone.activeSelf != isFalling)
            stompZone.SetActive(isFalling);
    }

    public void TakeDamage(float amount, Vector2 hitSource)
    {
        health -= amount;
        UIController.Instance.UpdateHealthUI(health);
        Debug.Log($"Player took {amount} damage! Health left: {health}");

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

    private void ApplyKnockback(Vector2 hitSource)
    {
        if (rb != null)
        {
            Vector2 knockbackDirection = (transform.position - (Vector3)hitSource).normalized;
            rb.velocity = Vector2.zero;
            rb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Coin"))
        {
            Destroy(collision.gameObject);
            coinTotal++;
            UIController.Instance.UpdateCoinUI(coinTotal);
            playerSFX.PlayOneShot(coinSFX);
            if(coinTotal >= 10)
            {
                goldenPlatform.SetActive(true);
            }
        }
    }

    public void Bounce()
    {
        if (rb != null)
        {
            rb.velocity = new Vector2(rb.velocity.x, 15f); // tune bounce force
        }
    }

    private void Die()
    {
        playerDead = true;

        FindAnyObjectByType<SideScrollerController>().enabled = false;
        GetComponent<Collider2D>().enabled = false;

        rb.velocity = Vector2.zero;
        rb.AddForce(Vector2.up * 10f, ForceMode2D.Impulse); 

        transform.rotation = Quaternion.Euler(0f, 0f, 180f);

        Debug.Log("Player has died!");
    }
}
