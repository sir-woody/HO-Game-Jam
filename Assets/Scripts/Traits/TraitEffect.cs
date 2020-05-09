using System;
using UnityEngine;

[Serializable]
public class TraitEffect
{
    [SerializeField] internal TraitEffectData traitEffectData;
    float frequencyCounter;

    public void ApplyEffect(Character character)
    {
        //Debug.Log("ApplyEffect");

        if (frequencyCounter > 0)
        {
            frequencyCounter -= Time.deltaTime;
            return;
        }
        //Debug.Log("frequencyCounter = 0");

        if (!traitEffectData.isAffectingOthers)
        {
            //if(character == null) Debug.Log("character is null");
            //if(character.GetStat(traitEffectData.affectedStat) == null) Debug.Log("character.GetStat(traitEffectData.affectedStat) is null");

            character.GetStat(traitEffectData.affectedStat)
                ?.ApplyTraitEffect(traitEffectData.effectValue, traitEffectData.effectType);

            //Debug.Log($"effect applied {traitEffectData.effectValue} on {traitEffectData.affectedStat} of {character.gameObject.name}");
        }
        else
        {
            foreach (var teamMembers in TeamManager.Instance.characters)
            {
                teamMembers.GetStat(traitEffectData.affectedStat)
                    ?.ApplyTraitEffect(traitEffectData.effectValue, traitEffectData.effectType);
            }
        }

        frequencyCounter = traitEffectData.frequency;
    }
}