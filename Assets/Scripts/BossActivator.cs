using UnityEngine;

public class BossActivator : MonoBehaviour
{
    public static BossActivator Instance { get; private set; }

    [SerializeField] private GameObject bossObject;
    [SerializeField] private int enemiesRequiredToActivate = 3;

    private int enemiesStomped = 0;
    private bool bossActivated = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void AddEnemyStomped()
    {
        if (bossActivated) return;

        enemiesStomped++;
        Debug.Log($"Enemy stomped! {enemiesStomped} / {enemiesRequiredToActivate}");

        if (enemiesStomped >= enemiesRequiredToActivate)
        {
            bossObject.SetActive(true);
            MusicChanger.Instance.SwapToBossTrack();
            bossActivated = true;
            Debug.Log("Boss has been activated!");
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}
