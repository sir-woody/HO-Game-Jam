using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemFrame : MonoBehaviour, IPointerDownHandler
{
    private class GrabData
    {
        public Vector3 offset;
        public ItemFrame emptyFramePlaceholder;
    }

    [SerializeField]
    private Image itemImage = null;
    [SerializeField]
    private CanvasGroup canvasGroup = null;
    [SerializeField]
    private LayoutElement layoutElement = null;
    [SerializeField]
    private float lerpRatio = 0.3f;

    private Item item = null;
    private Backpack backpack = null;
    private GraphicRaycaster raycaster;
    private GrabData grab = null;
    private bool draggable;
    private bool clickable => !draggable && TeamManager.Instance.CanEquip();

    public Image ItemImage => itemImage;

    public void Initialize(Backpack backpack, Item item, GraphicRaycaster raycaster, bool draggable = true)
    {
        this.backpack = backpack;
        this.item = item;
        this.raycaster = raycaster;
        this.draggable = draggable;
        if (item == null)
        {
            canvasGroup.blocksRaycasts = false;
            canvasGroup.alpha = 0;
        }
        else
        {
            ItemImage.sprite = item.GetSprite();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!draggable)
        {
            eventData.position = transform.position;
        }

        if (grab != null)
        {
            ReturnFromGrab();
        }
        grab = new GrabData()
        {
            offset = Camera.main.ScreenToWorldPoint(eventData.position) - transform.position,
            emptyFramePlaceholder = backpack.AddEmptyFrameAt(transform.GetSiblingIndex()),
        };
        canvasGroup.blocksRaycasts = false;
        layoutElement.ignoreLayout = true;
    }

    private void Update()
    {
        if (!clickable)
        {
            ItemImage.GetComponent<Image>().color = new Color(60, 60, 60);
        }
        else
        {
            ItemImage.GetComponent<Image>().color = Color.white;
        }

        if (grab != null)
        {
            if (draggable)
            {
                if (Input.GetMouseButtonUp(0) == true)
                {
                    Vector3 mousePosition = Input.mousePosition;
                    List<RaycastResult> results = new List<RaycastResult>();
                    raycaster.Raycast(new PointerEventData(null) { position = mousePosition }, results);

                    foreach (RaycastResult item in results)
                    {
                        Seat seat = item.gameObject.GetComponent<Seat>();
                        if (seat != null && seat.Character != null)
                        {
                            if (seat.Character == this.item.Owner)
                            {
                                if (this.item.CanUse())
                                {
                                    Debug.Log($"Character {seat.Character.name} tries to use the item {this.item.name}");
                                    this.item.UseOn(seat.Character);
                                    RemoveFromBackpack();
                                }
                                else
                                {
                                    CancelGrab();
                                }
                            }
                            else
                            {
                                if (seat.Character.CanEquip())
                                {
                                    Debug.Log($"Moved item {this.item.name} from {this.item.Owner.name} to {seat.Character.name}");
                                    BackpackManager.Instance.MoveItem(this.item.Owner, seat.Character, this.item);
                                    RemoveFromBackpack();
                                }
                                else
                                {
                                    CancelGrab();
                                }
                            }
                            return;
                        }
                    }
                }

                if (Input.GetMouseButton(0) == false)
                {
                    CancelGrab();
                }
                else
                {
                    Vector3 mousePosition = Input.mousePosition;
                    Vector3 position = Camera.main.ScreenToWorldPoint(mousePosition);
                    position.z = 0;
                    transform.position = Vector3.Lerp(transform.position, position, lerpRatio);
                }
            }
            else if(clickable)
            {
                TeamManager.Instance.Equip(this.item);
                RemoveFromBackpack();
                ReturnFromGrab();
            }
            else
            {
                CancelGrab();
            }
        }
    }

    private void CancelGrab()
    {
        Vector3 position = grab.emptyFramePlaceholder.transform.position;
        position.z = 0;
        transform.position = Vector3.Lerp(transform.position, position, lerpRatio);
        if (Vector3.Distance(transform.position, position) < 0.1f)
        {
            ReturnFromGrab();
        }
    }

    private void ReturnFromGrab()
    {
        backpack.RemoveEmptyFrame(grab.emptyFramePlaceholder);
        layoutElement.ignoreLayout = false;
        canvasGroup.blocksRaycasts = true;
        grab = null;
    }
    private void RemoveFromBackpack()
    {
        print("RemoveFromBackpack");
        backpack.RemoveEmptyFrame(grab.emptyFramePlaceholder);
        Destroy(this.gameObject);
    }
}