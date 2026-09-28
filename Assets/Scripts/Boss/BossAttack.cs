using System.Collections;
using UnityEngine;

public abstract class BossAttack : MonoBehaviour
{
    [SerializeField] protected float cooldown = 2f;

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
}