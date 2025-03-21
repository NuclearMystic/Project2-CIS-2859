using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIController : MonoBehaviour
{
    public static UIController Instance { get; private set; }

    [SerializeField]
    private Slider hpSlider;
    [SerializeField]
    private TMP_Text coinText;

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

    public void UpdateHealthUI(float value)
    {
        hpSlider.value = value;
    }

    public void UpdateCoinUI(int coinTotal)
    {
        coinText.text = coinTotal.ToString();
    }
}
