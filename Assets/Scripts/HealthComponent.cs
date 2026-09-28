using UnityEngine;

public class HealthComponent : MonoBehaviour
{
    [SerializeField]
    private float health;
    // THIS IS A VERY BAD IDEA AND IT MEANS I'M HARDCODING BUT I DO HAVE TIME :SOB:
    [SerializeField] private PlayerController playerController;
    [SerializeField] private HealthBarPlayer playerHealthBar;
    public float Health { get { return health; } }

    public void TakeDamage(float damage)
    {
        if (!playerController.IsDodging)
        {
            health -= damage;
            playerHealthBar.SetHealth((int)health);
        }
    }
}
