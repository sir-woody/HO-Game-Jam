using System;
using UnityEngine;

[Serializable]
public class TraitEffect
{
    [SerializeField] internal TraitEffectData traitEffectData;
    float frequencyCounter;

    public void ApplyEffect(Character character, float elapsedTime)
    {
        if (frequencyCounter <= 0)
        {
            frequencyCounter -= Time.deltaTime;
            return;
        }

        if (!traitEffectData.isAffectingOthers)
        {
            character.GetStat(traitEffectData.affectedStat)
                .ApplyTraitEffect(traitEffectData.effectValue, traitEffectData.effectType);
        }
        else
        {
            foreach (var teamMembers in character.GetTeamMembers())
            {
                teamMembers.GetStat(traitEffectData.affectedStat)
                    .ApplyTraitEffect(traitEffectData.effectValue, traitEffectData.effectType);
            }
        }

        frequencyCounter = traitEffectData.frequency;
    }
}