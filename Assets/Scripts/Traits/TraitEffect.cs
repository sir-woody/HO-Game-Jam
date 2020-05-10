using System;
using UnityEngine;

[Serializable]
public class TraitEffect
{
    [SerializeField] internal TraitEffectData traitEffectData;

    public void ApplyEffect(Character character)
    {
        var value = Time.deltaTime * traitEffectData.frequency * traitEffectData.effectValue;

        if (!traitEffectData.isAffectingOthers)
        {
            character.GetStat(traitEffectData.affectedStat)
                ?.ApplyTraitEffect(value, traitEffectData.effectType);
        }
        else
        {
            foreach (var teamMembers in TeamManager.Instance.characters)
            {
                teamMembers.GetStat(traitEffectData.affectedStat)
                    ?.ApplyTraitEffect(value, traitEffectData.effectType);
            }
        }
    }
}