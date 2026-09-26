using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Boss : MonoBehaviour
{
    [SerializeField] protected float maxHealth;
    [SerializeField] protected float moveSpeed;
    [SerializeField] protected Transform playerPos;

    public Transform PlayerPos => playerPos;

    protected float currentHealth;
    protected BossState state;
    protected List<BossAttack> attacks = new List<BossAttack>();

    private Coroutine activeAttack;

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
    protected virtual void Die()
    {
        state = BossState.Dead;

        if (activeAttack != null)
        {
            StopCoroutine(activeAttack);
            activeAttack = null;
        }
    }

    protected virtual void HandleIntro() { }
    protected virtual void ChooseNextAction()
    {
        BossAttack attack = PickAttack();

        if (attack != null)
        {
            StartAttack(attack);
        }
        else
        {
            ChangeState(BossState.Moving);
        }
    }
    protected virtual void HandleMovement() { }

    protected virtual BossAttack PickAttack()
    {
        List<BossAttack> validAttacks = new List<BossAttack>();

        foreach (BossAttack attack in attacks)
        {
            if (attack != null && attack.IsOffCooldown() && attack.CanUse(this))
                validAttacks.Add(attack);
        }

        if (validAttacks.Count == 0) return null;

        return validAttacks[Random.Range(0, validAttacks.Count)];
    }

    protected void StartAttack(BossAttack attack)
    {
        if (state == BossState.Dead || attack == null)
        {
            return;
        }

        state = BossState.Attacking;
        attack.MarkUsed();
        activeAttack = StartCoroutine(RunAttack(attack));
    }

    private IEnumerator RunAttack(BossAttack attack)
    {
        yield return attack.Execute(this);

        activeAttack = null;

        if (state != BossState.Dead)
        {
            state = BossState.Idle;
        }
    }

    public virtual void TakeDamage(float damage)
    {
        if (state == BossState.Dead || damage <= 0)
            return;

        currentHealth -= Mathf.Max(0f, currentHealth - damage);

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