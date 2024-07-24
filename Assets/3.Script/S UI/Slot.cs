using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerUpHandler
{
    // ========== Inspector public ==========

    public Image item_icon = null;
    public ItemInfo iteminfo_class = null;
    public int slotnum = 0;

    // ========== Private ==========

    private Color slot_color;
    private Image slot_image = null;
    private float slot_color_value = 0.2f;
    private Vector3 originalPosition;
    private Transform originalParent;
    private bool start_tr = false;

    private void Awake()
    {
        slot_image = this.GetComponent<Image>();
        if (item_icon == null)
        {
            item_icon = GetComponentInChildren<Image>();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        SlotColor(slot_color_value);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        SlotColor(0);
    }

    private void OnDisable()
    {
        SlotColor(0);
    }

    private void SlotColor(float value)
    {
        slot_color = slot_image.color;
        slot_color.a = value;
        slot_image.color = slot_color;
    }

    public void UpdateSlotUI()
    {
        if (iteminfo_class != null)
        {
            item_icon.sprite = iteminfo_class.item_image;
            item_icon.gameObject.SetActive(true);
        }
        else
        {
            item_icon.gameObject.SetActive(false);
        }
    }

    public void RemoveSlot()
    {
        iteminfo_class = null;
        item_icon.gameObject.SetActive(false);
    }

    public void SetItem(ItemInfo newItemInfo)
    {
        iteminfo_class = newItemInfo;
        UpdateSlotUI();
    }

    public ItemInfo GetItem()
    {
        return iteminfo_class;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        bool isUse = iteminfo_class != null && iteminfo_class.ItemUse();
        if (isUse)
        {
            RemoveSlot();
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        start_tr = !(Input.GetMouseButton(0) && Input.GetMouseButton(1) || Input.GetMouseButton(2));

        if (start_tr)
        {
            item_icon.raycastTarget = false;
            originalPosition = item_icon.transform.localPosition;
            originalParent = item_icon.transform.parent;

            item_icon.transform.SetParent(this.transform.root);
            item_icon.transform.SetAsLastSibling();
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (start_tr)
        {
            item_icon.transform.position = Input.mousePosition;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (start_tr)
        {
            item_icon.raycastTarget = true;

            GameObject newParentObject = eventData.pointerEnter;

            if (newParentObject != null && newParentObject.GetComponent<Slot>() != null)
            {
                Slot newSlot = newParentObject.GetComponent<Slot>();

                // 현재 슬롯에서 새로운 슬롯으로 아이템 교환
                ItemInfo draggedItemInfo = GetItem();
                ItemInfo newSlotItemInfo = newSlot.GetItem();

                // 아이템이 둘 다 있는 상태에서 서로 교환
                newSlot.SetItem(draggedItemInfo);
                SetItem(newSlotItemInfo);

                item_icon.transform.SetParent(originalParent);
                item_icon.transform.localPosition = originalPosition;

                newSlot.UpdateSlotUI();
                UpdateSlotUI();
            }
            else
            {
                // 유효한 슬롯이 아닌 경우, 아이템을 원래 자리로 되돌림
                item_icon.transform.SetParent(originalParent);
                item_icon.transform.localPosition = originalPosition;
            }
        }
    }
}
