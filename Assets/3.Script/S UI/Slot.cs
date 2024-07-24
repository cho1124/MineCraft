using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler
{
    // ========== Inspector public ==========

    public Image item_icon = null;
    public ItemInfo iteminfo_class = null;
    public int slotnum = 0;

    // ========== Inspector private ==========

    private Color slot_color;
    private Image slot_image = null;
    private float slot_color_value = 0.2f;

    private void Awake()
    {
        slot_image = this.GetComponent<Image>();
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
        item_icon.sprite = iteminfo_class.item_image;

        item_icon.gameObject.SetActive(true);
    }

    public void RemoveSlot()
    {
        iteminfo_class = null;

        item_icon.gameObject.SetActive(false);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        bool isUse = iteminfo_class.ItemUse();
    }
}