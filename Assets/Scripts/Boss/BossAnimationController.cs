using UnityEngine;

public class BossAnimationController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private Vector3 previousPosition;
    private int isMoving = Animator.StringToHash("IsMoving");

    private void LateUpdate()
    {
        Vector3 movement = transform.position - previousPosition;
        animator.SetBool(isMoving, movement.sqrMagnitude > 0.0001f);
        previousPosition = transform.position;
    }

    public void FaceDirection(Vector2 direction)
    {
        if (direction.x < -0.01f)
            spriteRenderer.flipX = true;
        else if (direction.x > 0.01f)
            spriteRenderer.flipX = false;
    }

    public void PlayAttack(string triggerName)
    {
        animator.SetTrigger(triggerName);
    }

    public void SetBool(string parameterName, bool value)
    {
        animator.SetBool(parameterName, value);
    }
}
