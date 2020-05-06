using UnityEngine;

public class Item : MonoBehaviour
{
    bool isUsed;
    string affectedStat;
    float statReplenishEffect;
    float weight;

    public void Use(Character character)
    {
        character.GetStat(affectedStat).Replenish(statReplenishEffect);
        isUsed = true;
    }

    void LateUpdate()
    {
        if (isUsed)
        {
            Destroy(this.gameObject);
        }
    }
}
