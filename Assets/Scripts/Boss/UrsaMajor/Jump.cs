using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jump : BossAttack
{
    [Header("Jump Timing")]
    [SerializeField] private float timeBeforeShadow = 2f;
    [SerializeField] private float timeFromShadowToLanding = 2f;
    [SerializeField] private float takeoffAnimationDuration = 0.4f;

    [Header("Shadow")]
    [SerializeField] private GameObject shadowPrefab;
    [SerializeField] private float slamRadius = 3f;
    [SerializeField] private float slamDamage = 10f;

    [Header("Shockwaves")]
    [SerializeField] private int shockwaveCount = 3;
    [SerializeField] private float delayBetweenShockwaves = 0.5f;
    [SerializeField] private float shockwaveMaxRadius = 10f;
    [SerializeField] private float shockwaveDuration = 8f;
    [SerializeField] private float playerHitRadius = 0.5f;
    [SerializeField] private float shockwaveDamage = 15f;
    [SerializeField] private Color gizmoColor = Color.cyan;

    [Header("Shockwave Visuals")]
    [SerializeField] private LineRenderer shockwaveLinePrefab;
    [SerializeField] private int ringSegments;

    private class Shockwave
    {
        public Vector2 center;
        public float radius;
        public LineRenderer lineRenderer;
        public Vector3[] positions;
    }

    private List<Shockwave> shockwaves = new List<Shockwave>();
    private Vector2 landingPosition;
    private bool hasLandingPosition;

    public override bool CanUse(Boss boss)
    {
        return boss != null && boss.PlayerPos != null;
    }

    public override IEnumerator Execute(Boss boss)
    {
        if (boss == null || boss.PlayerPos == null) 
            yield break;

        SpriteRenderer bossSpriteRenderer = boss.GetComponent<SpriteRenderer>();
            float visibleTakeoffTime = Mathf.Min(
            takeoffAnimationDuration,
            timeBeforeShadow
        );

        yield return new WaitForSeconds(visibleTakeoffTime);

        if (bossSpriteRenderer != null)
            bossSpriteRenderer.enabled = false;

        yield return new WaitForSeconds(timeBeforeShadow - visibleTakeoffTime);

        landingPosition = boss.PlayerPos.position;
        GameObject shadow = Instantiate(shadowPrefab, landingPosition, Quaternion.identity);

        yield return new WaitForSeconds(timeFromShadowToLanding);

        hasLandingPosition = true;

        boss.transform.position = landingPosition;

        if (bossSpriteRenderer != null)
            bossSpriteRenderer.enabled = true;

        if (boss.BossAnimation != null)
            boss.BossAnimation.PlayAttack("Land");

        // If player is inside of the slam circle
        if (boss.PlayerPos != null &&
            Vector2.Distance(landingPosition, boss.PlayerPos.position) <= slamRadius)
        {
            // Deal damage to player
            print("Attacked player with " + this);
        }

        if (bossSpriteRenderer != null)
            bossSpriteRenderer.enabled = true;

        if (shadow != null)    
            Destroy(shadow);

        for (int i = 0; i < shockwaveCount; i++)
        {
            StartCoroutine(ExpandShockwave(landingPosition, boss.PlayerPos));

            if (i < shockwaveCount - 1) 
                yield return new WaitForSeconds(delayBetweenShockwaves);
        } 

        hasLandingPosition = false;
    }

    private IEnumerator ExpandShockwave(Vector2 center, Transform player)
    {
        Shockwave wave = new Shockwave
        {
            center = center,
            radius = 0f,
            lineRenderer = Instantiate(shockwaveLinePrefab),
            positions = new Vector3[ringSegments]
        };

        wave.lineRenderer.useWorldSpace = true;
        wave.lineRenderer.loop = true;
        wave.lineRenderer.positionCount = ringSegments;

        Destroy(wave.lineRenderer.gameObject, shockwaveDuration + 0.1f);

        shockwaves.Add(wave);

        float elasped = 0f;
        float previousRadius = 0f;
        bool hasHitPlayer = false;

        while (elasped < shockwaveDuration)
        {
            elasped += Time.deltaTime;

            float progress = Mathf.Clamp01(elasped / shockwaveDuration);
            wave.radius = Mathf.Lerp(0f, shockwaveMaxRadius, progress);
            UpdateShockwaveLine(wave, center);

            if (!hasHitPlayer && player != null)
            {
                float playerDistance = Vector2.Distance(center, player.position);

                bool waveReachedPlayer = 
                    playerDistance >= previousRadius - playerHitRadius &&
                    playerDistance <= wave.radius + playerHitRadius;

                if (waveReachedPlayer)
                {
                    // Deal damage to player
                    print("Attacked player with " + this);
                    
                    hasHitPlayer = true;
                }
            }

            previousRadius = wave.radius;
            yield return null;
        }

        Destroy(wave.lineRenderer.gameObject);
        shockwaves.Remove(wave);
    }

    private void UpdateShockwaveLine(Shockwave wave, Vector2 center)
    {
        for (int i = 0; i < ringSegments; i++)
        {
            float angle = i * Mathf.PI * 2f / ringSegments;

            wave.positions[i] = new Vector3(
                center.x + Mathf.Cos(angle) * wave.radius,
                center.y + Mathf.Sin(angle) * wave.radius,
                0
            );
        }

        wave.lineRenderer.SetPositions(wave.positions);
    }

    private void OnDrawGizmos()
    {
        if (hasLandingPosition)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(landingPosition, slamRadius);
        }

        Gizmos.color = gizmoColor;

        foreach (Shockwave wave in shockwaves)
        {
            Gizmos.DrawWireSphere(wave.center, wave.radius);
        }
    } 
}
