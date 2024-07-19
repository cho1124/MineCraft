using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventoryItemControl : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    // ========== Inspector public ==========

    [SerializeField] private Image image = null;

    public static bool test_tr = false;

    // ========== Inspector private ==========

    [HideInInspector] private Inventory Inventory_class = null;

    [HideInInspector] public Transform tm = null;

    private bool start_tr = false;

    private void Awake()
    {
        Inventory_class = FindObjectOfType<Inventory>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        start_tr = !(Input.GetMouseButton(0) && Input.GetMouseButton(1) || Input.GetMouseButton(2));

        if (start_tr)
        {
            test_tr = false; // test
            image.raycastTarget = false;
            Inventory_class.item_click_tr = true;

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
            Inventory_class.item_click_tr = false;

            if (test_tr) // test
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