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
    [SerializeField] private Image image_spinning_wheel;
    [SerializeField] private Sprite gold_sprite;
    [SerializeField] private Sprite silver_sprite;
    [HideInInspector] private Sprite bronze_sprite;

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

        bronze_sprite = image_spinning_wheel.sprite;
        ui_text_button_claim.interactable = false;
    }

    private void Update()
    {
        if (wheelOfFortune.level > 1)
        {
            ui_text_button_claim.interactable = true;
        }

        if (wheelOfFortune.level % 30 == 0)
        {
            image_spinning_wheel.sprite = gold_sprite;
        }
        else if (wheelOfFortune.level % 5 == 0)
        {
            image_spinning_wheel.sprite = silver_sprite;
        }
        else
        {
            image_spinning_wheel.sprite = bronze_sprite;
        }
    }
}

