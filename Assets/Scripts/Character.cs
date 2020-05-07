using System.Linq;
using UnityEngine;

public class Character : MonoBehaviour
{
    public new string name;
    public string description;
    Stat[] stats;
    TraitController TraitController;

    float baseSpeed = 1;
    [SerializeField] GameObject equipment;

    void Start()
    {
        stats = GetComponents<Stat>();
        //traits = GetComponents<Trait>();
        TraitController = GetComponent<TraitController>();
        TraitController.ApplyEffects(EffectType.Initial);
        GetItems().ToList().ForEach(Equip);
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

    public Character[] GetTeamMembers() => transform.parent.GetComponentsInChildren<Character>();

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
            TraitController.ApplyEffects(EffectType.Continous);
            
            foreach (var stat in stats)
            {
                stat.Deplete();
            }
           
            foreach (var item in GetItems().Where(x=>x.continousEffect))
            {
                item.Use(this);
            }
        }
    }
}
