using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private GameObject attachedBoss;
    [SerializeField] private TextMeshProUGUI bossName;

    public Slider slider;

    private void Start()
    {
        slider.maxValue = attachedBoss.GetComponent<Boss>().MaxHealth;
        slider.value = slider.maxValue;

        bossName.text = attachedBoss.name;
    }

    public void SetHealth(int health)
    {
        print("YABADABADO");
        slider.value = health;
    }
}
