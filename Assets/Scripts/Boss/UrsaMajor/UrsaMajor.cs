using UnityEngine;

public class UrsaMajor : Boss
{
    [Header("Ursa Major Attacks")] 
    [SerializeField] private BossAttack frontSwing;
    [SerializeField] private BossAttack roll;
    [SerializeField] private BossAttack jumpAndLand;

    [SerializeField] private float stoppingDistance = 2f;

    protected override void Start()
    {
        base.Start();

        attacks.Add(frontSwing);
        attacks.Add(roll);
        attacks.Add(jumpAndLand);
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