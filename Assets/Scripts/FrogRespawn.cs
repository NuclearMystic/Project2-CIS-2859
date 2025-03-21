using System.Collections;
using UnityEngine;

public class FrogRespawn : MonoBehaviour
{
    public Transform respawnPoint; 
    public Transform startPoint;   
    public float launchForce = 5f;  
    public AudioClip splashSound;  
    public AudioClip jumpSound;    
    public Collider2D waterTrigger; 

    private Animator animator;
    private Rigidbody2D rb;
    private Collider2D frogCollider;
    private SideScrollerController playerController;
    private AudioSource audioSource;

    private void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        frogCollider = GetComponent<Collider2D>();
        playerController = GetComponent<SideScrollerController>(); 
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Water")) 
        {
            StartCoroutine(HandleFrogFall());
        }
    }

    private IEnumerator HandleFrogFall()
    {
        if (waterTrigger != null)
            waterTrigger.enabled = false;

        if (playerController != null)
            playerController.enabled = false;

        audioSource.PlayOneShot(splashSound);
        animator.SetTrigger("Disappear");
        yield return new WaitForSeconds(1f); 
        animator.speed = 1f;

        transform.position = respawnPoint.position;
        rb.velocity = Vector2.zero; 
        yield return new WaitForSeconds(0.1f);

        rb.velocity = new Vector2(0, launchForce);

        yield return new WaitForSeconds(0.3f); 
        if (playerController != null)
            playerController.enabled = true;

        yield return new WaitForSeconds(0.5f); 
        if (waterTrigger != null)
            waterTrigger.enabled = true;
    }

    public void PauseAnimator()
    {
        animator.speed = 0f;
    }

    public void ResumeAnimator()
    {
        animator.speed = 1f;
    }
}
