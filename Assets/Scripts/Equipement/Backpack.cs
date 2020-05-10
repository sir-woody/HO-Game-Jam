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
    [SerializeField]
    private float lerpRatio = 0.2f;

    private List<ItemFrame> itemFrames = new List<ItemFrame>();

    public Character Owner { get; private set; }

    private void Update()
    {
        transform.localPosition = Vector3.Lerp(transform.localPosition, Vector3.zero, lerpRatio);
    }

    public void ShowForCharacter(Character character, GraphicRaycaster raycaster)
    {
        this.Owner = character;
        backpackOwnerImage.sprite = character.GetSprite(Character.SpriteType.FrameDefault);
        Item[] items = character.GetItems();
        InitializeItems(items, raycaster, true);
    }

    public void ShowWithoutCharacter(Item[] items, GraphicRaycaster raycaster, bool draggable)
    {
        InitializeItems(items, raycaster, draggable);
    }

    private void InitializeItems(Item[] items, GraphicRaycaster raycaster, bool draggable)
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