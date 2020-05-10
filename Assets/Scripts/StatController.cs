﻿using System;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;

[RequireComponent(typeof(Character))]
public class StatController : MonoBehaviour
{
    [SerializeField] internal Stat[] stats;

    void Start()
    {
        foreach (var stat in stats)
        {
            stat.Initialize(stats);
        }
    }
}

[Serializable]
public class Stat
{
    public string name;
    public string description;
    public string overflowTo;
    Stat overflowToStat;
    float value;
    [SerializeField] float regeneration = 70f;
    [SerializeField] float maxValue = 100f;
    float maxValueModifier;
    [SerializeField] internal float usePerSecond = 1f;
    [SerializeField] internal bool startingFull;
    private bool isInitialized;
    [SerializeField] Sprite sprite;

    public event Action<float, float> OnChange;

    public Sprite GetSprite() => sprite;

    internal void Initialize(Stat[] stats)
    {
        if (startingFull) value = MaxValue;
        overflowToStat = stats.FirstOrDefault(x => x.name == overflowTo);
        OnChange?.Invoke(value, MaxValue);
        isInitialized = true;
    }

    float MaxValue => maxValue + maxValueModifier;

    public float GetPercentage() => value / MaxValue;

    internal void ApplyTraitEffect(float effect, EffectType traitType, bool resetStatValue = false)
    {
        if (traitType == EffectType.Initial)
        {
            maxValueModifier += effect;
            if (resetStatValue) ResetStat();
        }
        if (traitType == EffectType.Continous)
        {
            if (resetStatValue) ResetStat();
            value += effect;
        }
        OnChange?.Invoke(value, MaxValue);
    }

    void ResetStat()
    {
        if (startingFull)
        {
            value = MaxValue;
        }
        else
        {
            value = 0;
        }
    }

    internal void Deplete(float val)
    {
        if (startingFull)
        {
            if (value <= 0)
            {
                overflowToStat?.Deplete(val);
            }
            else
            {
                value -= val;
            }
        }
        else
        {
            if (value >= MaxValue)
            {
                overflowToStat?.Deplete(val);
            }
            else
            {
                value += val;
            }
        }
        OnChange?.Invoke(value, MaxValue);
    }

    internal bool IsDepleted() => isInitialized && startingFull ? value <= 0 : value >= maxValue;

    internal void Deplete()
    {
        if (usePerSecond == 0) return;
        
        Deplete(usePerSecond * Time.deltaTime);
    }

    internal void Replenish(float amount)
    {
        if (amount < 0)
        {
            Deplete(-amount);
        }
        else
        {
            value += startingFull
            ? Math.Min(amount, MaxValue - value)
            : -Math.Min(amount, value);
        }
        
        OnChange?.Invoke(value, MaxValue);
    }

    //bool IsHigh() => startingFull ? value >= MaxValue : value <= 0;
    //bool IsLow() => startingFull ? value <= 0 : value >= MaxValue;

    //internal void ReplenishWithOverflow(float val)
    //{


    //    if (startingFull)
    //    {
    //        if (value >= MaxValue)
    //        {
    //            return;
    //        }
    //        else if (value <= 0)
    //        {
    //            overflowToStat?.ReplenishWithOverflow(val);
    //        }
    //        else
    //        {
    //            value += val;
    //        }
    //    }
    //    else
    //    {
    //        if (value <= 0)
    //        {
    //            return;
    //        }
    //        else if(value >= MaxValue)
    //        {
    //            overflowToStat?.ReplenishWithOverflow(val);
    //        }
    //        else
    //        {
    //            value -= val;
    //        }
    //    }
    //    OnChange?.Invoke(value, MaxValue);
    //}

    internal void Refresh()
    {
        OnChange?.Invoke(value, MaxValue);
    }

    internal void Regenerate()
    {

        Replenish(regeneration);
    }
}
