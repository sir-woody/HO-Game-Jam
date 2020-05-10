using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Character : MonoBehaviour
{
    [Serializable]
    public class SpriteGroupSerializedDictionary : SerializedDictionary<SpriteType, Sprite> { }

    public enum SpriteType
    {
        Idle,
        Hoover,

        FrameDefault,
        FrameDeath,
        FrameWarning0,
        FrameWarning1,
    }

    public new string name;
    public string description;
    public SpriteGroupSerializedDictionary sprites;
    Stat[] stats;
    TraitController TraitController;
    VoiceController VoiceController;

    bool isDead;
    [SerializeField] float backpackSize = 10;
    [SerializeField] float baseSpeed = 1;
    [SerializeField] float minSpeed = .1f;
    [SerializeField] GameObject equipment;
    [SerializeField] List<BackpackManager.ItemType> startingItems = null;

    void Start()
    {
        stats = GetComponent<StatController>().stats;
        VoiceController = GetComponent<VoiceController>();
        TraitController = GetComponent<TraitController>();
        TraitController.ApplyEffects(EffectType.Initial);
        foreach (BackpackManager.ItemType itemType in startingItems)
        {
            BackpackManager.Instance.SpawnItem(this, itemType);
        }
    }

    //Item management
    public bool CanEquip() => GetItems().Count() < backpackSize;
    public Item[] GetItems() => equipment.GetComponentsInChildren<Item>(true);
    public void Equip(Item item)
    {
        item.Owner = this;
        item.gameObject.transform.parent = equipment.transform;
        item.gameObject.SetActive(false);
    }
    public void Unequip(Item item)
    {
        item.Owner = null;
        item.gameObject.transform.parent = null;
        item.gameObject.SetActive(true);
    }

    //Stats
    internal Stat GetStat(string name)
    {
        return stats.FirstOrDefault(x => x.name == name);
    }

    internal float GetStatPercentage(string name)
    {
        return GetStat(name)?.GetPercentage() ?? 0f;
    }

    internal float GetCurrentSpeed()
    {
        return Mathf.Clamp(baseSpeed * GetStatPercentage("Zdrowie") * GetStatPercentage("Energia"), minSpeed, 2 * baseSpeed);
    }

    //Deplete stats during travel
    private bool isClimbing;
    public void StartClimbing()
    {
        isClimbing = true;
    }
    public void StopClimbing()
    {
        isClimbing = false;
    }

    private void Update()
    {
        if (isClimbing)
        {
            TraitController.ApplyEffects(EffectType.Continous);

            foreach (Stat stat in stats)
            {
                stat.Deplete();
            }

            foreach (Item item in GetItems().Where(x => x.continousEffect))
            {
                item.Use(this);
            }
        }
        //temporary solution to test putting items into backpack during debuging
        //TODO remove after backpack is implemented
        GetItems().ToList().ForEach(Equip);

        if (GetStat("Zdrowie").IsDepleted())
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;

        //TODO Change sprite to gray,
        //Debug.LogError($"{nameof(Die)} not implemented");
    }

    public bool IsDead()
    {
        return isDead;
    }

    /// <summary>
    /// Character was clicked during <see cref="RestEvent"/>.
    /// Start a dialog
    /// </summary>
    public void OnCharacterClicked()
    {
        SoundManager.Instance.PlaySound(SoundManager.SoundType.Voice, VoiceController.GetOnClickSound());
        if (BackpackManager.Instance.Backpack != null && BackpackManager.Instance.Backpack.Owner == this)
        {
            BackpackManager.Instance.HideBackpack();
        }
        else
        {
            BackpackManager.Instance.SpawnBackpack(this);
        }
    }

    /// <summary>
    /// TODO: change <see cref="sprite"/> to a list of sprites,
    /// select a sprite depending on current character status (healthy, sick, tired, etc.).
    /// </summary>
    public Sprite GetSprite(SpriteType spriteType)
    {
        return sprites[spriteType];
    }
}
