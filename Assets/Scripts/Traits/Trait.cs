using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class Trait
{
    [SerializeField] internal TraitData traitData;
    [SerializeField] bool isVisible;

    public void ApplyEffects(Character character, EffectType effectType)
    {
        var effectsOfType = traitData?.effects?.Where(x => 
            x.traitEffectData.effectType == effectType) ?? new List<TraitEffect>();
        foreach (var effect in effectsOfType)
        {
            //Debug.Log("applied effect");

            effect.ApplyEffect(character);
        }
    }

    public void UnHide()
    {
        isVisible = true;
    }

    public bool IsVisible() => !isVisible;
}
