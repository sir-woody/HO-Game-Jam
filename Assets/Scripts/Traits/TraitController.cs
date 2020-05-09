using System;
using System.Linq;
using UnityEngine;
using UnityEngine.SocialPlatforms;

[RequireComponent(typeof(Character))]
public class TraitController : MonoBehaviour
{
    Character affectedCharacter;

    [SerializeField] Trait[] traits;
    [SerializeField] TraitData[] alltraits;
    
    public void Start()
    {
        
        affectedCharacter = GetComponent<Character>();
        Randomize();
    }

    public void ApplyEffects(EffectType effectType)
    {
        foreach (var trait in traits)
        {
            trait.ApplyEffects(affectedCharacter, effectType);
        }
    }

    public void Randomize()
    {
        for (int i = 0; i < traits.Length; i++)
        {
            if (traits[i].traitData == null)
            {
                traits[i].traitData = GetRandomTrait();
            }
        }
    }

    private TraitData GetRandomTrait()
    {
        var blockedTraits = traits
            .Where(x=>x.traitData!=null)
            .Select(x => x.traitData.blockingTrait);
        var availableTraits = alltraits.Except(traits.Select(x=>x.traitData)).Except(blockedTraits);
        if (!availableTraits.Any()) return null;
        var rnd = new System.Random((int)DateTime.Now.Ticks).Next(0, availableTraits.Count());
        return availableTraits.ElementAt(rnd);
    }
}
