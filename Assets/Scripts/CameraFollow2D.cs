using UnityEngine;
using Cinemachine;

public class CameraFollow2D : MonoBehaviour
{
    public Transform player; // Assign the player transform in the Inspector
    private CinemachineVirtualCamera virtualCamera;
    private CinemachineFramingTransposer framingTransposer;

    public float lookAheadRight = 0.3f; // Screen X value when looking right
    public float lookAheadLeft = 0.7f;  // Screen X value when looking left
    public float returnToCenter = 0.5f; // Default Screen X when stopped

    public float lookAheadSpeed = 5f; // How fast the lookahead adjusts
    private Vector3 previousPlayerPosition;
    private float targetScreenX;

    void Start()
    {
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
        framingTransposer = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();

        if (player != null)
            previousPlayerPosition = player.position;

        targetScreenX = returnToCenter; // Start centered
    }

    void Update()
    {
        if (player == null) return;

        Vector3 playerMovement = player.position - previousPlayerPosition;

        // Determine which direction the player is moving
        if (playerMovement.x > 0.1f)
        {
            targetScreenX = lookAheadRight; // Looking right
        }
        else if (playerMovement.x < -0.1f)
        {
            targetScreenX = lookAheadLeft; // Looking left
        }
        else
        {
            targetScreenX = returnToCenter; // Lerp back to center when idle
        }

        // Smoothly adjust the Screen X value
        framingTransposer.m_ScreenX = Mathf.Lerp(framingTransposer.m_ScreenX, targetScreenX, Time.deltaTime * lookAheadSpeed);

        previousPlayerPosition = player.position;
    }
}
