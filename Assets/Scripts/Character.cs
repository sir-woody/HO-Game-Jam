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
        public Sprite dead;
    }
    public enum SpriteType
    {
        Idle,
        Hoover,
        Dead
    }

    public new string name;
    public string description;
    public List<SpriteGroup> sprites;
    Stat[] stats;
    TraitController TraitController;
    VoiceController VoiceController;

    bool isDead;
    [SerializeField] float baseSpeed = 1;
    [SerializeField] float minSpeed = .1f;
    [SerializeField] GameObject equipment;

    void Start()
    {
        stats = GetComponent<StatController>().stats;
        //traits = GetComponents<Trait>();
        VoiceController = GetComponent<VoiceController>();
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

    internal float GetCurrentSpeed() => Mathf.Clamp(baseSpeed * GetStatPercentage("Zdrowie") * GetStatPercentage("Energia"), minSpeed, 2 * baseSpeed);

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

            foreach (var item in GetItems().Where(x => x.continousEffect))
            {
                item.Use(this);
            }
        }
        GetItems().ToList().ForEach(Equip);

        if (GetStat("Zdrowie").IsDepleted()) Die();
    }

    private void Die()
    {
        isDead = true;

        //TODO Change sprite to gray,
        //Debug.LogError($"{nameof(Die)} not implemented");
    }

    public bool IsDead() => isDead;

    /// <summary>
    /// Character was clicked during <see cref="RestEvent"/>.
    /// Start a dialog
    /// </summary>
    public void OnCharacterClicked()
    {
        SoundManager.Instance.PlaySound(SoundManager.SoundType.Voice, VoiceController.GetOnClickSound());
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
            case SpriteType.Dead: return sprites[spriteIndex].dead;
            default: return null;
        }
    }
}
