using UnityEngine;

public class WaterBob : MonoBehaviour
{
    public float bobSpeed = 1.0f;  
    public float bobHeight = 0.1f; 

    private float startY;  

    void Start()
    {
        startY = transform.position.y; 
    }

    void Update()
    {      
        // using sin wave for smooth float like movement
        float newY = startY + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
}
