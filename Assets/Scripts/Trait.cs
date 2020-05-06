using UnityEngine;

[CreateAssetMenu(fileName = "Trait", menuName = "ScriptableObjects/Trait")]
public class Trait : ScriptableObject
{
    [SerializeField] bool isVisible;
    public new string name;
    public string description;
    [SerializeField] string affectedStatName;
    [SerializeField] float effectPercentage;
    [SerializeField] Trait blockingTrait;


    public static float SumarizeEffect(Trait[] traits, string affectedStat)
    {
        float summarizedEffect = 0;

        foreach (var trait in traits)
        {
            if (trait.affectedStatName == affectedStat)
            {
                summarizedEffect += trait.effectPercentage / 100f;
            }
        }

        return summarizedEffect;
    }

    void UnHide()
    {
        isVisible = true;
    }

    public bool IsVisible() => !isVisible;
}
