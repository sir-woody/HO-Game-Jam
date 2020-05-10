using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TooltipController : MonoBehaviour,
                             IPointerEnterHandler,
                             IPointerExitHandler
{
    
    [Multiline, SerializeField] string description;
    [SerializeField] GameObject toolTipPrefab;
    GameObject toolTip;
    [SerializeField] float tooltipDelay = 1f;
    float tooltipDelayCounter;
    TMP_Text textField;
    bool show;
    private float lerpRatio = 0.3f;

    void Start()
    {
        toolTip = Instantiate(toolTipPrefab, GameplayManager.Instance.tooltipParent);
        toolTip.transform.SetAsLastSibling();
        textField = toolTip.GetComponentInChildren<TMP_Text>();
        textField.text = description;
    }

    void Update()
    {
        if (show)
        {
            if (tooltipDelay > tooltipDelayCounter)
            {
                tooltipDelayCounter += Time.deltaTime;
            }
            else
            {
                toolTip?.SetActive(true);

                Vector3 mousePosition = Input.mousePosition + GetOffset(Input.mousePosition, (RectTransform)toolTip.transform);
                Vector3 position = Camera.main.ScreenToWorldPoint(mousePosition) ;
                position.z = 0;
                toolTip.transform.position = Vector3.Lerp(toolTip.transform.position, position,  lerpRatio);
            }
        }
        else
        {
            toolTip?.SetActive(false);
        }
    }

    Vector3 GetOffset(Vector3 mousePosition, RectTransform transform)
    {
        float padding = 0;
        //left side of screen
        if (mousePosition.x < Screen.width / 2)
        {
            //top part of screen
            if(mousePosition.y<Screen.height/2)
            {
                return new Vector3(padding + transform.rect.width / 2, padding + transform.rect.height/ 2, 0);
            }
            //bottom part of screen
            else
            {
                return new Vector3(padding + transform.rect.width / 2, -padding - transform.rect.height / 2, 0);

            }
        }
        //right side of screen
        else
        {
            //top part of screen
            if (mousePosition.y < Screen.height / 2)
            {
                return new Vector3(-padding - transform.rect.width / 2, padding + transform.rect.height / 2, 0);

            }
            //bottom part of screen
            else
            {
                return new Vector3(-padding - transform.rect.width / 2, -padding - transform.rect.height / 2, 0);

            }
        }
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        print("OnPointerEnter");
        show = true;
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        print("OnPointerExit");
        show = false;
        tooltipDelayCounter = 0f;
    }
}
