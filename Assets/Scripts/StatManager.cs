using System;
using TMPro;
using UnityEngine;

public class StatManager : Singleton<StatManager>
{
    [Serializable]
    public class StatGroupSerializedDictionary : SerializedDictionary<Seat.SeatPosition, StatGroup> { }

    [SerializeField] private StatBar statBarPrefab;
    [Space]
    [SerializeField] private StatGroupSerializedDictionary statGroups;
    [SerializeField] private bool displayCharactersNames;

    private void Awake()
    {
        foreach (StatGroup container in statGroups.Values)
        {
            Clear(container);
        }
        Instance.Hide();
    }

    public void BindStats(Character character)
    {
        Seat.SeatPosition statContainerId = character.seatPosition;
        StatGroup statGroup = statGroups[statContainerId];
        if (displayCharactersNames)
        {
            statGroup.CharacterName.text = character.name;
            statGroup.CharacterName.gameObject.SetActive(true);
        }
        else
        {
            statGroup.CharacterName.gameObject.SetActive(false);
        }

        Stat[] stats = character.GetComponent<StatController>().stats;
        character.OnDeath += (x) =>
        {
            statGroup.CharacterFrame.sprite = character.GetSprite(Character.SpriteType.FrameDeath);
        };

        if (character.IsDead() == true)
        {
            statGroup.CharacterFrame.sprite = character.GetSprite(Character.SpriteType.FrameDeath);
        }
        else
        {
            statGroup.CharacterFrame.sprite = character.GetSprite(Character.SpriteType.FrameDefault);
        }

        foreach (Stat stat in stats)
        {
            StatBar statBar = Instantiate(statBarPrefab, statGroup.StatParent.transform);
            statBar.GetComponent<StatBar>().Initialize(stat);
        }
    }


    private void Clear(StatGroup statContainer)
    {
        foreach (RectTransform child in statContainer.StatParent.transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
