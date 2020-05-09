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
        public Transform previousParent;
        public int previousParentSiblingIndex;
        public Vector3 offset;
    }

    [SerializeField]
    private Image itemImage = null;
    [SerializeField]
    private CanvasGroup canvasGroup = null;

    private Item item = null;
    private GraphicRaycaster raycaster;
    private GrabData grab = null;

    public Image ItemImage => itemImage;

    public void Initialize(Item item, GraphicRaycaster raycaster)
    {
        this.item = item;
        this.raycaster = raycaster;
        ItemImage.sprite = item.GetSprite();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        grab = new GrabData()
        {
            previousParent = transform.parent,
            previousParentSiblingIndex = transform.GetSiblingIndex(),
            offset = transform.parent.InverseTransformPoint(Camera.main.ScreenToWorldPoint(eventData.position)),
        };
        canvasGroup.blocksRaycasts = false;
        transform.SetParent(null);
    }

    private void Update()
    {
        if (grab != null)
        {
            Vector3 mousePosition = Input.mousePosition;
            transform.position = this.grab.previousParent.InverseTransformPoint(Camera.main.ScreenToWorldPoint(mousePosition)) + this.grab.offset;
            if (Input.GetMouseButton(0) == false)
            {
                transform.SetParent(grab.previousParent, false);
                transform.SetSiblingIndex(grab.previousParentSiblingIndex);
                List<RaycastResult> results = new List<RaycastResult>();
                raycaster.Raycast(new PointerEventData(EventSystem.current) { position = mousePosition }, results);

                foreach (var item in results)
                {
                    Debug.Log($"item: {item.gameObject.name}");
                }

                canvasGroup.blocksRaycasts = true;
                grab = null;
            }
        }
    }
}