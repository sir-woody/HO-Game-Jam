using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class Backpack : MonoBehaviour
{
    [SerializeField]
    private ItemFrame itemFramePrefab = null;
    [SerializeField]
    private Image backpackOwnerImage = null;
    [SerializeField]
    private GridLayoutGroup gridLayoutGroup = null;

    private List<ItemFrame> itemFrames = new List<ItemFrame>();

    public Character Owner { get; private set; }
    public void Show(Character character, GraphicRaycaster raycaster)
    {
        this.Owner = character;
        backpackOwnerImage.sprite = character.GetSprite(Character.SpriteType.FrameDefault);
        Item[] items = character.GetItems();
        Show(items, raycaster);
    }

    public void Show(Item[] items, GraphicRaycaster raycaster, bool draggable = true)
    {
        foreach (Item item in items)
        {
            ItemFrame frame = Instantiate(itemFramePrefab, gridLayoutGroup.transform, false);
            frame.Initialize(this, item, raycaster, draggable);
            itemFrames.Add(frame);
        }
    }

    public bool IsEmpty()
    {
        return !itemFrames.Any() || itemFrames.All(x=>x==null);
    }

    public void Hide()
    {
        foreach (ItemFrame frame in itemFrames)
        {
            Destroy(frame.gameObject);
        }
        itemFrames.Clear();
    }

    public ItemFrame AddEmptyFrameAt(int siblingIndex)
    {
        ItemFrame frame = Instantiate(itemFramePrefab, gridLayoutGroup.transform, false);
        frame.Initialize(this, null, null);
        frame.transform.SetSiblingIndex(siblingIndex);
        return frame;
    }
    public void RemoveEmptyFrame(ItemFrame emptyItemFrame)
    {
        Destroy(emptyItemFrame.gameObject);
    }
}