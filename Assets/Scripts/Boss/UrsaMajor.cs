using UnityEngine;

public class UrsaMajor : Boss
{
    [Header("Ursa Major Attacks")] 
    [SerializeField] private BossAttack frontSwing;
    [SerializeField] private BossAttack roll;
    [SerializeField] private BossAttack jumpAndLand;

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

    protected override void Move()
    {
        if (playerPos == null) return;

        Vector3 direction = (playerPos.position - transform.position).normalized;

        transform.position += direction * moveSpeed * Time.deltaTime;
    }

    protected override void Die()
    {
        base.Die();
    }
}