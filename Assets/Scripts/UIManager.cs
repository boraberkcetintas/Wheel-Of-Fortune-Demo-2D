using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [SerializeField] private WheelOfFortune wheelOfFortune;
    [SerializeField] private Button ui_text_button_spin;
    [SerializeField] private Button ui_text_button_claim;
    [SerializeField] private Button ui_text_button_restart;
    [SerializeField] private TextMeshProUGUI ui_text_level_value;

    void Start()
    {
        ui_text_button_spin.onClick.AddListener(() =>
        {
            wheelOfFortune.Spin();
        });

        ui_text_button_claim.onClick.AddListener(() =>
        {
            wheelOfFortune.GameOver();
        });

        ui_text_button_restart.onClick.AddListener(() =>
        {
            SceneManager.LoadScene(0);
        });
    }
}

