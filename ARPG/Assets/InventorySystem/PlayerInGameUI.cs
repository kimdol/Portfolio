using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInGameUI : MonoBehaviour
{
    public StatsObject statsObject;

    public Text levelText;
    public Image healthSlider;
    public Image manaSlider;

    // Start is called before the first frame update
    void Start()
    {
        levelText.text = statsObject.level.ToString("n0");

        healthSlider.fillAmount = statsObject.HealthPercentage;
        manaSlider.fillAmount = statsObject.ManaPercentage;
    }

    private void OnEnable()
    {
        statsObject.OnChangedStats += OnChangedStats;
    }

    private void OnDisable()
    {
        statsObject.OnChangedStats -= OnChangedStats;
    }

    private void OnChangedStats(StatsObject statsObject)
    {
        healthSlider.fillAmount = statsObject.HealthPercentage;
        manaSlider.fillAmount = statsObject.ManaPercentage;
    }
}
