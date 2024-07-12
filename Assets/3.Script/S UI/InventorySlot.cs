using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    private Color slot_color;
    private Image slot_image = null;

    private void Awake()
    {
        slot_image = this.GetComponent<Image>();
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (this.transform.childCount == 0)
        {
            GameObject obj = eventData.pointerDrag;

            InventoryItemControl item = obj.GetComponent<InventoryItemControl>();

            item.tm = this.transform;
        }
        else
        {
            // 아이템 중복 체크 로직 요청
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        SlotColor(0.2f);
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
}