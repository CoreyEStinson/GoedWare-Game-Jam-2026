using System.Collections.Generic;
using UnityEngine;

public class UrsaMajor : Boss
{
    [Header("Ursa Major Attacks")] 
    [SerializeField] private BossAttack frontSwing;
    [SerializeField] private BossAttack roll;
    [SerializeField] private BossAttack jumpAndLand;

    [SerializeField] private float stoppingDistance = 2f;
    [SerializeField] private float maxTimeMoving = 8f;

    private float timeMoving;

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

        timeMoving += Time.deltaTime;

        if (timeMoving >= maxTimeMoving)
        {
            BossAttack attack = PickReadyAttack();

            if (attack != null)
            {
                timeMoving = 0f;
                StartAttack(attack);
                return;
            }

            timeMoving = 0f;
        }

        float distance = Vector3.Distance(transform.position, playerPos.position);

        if (distance <= stoppingDistance)
        {
            timeMoving = 0f;
            ChangeState(BossState.Idle);
            return;
        }

        Vector3 direction = (playerPos.position - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;
    }

    // Ignore if the attack is valid to ignore distance checks
    private BossAttack PickReadyAttack()
    {
        List<BossAttack> readyAttacks = new List<BossAttack>();

        foreach (BossAttack attack in attacks)
        {
            if (attack != null && attack.IsOffCooldown()) 
                readyAttacks.Add(attack);
        }

        if (readyAttacks.Count == 0) 
            return null;
        
        return readyAttacks[Random.Range(0, readyAttacks.Count)];
    }

    protected override void Die()
    {
        base.Die();
    }
}