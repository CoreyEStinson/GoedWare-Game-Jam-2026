using Unity.Mathematics;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Transform shakeTarget;

    [Header("Charge Vibration")]
    [SerializeField] private float vibrationAmount = 0.035f;
    [SerializeField] private float vibrationSpeed = 35f;

    private int IsMoving = Animator.StringToHash("IsMoving");
    private int Attack = Animator.StringToHash("Attack");
    private int Dodge = Animator.StringToHash("Dodge");
    private int Dash = Animator.StringToHash("Dash");

    private Vector3 shakeOffset;
    private float vibrationTime;
    private bool isCharging;

    public void SetMovement(Vector2 direction)
    {
        animator.SetBool(IsMoving, direction.sqrMagnitude >= 0.01f);

        if (direction.x < -0.01f)
            spriteRenderer.flipX = true;
        else if (direction.x > 0.01f) 
            spriteRenderer.flipX = false;
    }

    public void StartCharging()
    {
        isCharging = true;
        vibrationTime = 0f;
    }

    public void PlayAttack()
    {
        StopCharging();
        animator.SetTrigger(Attack);
    }

    public void PlayDodge()
    {
        StopCharging();
        animator.SetTrigger(Dodge);
    }

    public void PlayDash()
    {
        StopCharging();
        animator.SetTrigger(Dash);
    }

    private void LateUpdate()
    {
        if (!isCharging)
            return;

        vibrationTime += Time.deltaTime * vibrationSpeed;

        Vector3 newOffset = new Vector3(
            Mathf.Sin(vibrationTime),
            Mathf.Cos(vibrationTime * 1.37f),
            0f
        ) * vibrationAmount;

        shakeTarget.localPosition += newOffset - shakeOffset;
        shakeOffset = newOffset;
    }

    private void StopCharging()
    {
        isCharging = false;
        shakeTarget.localPosition -= shakeOffset;
        shakeOffset = Vector3.zero;
    }
}
