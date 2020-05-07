using System;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Character))]
public class Stat : MonoBehaviour
{
    public new string name;
    public string description;
    float value;
    [SerializeField] float maxValue = 100f;
    float maxValueModifier;
    [SerializeField] internal float usePerSecond = 1f;
    [SerializeField] internal bool startingFull;
    Character character;

    void Start()
    {
        character = GetComponent<Character>();
        if (startingFull) value = maxValue;
    }

    float MaxValue => maxValue * maxValueModifier;

    public float GetPercentage() => value / maxValue;

    internal void ApplyTraitEffect(float effect, EffectType traitType, bool resetStatValue = false)
    {
        if(traitType.HasFlag(EffectType.Initial))
        {
            maxValueModifier += effect;
            if (resetStatValue) ResetStat();
        }
        if(traitType.HasFlag(EffectType.Continous))
        {
            if (resetStatValue) ResetStat();
            value += effect;
        }
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
    }

    internal void Replenish(float amount)
    {
        value += startingFull 
            ? Math.Min(amount, maxValue - value) 
            : -Math.Max(amount, value);
    }
}
