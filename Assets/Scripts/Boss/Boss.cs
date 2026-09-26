using System.Collections.Generic;
using UnityEngine;

public abstract class Boss : MonoBehaviour
{
    [SerializeField] protected float maxHealth;
    [SerializeField] protected float moveSpeed;

    protected float currentHealth;
    protected Transform playerPos;

    protected BossState state;

    protected List<BossAttack> attacks;

    protected virtual void Start()
    {
        currentHealth = maxHealth;
        state = BossState.Intro;
    }

    protected virtual void Update()
    {
        switch (state)
        {
            case BossState.Intro:
                HandleIntro();
                break;

            case BossState.Idle:
                ChooseNextAction();
                break;

            case BossState.Moving:
                HandleMovement();
                break;

            case BossState.Attacking:
                break;

            case BossState.Dead:
                break;
        }
    }

    
    protected virtual void Move() { }
    protected virtual void Die() { }

    protected virtual void HandleIntro() { }
    protected virtual void ChooseNextAction() { }
    protected virtual void HandleMovement() { }

    protected virtual BossAttack PickAttack()
    {
        List<BossAttack> validAttacks = new List<BossAttack>();

        foreach (BossAttack attack in attacks)
        {
            if (attack.CanUse(this))
            {
                validAttacks.Add(attack);
            }
        }

        if (validAttacks.Count == 0) return null;

        return validAttacks[Random.Range(1, validAttacks.Count)];
    }

    public virtual void TakeDamage(float damage)
    {
        if (state == BossState.Dead)
            return;

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public virtual void ChangeState(BossState state)
    {
        this.state = state;
    }
}

public enum BossState
{
    Intro,
    Idle,
    Attacking,
    Moving,
    Dead
}