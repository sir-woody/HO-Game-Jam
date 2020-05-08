using System;
using UnityEngine;

[RequireComponent(typeof(Character))]
public class StatController : MonoBehaviour
{
    [SerializeField] internal Stat[] stats;

    void Start()
    {
        foreach (var stat in stats)
        {
            stat.Initialize();
        }
    }
}

[Serializable]
public class Stat
{
    public string name;
    public string description;
    float value;
    [SerializeField] float maxValue = 100f;
    float maxValueModifier;
    [SerializeField] internal float usePerSecond = 1f;
    [SerializeField] internal bool startingFull;

    public event Action<float, float> OnChange;

    internal void Initialize()
    {
        if (startingFull) value = maxValue;
    }

    float MaxValue => maxValue * maxValueModifier;

    public float GetPercentage() => value / maxValue;

    internal void ApplyTraitEffect(float effect, EffectType traitType, bool resetStatValue = false)
    {
        if (traitType.HasFlag(EffectType.Initial))
        {
            maxValueModifier += effect;
            if (resetStatValue) ResetStat();
        }
        if (traitType.HasFlag(EffectType.Continous))
        {
            if (resetStatValue) ResetStat();
            value += effect;
        }
        OnChange?.Invoke(value, maxValue);
    }

    void ResetStat()
    {
        if (startingFull)
        {
            value = maxValue;
        }
        else
        {
            value = 0;
        }
    }

    internal void Deplete()
    {
        value -= startingFull
            ? Math.Min(usePerSecond * Time.deltaTime, value)
            : -Math.Min(usePerSecond * Time.deltaTime, MaxValue - value);
        OnChange?.Invoke(value, maxValue);
    }

    internal void Replenish(float amount)
    {
        value += startingFull
            ? Math.Min(amount, maxValue - value)
            : -Math.Max(amount, value);
        OnChange?.Invoke(value, maxValue);
    }

    internal void Refresh()
    {
        OnChange?.Invoke(value, maxValue);
    }
}
