using System.Collections;
using UnityEngine;

public abstract class BossAttack : MonoBehaviour
{
    [SerializeField] protected float cooldown = 2f;

    [Header("Slash Visual")]
    [SerializeField] private GameObject slashPrefab;
    [SerializeField] protected float slashDistance = 1.5f;
    [SerializeField] private float slashLifetime = 0.25f;
    [SerializeField] private float slashRotationOffset;

    protected float lastUsedTime = float.NegativeInfinity;
    protected bool showHitbox;

    public abstract bool CanUse(Boss boss);
    public abstract IEnumerator Execute(Boss boss);

    public bool IsOffCooldown()
    {
        return Time.time >= lastUsedTime + cooldown;
    }

    public void MarkUsed()
    {
        lastUsedTime = Time.time;
    }

    public void Nuke()
    {
        StopAllCoroutines();
    }

    protected void SpawnSlash(Boss boss, Vector2 direction)
    {
        if (slashPrefab == null || boss == null || direction.sqrMagnitude < 0.001f)
            return;

        direction.Normalize();

        Vector2 position =
            (Vector2)boss.transform.position + direction * slashDistance;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.Euler(0f, 0f, angle + slashRotationOffset);

        GameObject slash = Instantiate(slashPrefab, position, rotation);
        Destroy(slash, slashLifetime);
    }
}