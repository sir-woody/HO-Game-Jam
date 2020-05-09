using UnityEngine;

public class Item : MonoBehaviour
{
    bool isUsed;
    public string description;
    [SerializeField] internal string affectedStat;
    [SerializeField] internal float effect;
    [SerializeField] internal bool continousEffect;
    [SerializeField] bool expires = true;
    [SerializeField] float weight;

    public void Use(Character character)
    {
        var value = continousEffect ? Time.deltaTime * effect : effect;
        character.GetStat(affectedStat).Replenish(value);
        isUsed = true;
    }

    void LateUpdate()
    {
        if (expires && isUsed)
        {
            Destroy(this.gameObject);
        }
    }
    public bool CanUse() => !isUsed;
    public void MakeUsable() => isUsed = false;
}
