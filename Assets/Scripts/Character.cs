using System.Linq;
using UnityEngine;

public class Character : MonoBehaviour
{
    float baseSpeed = 1;
    [SerializeField] GameObject equipment;
    [SerializeField] Trait[] traits;
    [SerializeField] Stat[] stats;

    void Initialize()
    {
        foreach (var stat in stats)
        {
            stat.ApplyTraitEffect(Trait.SumarizeEffect(traits, stat.name));
            stat.Initialize();
        }
    }

    //Item management
    public Item[] GetItems() => equipment.GetComponentsInChildren<Item>();

    public void Equip(Item item)
    {
        item.gameObject.transform.parent = equipment.transform;
        item.gameObject.SetActive(false);
    }

    public void Unequip(Item item)
    {
        item.gameObject.transform.parent = null;
        item.gameObject.SetActive(true);
    }

    //Stats
    internal Stat GetStat(string name) => stats.FirstOrDefault(x => x.name == name);

    internal float GetStatPercentage(string name) => GetStat(name)?.GetPercentage() ?? 0f;

    float GetCurrentSpeed() => baseSpeed * GetStatPercentage("health") * GetStatPercentage("stamina");


    //Deplete stats during travel
    bool isClimbing;

    void Climb()
    {
        isClimbing = true;
    }
    void StopClimbing()
    {
        isClimbing = false;
    }

    void Update()
    {
        if (isClimbing)
        {
            foreach (var stat in stats)
            {
                stat.Deplete(Time.deltaTime);
            }
        }
    }
}
