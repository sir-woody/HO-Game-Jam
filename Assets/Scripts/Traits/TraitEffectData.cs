using UnityEngine;

[CreateAssetMenu(fileName = "TraitEffectData", menuName = "ScriptableObjects/TraitEffectData")]
public class TraitEffectData : ScriptableObject
{
    [SerializeField] internal bool isAffectingOthers;
    [SerializeField] internal EffectType effectType;
    [SerializeField] internal string affectedStat;
    [SerializeField] internal float effectValue;
    [SerializeField] internal float frequency;
}
