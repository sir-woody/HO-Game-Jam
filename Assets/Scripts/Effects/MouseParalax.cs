using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class MouseParalax : MonoBehaviour
{
    [SerializeField]
    private Vector2 extends = Vector2.zero;
    [SerializeField]
    private bool inverse = false;
    [SerializeField]
    private RectTransform extendsRect = null;
    [SerializeField]
    private RectTransform extendsReferenceRect = null;
    [SerializeField]
    private RectTransform movedObject = null;
    [SerializeField]
    private float lerpRatio = 0.3f;

    private new Camera camera;

    private void Awake()
    {
        camera = Camera.main;
    }

    private void Update()
    {
        Vector2 mousePosition = (Vector2)this.camera.ScreenToViewportPoint(Input.mousePosition) * 2 - Vector2.one;
        mousePosition.x = Mathf.Sin(Mathf.Clamp(mousePosition.x, -1, 1) * Mathf.PI * 0.5f);
        mousePosition.y = Mathf.Sin(Mathf.Clamp(mousePosition.y, -1, 1) * Mathf.PI * 0.5f);
        Vector2 paralax;
        if (extendsRect == null || extendsReferenceRect == null)
        {
            paralax = Vector2.Scale(this.extends, mousePosition);
        }
        else
        {
            paralax = Vector2.Scale((extendsRect.rect.size - extendsReferenceRect.rect.size) * 0.5f, mousePosition);
        }
        movedObject.localPosition = Vector2.Lerp(movedObject.localPosition, inverse ? -paralax : paralax, lerpRatio);
    }
}
