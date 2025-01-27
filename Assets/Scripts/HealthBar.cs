using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthBar : MonoBehaviour
{
    public Slider HealthSlider;
    public TextMeshProUGUI HealthText;

    public void UpdateHealth(int currentHealth, int maxHealth)
    {
        HealthSlider.maxValue = maxHealth;
        HealthSlider.value = currentHealth;
        HealthText.text = $"{currentHealth}/{maxHealth}";
    }
}
