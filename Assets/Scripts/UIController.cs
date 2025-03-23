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

    public GameObject endScreen;

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

    public void EnableEndScreen()
    {

        endScreen.SetActive(true);
        FindAnyObjectByType<SideScrollerController>().enabled = false;  
        // Include any logic to stop game in script on endScreen gameobjects ondisable method
    }
}
