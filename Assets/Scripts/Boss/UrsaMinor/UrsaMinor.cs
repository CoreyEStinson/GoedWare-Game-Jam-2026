using UnityEngine;
using System.Collections;

public class UrsaMinor : Boss
{
    [Header("UrsaMajor")]
    [SerializeField] private Transform ursaMajor;
    [Header("Ursa Minor Attacks")]
    [SerializeField] private BossAttack chargeForth;
    [SerializeField] private BossAttack flurry;

    [SerializeField] private float stoppingDistance = 2f;
    [SerializeField] private HealthBar healthBar;

    protected override void Start()
    {
        base.Start();

        attacks.Add(chargeForth);
        attacks.Add(flurry);
    }

    protected override void HandleIntro()
    {
        // Play intro animation or something

        ChangeState(BossState.Idle);
    }

    protected override void HandleMovement()
    {
        if (playerPos == null) return;

        float distance = Vector3.Distance(transform.position, playerPos.position);

        if (distance <= stoppingDistance)
        {
            ChangeState(BossState.Idle);
            return;
        }

        Vector3 direction = (playerPos.position - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;
    }

    protected override void Die()
    {
        base.Die();
    }

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);

        if (Random.Range(0, 5) == 0)
        {
            CancelCurrentAttack();

            float time = Vector2.Distance(transform.position, ursaMajor.position) / (moveSpeed * 2);
            StartCoroutine(LerpMoveToUrsa(ursaMajor.position, time));
        }

        // Update heathbar
        print("Setting healthbar");
        healthBar.SetHealth((int)currentHealth);
    }

    private IEnumerator LerpMoveToUrsa(Vector2 endPos, float time)
    {
        Vector2 startPos = transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < time)
        {
            float t = elapsedTime / time;

            transform.position = Vector2.Lerp(startPos, endPos, t);

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        transform.position = endPos;
        state = BossState.Idle;
    }
}
