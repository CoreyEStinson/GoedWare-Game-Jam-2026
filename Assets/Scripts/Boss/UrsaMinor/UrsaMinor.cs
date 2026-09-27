using UnityEngine;

public class UrsaMinor : Boss
{
    [Header("Ursa Minor Attacks")]
    [SerializeField] private BossAttack chargeForth;
    [SerializeField] private BossAttack flurry;

    [SerializeField] private float stoppingDistance = 2f;

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
}
