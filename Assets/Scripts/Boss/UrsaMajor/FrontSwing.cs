using System.Collections;
using UnityEngine;

public class FrontSwing : BossAttack
{
    [SerializeField] private float range = 2.5f;
    [SerializeField] private float approachRange = 2.5f;
    [SerializeField] private float arcDegrees = 120f;
    [SerializeField] private float damage = 20f;
    [SerializeField] private float activeDuration = 0.15f;
    [SerializeField] private float windup = 0.5f;
    [SerializeField] private float recovery = 0.6f;

    private Vector2 swingDirection;

    public override bool CanUse(Boss boss)
    {
        if (boss == null || boss.PlayerPos == null)
            return false;


        // Check if the player is close enough to the boss
        Vector2 toPlayer = (Vector2)(boss.PlayerPos.position - boss.transform.position);
        return toPlayer.sqrMagnitude <= approachRange * approachRange;
    }

    public override IEnumerator Execute(Boss boss)
    {
        if (boss == null || boss.PlayerPos == null)
            yield break;

        while (boss != null && boss.PlayerPos != null)
        {
            Vector2 towardsPlayer = (Vector2)(boss.PlayerPos.position - boss.transform.position);

            float distance = towardsPlayer.magnitude;

            if (distance <= range)
                break;

            float step = Mathf.Min(boss.MoveSpeed * Time.deltaTime, distance - range);
            boss.transform.position += (Vector3)towardsPlayer.normalized * step;

            yield return null;
        }

        Vector2 forward = (Vector2)(boss.PlayerPos.position - boss.transform.position).normalized;

        yield return new WaitForSeconds(windup);

        swingDirection = forward;
        showHitbox = true;
        SpawnSlash(boss, swingDirection);

        Vector2 toPlayer = (Vector2)(boss.PlayerPos.position - boss.transform.position);

        bool inRange = toPlayer.sqrMagnitude <= range * range;
        bool inArc = toPlayer.sqrMagnitude > 0 && 
                        Vector2.Angle(forward, toPlayer) <= arcDegrees * 0.5f;

        if (inRange && inArc)
        {
            // Deal damage to the player
            print("Attacked player with " + this);
        }

        yield return new WaitForSeconds(activeDuration);
        showHitbox = false;

        yield return new WaitForSeconds(recovery);
    }

    private void OnDrawGizmos()
    {
        if (!showHitbox)
            return;

        Vector3 center = transform.position;
        float facingAngle = Mathf.Atan2(swingDirection.y, swingDirection.x)
                            * Mathf.Rad2Deg;
        float halfArc = arcDegrees * 0.5f;
        const int segments = 24;

        Gizmos.color = Color.cyan;

        Vector3 previousPoint = center + DirectionAt(facingAngle - halfArc) * range;

        for (int i = 1; i <= segments; i++)
        {
            float angle = Mathf.Lerp(
                facingAngle - halfArc,
                facingAngle + halfArc,
                i / (float)segments
            );

            Vector3 nextPoint = center + DirectionAt(angle) * range;
            Gizmos.DrawLine(previousPoint, nextPoint);
            previousPoint = nextPoint;
        }

        Gizmos.DrawLine(
            center,
            center + DirectionAt(facingAngle - halfArc) * range
        );
        Gizmos.DrawLine(
            center,
            center + DirectionAt(facingAngle + halfArc) * range
        );
    }

    private Vector3 DirectionAt(float angleDegrees)
    {
        float radians = angleDegrees * Mathf.Deg2Rad;
        return new Vector3(Mathf.Cos(radians), Mathf.Sin(radians), 0f);
    }
    
}
