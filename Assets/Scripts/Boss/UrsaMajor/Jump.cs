using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jump : BossAttack
{
    [Header("Jump Timing")]
    [SerializeField] private float timeBeforeShadow = 2f;
    [SerializeField] private float timeFromShadowToLanding = 2f;

    [Header("Shadow")]
    [SerializeField] private GameObject shadowPrefab;

    [Header("Shockwaves")]
    [SerializeField] private int shockwaveCount = 3;
    [SerializeField] private float delayBetweenShockwaves = 0.5f;
    [SerializeField] private float shockwaveMaxRadius = 10f;
    [SerializeField] private float shockwaveDuration = 8f;
    [SerializeField] private float playerHitRadius = 0.5f;
    [SerializeField] private float shockwaveDamage = 15f;
    [SerializeField] private Color gizmoColor = Color.cyan;

    private class Shockwave
    {
        public Vector2 center;
        public float radius;
    }

    private List<Shockwave> shockwaves = new List<Shockwave>();

    public override bool CanUse(Boss boss)
    {
        return boss != null && boss.PlayerPos != null;
    }

    public override IEnumerator Execute(Boss boss)
    {
        if (boss == null || boss.PlayerPos == null) 
            yield break;

        SpriteRenderer bossSpriteRenderer = boss.GetComponent<SpriteRenderer>();
        if (bossSpriteRenderer != null)
            bossSpriteRenderer.enabled = false;
        
        yield return new WaitForSeconds(timeBeforeShadow);

        Vector2 landingPosition = boss.PlayerPos.position;
        GameObject shadow = Instantiate(shadowPrefab, landingPosition, Quaternion.identity);

        yield return new WaitForSeconds(timeFromShadowToLanding);

        boss.transform.position = landingPosition;

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
    }

    private IEnumerator ExpandShockwave(Vector2 center, Transform player)
    {
        Shockwave wave = new Shockwave
        {
            center = center,
            radius = 0f
        };

        shockwaves.Add(wave);

        float elasped = 0f;
        float previousRadius = 0f;
        bool hasHitPlayer = false;

        while (elasped < shockwaveDuration)
        {
            elasped += Time.deltaTime;

            float progress = Mathf.Clamp01(elasped / shockwaveDuration);
            wave.radius = Mathf.Lerp(0f, shockwaveMaxRadius, progress);

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

        shockwaves.Remove(wave);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;

        foreach (Shockwave wave in shockwaves)
        {
            Gizmos.DrawWireSphere(wave.center, wave.radius);
        }
    } 
}
