using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Flurry : BossAttack
{
    [SerializeField] private int numOfSwings = 3;
    [SerializeField] private float range = 2.5f;
    [SerializeField] private float arcDegrees = 120f;
    [SerializeField] private float damage = 20f;
    [SerializeField] private float lungeDistance = 2f;
    [SerializeField] private float lungeTime = 0.1f;
    [SerializeField] private float turnLimitDegrees = 60f;
    [SerializeField] private float activeDuration = 0.15f;
    [SerializeField] private float windup = 0.5f;
    [SerializeField] private float recoveryBetweenSwings = 0.3f;
    [SerializeField] private float recovery = 0.6f;

    private Vector2 swingDirection;

    public override bool CanUse(Boss boss)
    {
        if (boss == null || boss.PlayerPos == null)
            return false;


        // Check if the player is close enough to the boss
        Vector2 toPlayer = (Vector2)(boss.PlayerPos.position - boss.transform.position);
        return toPlayer.sqrMagnitude <= range * range;
    }

    public override IEnumerator Execute(Boss boss)
    {
        if (boss == null || boss.PlayerPos == null)
            yield break;

        Vector2 previousForward = Vector2.zero;

        for (int i = 0; i < numOfSwings; i++)
        {
            Vector2 forward = (Vector2)(boss.PlayerPos.position - boss.transform.position).normalized;

            if (previousForward != Vector2.zero &&
                Vector2.Angle(previousForward, forward) > turnLimitDegrees)
            {
                int sign = (Vector2.SignedAngle(forward, previousForward) >= 0) ? -1 : 1;
                forward = Quaternion.Euler(0, 0, turnLimitDegrees * sign) * previousForward;
            }

            swingDirection = forward;

            yield return new WaitForSeconds(windup);

            yield return StartCoroutine(LerpMove((Vector2)transform.position + (forward * lungeDistance), lungeTime));

            showHitbox = true;
            SpawnSlash(boss, forward);

            Vector2 toPlayer = (Vector2)(boss.PlayerPos.position - boss.transform.position);

            bool inRange = toPlayer.sqrMagnitude <= range * range;
            bool inArc = toPlayer.sqrMagnitude > 0 &&
                            Vector2.Angle(forward, toPlayer) <= arcDegrees * 0.5f;

            if (inRange && inArc)
            {
                // Deal damage to the player
                boss.playerHealthComponent.TakeDamage(damage);
                print("Attacked player with " + this);
            }

            yield return new WaitForSeconds(activeDuration);
            showHitbox = false;

            previousForward = forward;
            yield return new WaitForSeconds(recoveryBetweenSwings);
        }

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

    private IEnumerator LerpMove(Vector2 endPos, float time)
    {
        Vector2 startPos = transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < time)
        {
            float t = elapsedTime / time;

            transform.position = Vector2.Lerp(startPos, endPos, t);

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        transform.position = endPos;
    }
}