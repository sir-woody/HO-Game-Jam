using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class BackpackManager : Singleton<BackpackManager>
{
    [Serializable]
    public class ItemSerializedDictionary : SerializedDictionary<ItemType, Item> { }

    public enum ItemType
    {
        Equipement,

        CandyBar,
        Food,
        Herbs,

        FirstAidKit,
        BandageAndPills,
    }

    [SerializeField]
    private Backpack backpackPrefab = null;
    [SerializeField]
    private RectTransform backpackSlot = null;
    [SerializeField]
    private GraphicRaycaster raycaster = null;

    [SerializeField]
    private ItemSerializedDictionary itemPrefabs = null;

    public void SpawnBackpack(Character character)
    {
        Backpack backpack = Instantiate(backpackPrefab, backpackSlot, false);
        backpack.Show(character, raycaster);
    }

    [Button]
    private void ShowRandom()
    {
        Character character = TeamManager.Instance.characters.GetRandom();
        SpawnBackpack(character);
    }

    public Item SpawnItem(Character character, ItemType itemType)
    {
        Item item = Instantiate(itemPrefabs[itemType], character.transform, false);
        character.Equip(item);
        return item;
    }
    public void DestroyItem(Character character, Item item)
    {
        character.Unequip(item);
        Destroy(item.gameObject);
    }
    public void MoveItem(Character from, Character to, Item item)
    {
        from.Unequip(item);
        to.Equip(item);
    }

}