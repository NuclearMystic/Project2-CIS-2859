using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Boss : MonoBehaviour
{
    public static Boss Instance { get; private set; }

    [SerializeField]
    private GameObject bossObject;

    private float bossHP = 3f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private float enemiesStomped = 0;

    public void AddEnemyStomped()
    {
        enemiesStomped++;
        Debug.Log("Stomped enemy added! " + enemiesStomped + " Total enemies stomped!");
    }

    public void Update()
    {
        if(enemiesStomped == 3)
        {
            bossObject.SetActive(true);
        }
    }

    public void BossDefeated()
    {
        // trigger win scenario
    }
}
