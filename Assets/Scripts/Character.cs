using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Character : MonoBehaviour
{
    [Serializable]
    public class SpriteGroup
    {
        public Sprite idle;
        public Sprite hoover;
    }
    public enum SpriteType
    {
        Idle,
        Hoover,
    }

    public new string name;
    public string description;
    public List<SpriteGroup> sprites;
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
    public void StartClimbing()
    {
        isClimbing = true;
    }
    public void StopClimbing()
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

    /// <summary>
    /// Character was clicked during <see cref="RestEvent"/>.
    /// Start a dialog
    /// </summary>
    public void OnCharacterClicked()
    {

    }

    /// <summary>
    /// TODO: change <see cref="sprite"/> to a list of sprites,
    /// select a sprite depending on current character status (healthy, sick, tired, etc.).
    /// </summary>
    public Sprite GetSprite(SpriteType spriteType)
    {
        int spriteIndex = 0;
        switch (spriteType)
        {
            case SpriteType.Idle: return sprites[spriteIndex].idle;
            case SpriteType.Hoover: return sprites[spriteIndex].hoover;
            default: return null;
        }
    }
}
