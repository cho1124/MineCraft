using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Slot_item : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] Image image;

    [HideInInspector] public Transform tm;

    bool start_tr = false;

    public void OnBeginDrag(PointerEventData eventData)
    {
        start_tr = !(Input.GetMouseButton(1) || Input.GetMouseButton(2));

        if (start_tr)
        {
            tm = this.transform.parent;

            this.transform.SetParent(this.transform.root);

            this.transform.SetAsLastSibling();

            image.raycastTarget = false;

            Inventory_manager.instance.click_tr = true;
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

            Inventory_manager.instance.click_tr = false;
        }
    }
}