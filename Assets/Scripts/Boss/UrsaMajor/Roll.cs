using System.Collections;
using UnityEngine;

public class Roll : BossAttack
{
    [SerializeField] private float rollDistance = 6f;
    [SerializeField] private float rollSpeed = 8f;
    [SerializeField] private float rollRangeMin = 5f;
    [SerializeField] private float rollRangeMax = 16f;
    [SerializeField] private float contactRadius = 1f;
    [SerializeField] private float damage = 15f;
    [SerializeField] private float hitCooldown = 0.75f;
    [SerializeField] private float recovery = 0.5f;

    private Vector2 rollStartPosition;
    private Vector2 rollDirection;

    public override bool CanUse(Boss boss)
    {
        if (boss == null || boss.PlayerPos == null)
        {
            return false;
        }

        Vector2 toPlayer = (Vector2)(boss.PlayerPos.position - boss.transform.position);
        return toPlayer.sqrMagnitude <= rollRangeMax * rollRangeMax && toPlayer.sqrMagnitude >= rollRangeMin * rollRangeMin;
    }

    public override IEnumerator Execute(Boss boss)
    {
        if (boss == null || boss.PlayerPos == null)
            yield break;

        // Cache the player's pos so the roll does not turn
        Vector2 start = boss.transform.position;
        Vector2 cachesPlayerPos = boss.PlayerPos.position;
        Vector2 toTarget = cachesPlayerPos - start;

        // Check for too small of a distance
        if (toTarget.sqrMagnitude <= Mathf.Epsilon)
            yield break;

        Vector2 direction = toTarget.normalized;

        rollStartPosition = boss.transform.position;
        rollDirection = direction;
        showHitbox = true;

        boss.BossAnimation.SetBool("IsRolling", true);

        float distanceTravelled = 0f;
        float nextHitTime = 0f;

        while (distanceTravelled < rollDistance)
        {
            float step = Mathf.Min(
                rollSpeed * Time.deltaTime,
                rollDistance - distanceTravelled
            );

            boss.transform.position += (Vector3)(direction * step);
            distanceTravelled += step;

            if (Time.time >= nextHitTime && 
                Vector2.Distance(boss.transform.position, boss.PlayerPos.position) <= contactRadius)
            {
                // Damage player 
                print("Attacked player with " + this);

                nextHitTime = Time.time + hitCooldown;
            }

            yield return null;
        }

        boss.BossAnimation.SetBool("IsRolling", false);

        showHitbox = false;

        yield return new WaitForSeconds(recovery);
    }

    private void OnDrawGizmos()
    {
        if (!showHitbox)
            return;

        float width = contactRadius * 2f;
        Vector3 start = rollStartPosition;
        Vector3 center = start + (Vector3)(rollDirection * (rollDistance * 0.5f));

        Gizmos.color = Color.yellow;
        Gizmos.matrix = Matrix4x4.TRS(
            center,
            Quaternion.FromToRotation(Vector3.right, rollDirection),
            Vector3.one
        );
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(rollDistance, width, 0f));

        // The boss's current contact hitbox.
        Gizmos.matrix = Matrix4x4.identity;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, contactRadius);
    }
    
}
