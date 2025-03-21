using UnityEngine;
using Cinemachine;

public class CameraFollow2D : MonoBehaviour
{
    public Transform player;
    private CinemachineVirtualCamera virtualCamera;
    private CinemachineFramingTransposer framingTransposer;

    public float lookAheadRight = 0.3f; 
    public float lookAheadLeft = 0.7f; 
    public float returnToCenter = 0.5f; 

    public float lookAheadSpeed = 5f;
    private Vector3 previousPlayerPosition;
    private float targetScreenX;

    void Start()
    {
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
        framingTransposer = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();

        if (player != null)
            previousPlayerPosition = player.position;

        targetScreenX = returnToCenter; 
    }

    void Update()
    {
        if (player == null) return;

        Vector3 playerMovement = player.position - previousPlayerPosition;

        if (playerMovement.x > 0.1f)
        {
            targetScreenX = lookAheadRight;
        }
        else if (playerMovement.x < -0.1f)
        {
            targetScreenX = lookAheadLeft; 
        }
        else
        {
            targetScreenX = returnToCenter;
        }

        framingTransposer.m_ScreenX = Mathf.Lerp(framingTransposer.m_ScreenX, targetScreenX, Time.deltaTime * lookAheadSpeed);

        previousPlayerPosition = player.position;
    }
}
