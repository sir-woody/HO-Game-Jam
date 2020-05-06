using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Stat", menuName = "ScriptableObjects/Stat")]
public class Stat : ScriptableObject
{
    public new string name;
    public string description;
    float value;
    [SerializeField] float maxValue = 100f;
    [SerializeField] internal float usePerSecond = 1f;
    [SerializeField] internal bool startingFull;

    public void Initialize()
    {
        if (startingFull) value = maxValue;
    }

    public float GetPercentage() => value / maxValue;

    public void Deplete(float elapsedTime)
    {
        value -= startingFull
            ? Math.Min(elapsedTime * usePerSecond, value)
            : -Math.Min(elapsedTime * usePerSecond, maxValue - value);
    }

    internal void ApplyTraitEffect(float percentage)
    {
        maxValue *= percentage;
    }

    public void Replenish(float amount)
    {
        value += startingFull 
            ? Math.Min(amount, maxValue - value) 
            : -Math.Max(amount, value);
    }
}
