using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    // Serialized
    [Header ("Movement")]
    [SerializeField]
    private float moveSpeed;

    [Header ("Attacking")]
    [SerializeField]
    private float attackDamage;
    [SerializeField]
    private float attackCooldown;
    [SerializeField]
    private float attackRange;
    [SerializeField]
    private float attackDistance;
    [SerializeField]
    private Transform attackPoint;

    [Header ("Dodging")]
    [SerializeField]
    private float dodgeSpeed;
    [SerializeField]
    private float dodgeLength;
    [SerializeField]
    private float dodgeCooldown;

    [Header ("Dashing")]
    [SerializeField]
    private float dashLength;

    // Privates
    private float attackCooldownTimer;
    private float speed;
    private Vector3 movementDirection;
    private Vector3 dodgeDirection;
    private float dodgeTimer;
    private float dodgeCooldownTimer;
    private bool isDodging;
    private bool isCharging;
    private int chargeLevel;
    private float chargeStart;
    private bool isDashing;
    private List<GameObject> alreadyHit = new List<GameObject>();

    //TEMP
    [SerializeField]
    private GameObject attackIndicatorPrefab;

    void Start()
    {
        speed = moveSpeed;
    }

    public void Move(InputAction.CallbackContext context)
    {
        movementDirection = context.ReadValue<Vector2>();
    }

    public void Attack(InputAction.CallbackContext context)
    {
        // Prevent the player from attacking when
        if (isDodging || isDashing || attackCooldownTimer > 0f)
        {
            return;
        }

        if (context.started)
        {
            chargeStart = Time.time;
            isCharging = true;
        }
        
        if (context.canceled)
        {
            if (isCharging)
            {
                Collider2D[] allHit = Physics2D.OverlapCircleAll(attackPoint.position, attackRange);

                foreach (Collider2D hit in allHit)
                {
                    if (hit.CompareTag("Damageable"))
                    {
                        hit.GetComponent<HealthComponent>().TakeDamage(attackDamage * chargeLevel);
                        print(hit.gameObject.name);
                        print(attackDamage * chargeLevel);
                    }
                }

                chargeLevel = 1;
                isCharging = false;
                attackCooldownTimer = attackCooldown;

                //TEMP
                GetComponent<SpriteRenderer>().color = Color.white;
                GameObject indicator = Instantiate(attackIndicatorPrefab, attackPoint.transform.position, Quaternion.identity);
                indicator.transform.localScale = Vector3.one * attackRange * 2;
                Destroy(indicator, 0.1f);
            }
        }
    }

    public void Dodge(InputAction.CallbackContext context)
    {
        // Prevent the player from dodging when
        if (isDodging || isDashing || dodgeCooldownTimer > 0)
        {
            return;
        }

        if (context.started)
        {
            if (chargeLevel > 1)
            {
                DashAttack();
                // Doesn't do a regular dodge at the end
                return;
            }

            isDodging = true;
            speed = dodgeSpeed;
            dodgeTimer = dodgeLength;

            dodgeDirection = movementDirection;
            if (dodgeDirection == Vector3.zero)
            {
                dodgeDirection = new Vector3(1, 0, 0);
            }

            //Reset the attack state
            isCharging = false;
            chargeLevel = 1;

            //TEMP
            GetComponent<SpriteRenderer>().color = Color.green;
        }
    }

    private void DashAttack()
    {
        isDashing = true;
        speed = dodgeSpeed * chargeLevel;
        dodgeTimer = dashLength;

        Vector3 mousePos = Mouse.current.position.ReadValue();
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mousePos);
        Vector3 mouseFacing = mouseWorldPos - transform.position;
        mouseFacing.z = 0f;
        mouseFacing.Normalize();
        dodgeDirection = mouseFacing;

        //Reset the attack state
        isCharging = false;

        GetComponent<SpriteRenderer>().color = Color.blue;
    }

    private void DodgeEnd()
    {
        // If the player was dodging
        isDodging = false;

        // If the player was dashing
        isDashing = false;
        chargeLevel = 1;
        alreadyHit.Clear();

        dodgeCooldownTimer = dodgeCooldown;
        speed = moveSpeed;
        dodgeTimer = 0f;

        //TEMP
        GetComponent<SpriteRenderer>().color = Color.white;
    }

    void Update()
    {
        if (attackCooldownTimer > 0)
        {
            attackCooldownTimer -= Time.deltaTime;
        }
        if (dodgeTimer > 0)
        {
            dodgeTimer -= Time.deltaTime;
        }
        if (dodgeCooldownTimer > 0)
        {
            dodgeCooldownTimer -= Time.deltaTime;
        }
    }

    private void FixedUpdate()
    {
        Vector3 dir = movementDirection;
        if (isDodging || isDashing)
        {
            dir = dodgeDirection;
        }
        transform.position += dir * speed * Time.fixedDeltaTime;

        if (isDodging || isDashing)
        {
            if (dodgeTimer <= 0f)
            {
                DodgeEnd();
            }
        }

        if (isDashing)
        {
            Collider2D[] allHit = Physics2D.OverlapCircleAll(transform.position + speed * dodgeDirection * Time.fixedDeltaTime, attackRange * 2);

            foreach (Collider2D hit in allHit)
            {
                if (hit.CompareTag("Damageable") && !alreadyHit.Contains(hit.gameObject))
                {
                    hit.GetComponent<HealthComponent>().TakeDamage(attackDamage * chargeLevel);
                    alreadyHit.Add(hit.gameObject);
                    print(hit.gameObject.name);
                    print(attackDamage * chargeLevel);
                }
            }

            //TEMP
            GameObject indicator = Instantiate(attackIndicatorPrefab, transform.position + speed * dodgeDirection * Time.fixedDeltaTime, Quaternion.identity);
            indicator.transform.localScale = Vector3.one * attackRange * 4;
            Destroy(indicator, 0.1f);
        }

        // Positions the attack point(s)
        Vector3 mousePos = Mouse.current.position.ReadValue();
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mousePos);
        Vector3 mouseFacing = mouseWorldPos - transform.position;
        mouseFacing.z = 0f;
        mouseFacing.Normalize();

        attackPoint.position = transform.position + (mouseFacing * attackDistance);

        // Attack charging
        if (isCharging)
        {
            switch (Time.time - chargeStart)
            {
                case < 1:
                    //fizzle
                    chargeLevel = 1;
                    break;

                case < 2:
                    //stage 1 charge
                    chargeLevel = 2;
                    GetComponent<SpriteRenderer>().color = Color.yellow;
                    break;

                case < 3:
                    //stage 2 charge
                    chargeLevel = 3;
                    GetComponent<SpriteRenderer>().color = Color.orange;
                    break;
                default:
                    //max charge
                    chargeLevel = 4;
                    GetComponent<SpriteRenderer>().color = Color.red;
                    break;
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
        {
            return;
        }

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
