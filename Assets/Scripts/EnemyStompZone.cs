using UnityEngine;

public class EnemyStompZone : MonoBehaviour
{
    public Enemy enemy;


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerStompZone"))
        {
            Debug.Log("Stomp detected!");
            Player player = other.GetComponentInParent<Player>();
            if (player != null)
            {
                enemy.Stomped();
                player.Bounce();
            }
        }
    }
}
