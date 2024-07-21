using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{

    // ========== Inspector public ==========

    [Header("슬롯 색 조절")]
    [SerializeField] private float slot_color_value = 0.2f;

    // ========== Inspector private ==========

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
            InventoryItemControl.test_tr = true;

            GameObject obj = eventData.pointerDrag;

            InventoryItemControl item = obj.GetComponent<InventoryItemControl>();

            item.tm = this.transform;
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
}