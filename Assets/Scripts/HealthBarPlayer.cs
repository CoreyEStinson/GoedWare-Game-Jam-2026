using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarPlayer : MonoBehaviour
{
    [SerializeField] private GameObject player;

    public Slider slider;

    private void Start()
    {
        slider.maxValue = player.GetComponent<HealthComponent>().Health;
        slider.value = slider.maxValue;
    }

    public void SetHealth(int health)
    {
        slider.value = health;
    }
}
