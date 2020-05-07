using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Character))]
public class TraitController : MonoBehaviour
{
    Unity.Mathematics.Random random = new Unity.Mathematics.Random();
    Character affectedCharacter;

    [SerializeField] Trait[] traits;
    [SerializeField] Trait[] alltraits;
    
    public void Start()
    {
        affectedCharacter = GetComponent<Character>();
        Randomize();
    }

    public void ApplyEffects(EffectType effectType)
    {
        foreach (var trait in traits)
        {
            trait.ApplyEffects(affectedCharacter, effectType, Time.deltaTime);
        }
    }

    public void Randomize()
    {
        for (int i = 0; i < traits.Length; i++)
        {
            if (traits[i] == null) traits[i] = GetRandomTrait();
        }
    }

    private Trait GetRandomTrait()
    {
        var blockedTraits = traits.Select(x => x.traitData.blockingTrait);
        var availableTraits = alltraits.Except(traits).Select(x=>x.traitData).Except(blockedTraits);
        var randomTrait = availableTraits.ElementAt(random.NextInt(0, availableTraits.Count() - 1));

        return new Trait { traitData = randomTrait };
    }
}
