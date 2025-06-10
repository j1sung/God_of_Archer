using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    private Stats stats;

    public float HP
    {
        get => stats.HP;
        set => stats.HP = Mathf.Clamp(value, 0, MaxHP);
    }

    public float SP
    {
        get => stats.SP;
        set => stats.SP = Mathf.Clamp(value, 0, MaxSP);
    }
    public abstract float MaxHP { get; }
    public abstract float MaxSP { get; }
    public abstract float HPRecovery { get; }
    public abstract float SPRecovery { get; }

    protected void Setup()
    {
        HP = MaxHP;
        SP = MaxSP;

        StartCoroutine("Recovery");
    }

    protected IEnumerator Recovery()
    {
        while (true)
        {
            if (HP < MaxHP) HP += HPRecovery;
            if (SP < MaxSP) SP += SPRecovery;

            yield return new WaitForSeconds(2);
        }

    }


}

[System.Serializable]
public struct Stats
{
    [HideInInspector] public float HP;
    [HideInInspector] public float SP;
}