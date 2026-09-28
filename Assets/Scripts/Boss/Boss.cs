using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Boss : MonoBehaviour
{
    [SerializeField] protected float maxHealth;
    public float MaxHealth { get { return maxHealth; } }
    [SerializeField] protected float currentHealth;
    [SerializeField] protected float moveSpeed;
    [SerializeField] protected Transform playerPos;
    [SerializeField] protected BossAnimationController bossAnimation;

    public Transform PlayerPos => playerPos;
    public float MoveSpeed => moveSpeed;
    public BossAnimationController BossAnimation => bossAnimation;
    protected BossState state;
    protected List<BossAttack> attacks = new List<BossAttack>();
    protected BossAttack activeAttack;
    protected Coroutine activeAttackCoroutine;

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

            // VERY DANGEROUS, YOU HAVE TO MANUALLY TAKE THE BOSS OUT OF WAITING
            case BossState.Waiting:
                break;

            case BossState.Moving:
                HandleMovement();
                break;

            case BossState.Attacking:
                break;

            case BossState.Dead:
                break;
        }

        if (playerPos != null && state != BossState.Attacking)
        {
            Vector2 direction = playerPos.position - transform.position;
            bossAnimation.FaceDirection(direction);
        }
    }

    
    protected virtual void Move() { }
    protected virtual void Die()
    {
        state = BossState.Dead;

        if (activeAttackCoroutine != null)
        {
            StopCoroutine(activeAttackCoroutine);
            activeAttackCoroutine = null;
        }
    }

    protected virtual void HandleIntro() { }
    protected virtual void ChooseNextAction()
    {
        BossAttack attack = PickAttack();
        activeAttack = attack;

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
        activeAttackCoroutine = StartCoroutine(RunAttack(attack));

        if (playerPos != null)
        {
            Vector2 direction = playerPos.position - transform.position;
            bossAnimation.FaceDirection(direction);
        }

        bossAnimation.PlayAttack(attack.GetType().Name);
    }

    private IEnumerator RunAttack(BossAttack attack)
    {
        yield return attack.Execute(this);

        activeAttackCoroutine = null;

        if (state != BossState.Dead)
        {
            state = BossState.Idle;
        }
    }

    public virtual void TakeDamage(float damage)
    {
        if (state == BossState.Dead || damage <= 0)
            return;

        currentHealth = Mathf.Max(0f, currentHealth - damage);

        if (currentHealth <= 0)
        {
            Die();
        }

        StartCoroutine(FlashRed());
    }

    private IEnumerator FlashRed()
    {
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();

        Color originalColor = sprite.color;

        sprite.color = Color.red;

        yield return new WaitForSeconds(0.1f);

        sprite.color = originalColor;
    }

    public virtual void ChangeState(BossState state)
    {
        this.state = state;
    }

    public void CancelCurrentAttack()
    {
        if (activeAttackCoroutine != null)
        {
            StopCoroutine(activeAttackCoroutine);
            activeAttackCoroutine = null;
        }

        if (activeAttack != null)
        {
            activeAttack.Nuke();
            activeAttack = null;
        }

        state = BossState.Waiting;
    }
}

public enum BossState
{
    Intro,
    Idle,
    Waiting,
    Attacking,
    Moving,
    Dead
}