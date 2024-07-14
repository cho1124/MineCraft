using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventoryItemControl : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    // ========== Inspector public ==========

    public static bool test_tr = false;

    [SerializeField] private Image image = null;

    [Header("디버그")]
    public bool click_tr = false;
    [SerializeField] private bool start_tr = false;

    // ========== Inspector private ==========

    [HideInInspector] public Transform tm = null;

    public void OnBeginDrag(PointerEventData eventData)
    {
        start_tr = !(Input.GetMouseButton(0) && Input.GetMouseButton(1));

        if (start_tr)
        {
            image.raycastTarget = false;
            click_tr = true;
            test_tr = false;

            tm = this.transform.parent;

            this.transform.SetParent(this.transform.root);
            this.transform.SetAsLastSibling();
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
            image.raycastTarget = true;
            click_tr = false;

            if (test_tr)
            {
                this.transform.SetParent(tm);
            }
            else
            {
                Destroy(this.gameObject);
            }
        }
    }
}