using UnityEngine;

public abstract class BossAttack
{
    [SerializeField] protected float cooldown;

    protected float lastUsedTime;

    public abstract bool CanUse(Boss boss);
    public abstract void Execute(Boss boss);

    public bool IsOffCooldown()
    {
        return Time.time >= lastUsedTime + cooldown;
    }
}