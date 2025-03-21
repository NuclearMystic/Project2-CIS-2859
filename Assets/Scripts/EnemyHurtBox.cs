using UnityEngine;

public class EnemyHurtbox : MonoBehaviour
{
    private Enemy enemy;

    private void Awake()
    {
        enemy = GetComponentInParent<Enemy>();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!enemy) return;

        if (other.CompareTag("Player"))
        {
            if (Time.time - enemy.lastDamageTime >= enemy.damageCooldown)
            {
                enemy.lastDamageTime = Time.time;

                Player player = other.GetComponent<Player>();
                if (player != null)
                {
                    player.TakeDamage(enemy.damageAmount, enemy.transform.position);
                    enemy.ApplyKnockback(other.transform.position);
                }
            }
        }
    }
}
