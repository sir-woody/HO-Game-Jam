using UnityEngine;

[CreateAssetMenu(fileName = "TraitData", menuName = "ScriptableObjects/TraitData")]
public class TraitData : ScriptableObject
{
    public new string name;
    public string description;
    [SerializeField] internal TraitData blockingTrait;
    [SerializeField] internal TraitEffect[] effects;
}
