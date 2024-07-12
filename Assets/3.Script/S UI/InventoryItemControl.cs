using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventoryItemControl : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private Image image;

    [HideInInspector] public Transform tm;

    [Header("µð¹ö±×")]
    public bool click_tr = false;
    [SerializeField] private bool start_tr = false;

    public void OnBeginDrag(PointerEventData eventData)
    {
        start_tr = !(Input.GetMouseButton(0) && Input.GetMouseButton(1));

        if (start_tr)
        {
            tm = this.transform.parent;

            this.transform.SetParent(this.transform.root);

            this.transform.SetAsLastSibling();

            image.raycastTarget = false;

            click_tr = true;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (start_tr)
        {
            this.transform.position = Input.mousePosition;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (start_tr)
        {
            this.transform.SetParent(tm);

            image.raycastTarget = true;

            click_tr = false;
        }
    }
}