using UnityEngine;

public class HealthComponent : MonoBehaviour
{
    [SerializeField]
    private float health;

    public void TakeDamage(float damage)
    {
        health -= damage;

        if (health < 0)
        {
            //DIEEE
        }
    }
}
