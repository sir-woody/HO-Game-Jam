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
        Empty = -1,
        Equipement = 10,

        CandyBar = 20,
        Food = 30,
        Herbs = 40,

        FirstAidKit = 50,
        BandageAndPills = 60,
    }

    [SerializeField]
    private Backpack backpackPrefab = null;
    [SerializeField]
    private RectTransform backpackSlot = null;
    [SerializeField]
    private GraphicRaycaster raycaster = null;

    [SerializeField]
    private ItemSerializedDictionary itemPrefabs = null;

    public Backpack Backpack { get; private set; }


    public void SpawnBackpack(Character character)
    {
        /// Hide previously shown backpack
        HideBackpack();
        Backpack = Instantiate(backpackPrefab, backpackSlot, false);
        Backpack.Show(character, raycaster);
    }
    public void HideBackpack()
    {
        if (Backpack != null)
        {
            Destroy(Backpack.gameObject);
            Backpack = null;
        }
    }

    public Item SpawnItem(Character character, ItemType itemType)
    {
        Item item = Instantiate(itemPrefabs[itemType], character.transform, false);
        character.Equip(item);
        return item;
    }
    public Item SpawnItem(ItemType itemType)
    {
        return Instantiate(itemPrefabs[itemType], backpackSlot.transform, false);
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
    //public void UseItem(Item item, Character character)
    //{
    //    item.Use();
    //}
}