using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class InventoryItem : MonoBehaviour, IPointerClickHandler
{
    Image itemIcon;
    InventoryUI Inven;
    Text itemCount;

    public Equipment_Type equip_type = Equipment_Type.NONE;
   
    public CanvasGroup canvasGroup { get; private set; }

    //public Item myItem { get; set; }
    public ItemComponent itemComponent { get; set; }
    
    public InventorySlot activeSlot { get; set; }

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        itemIcon = GetComponent<Image>();
        Inven = GetComponentInParent<InventoryUI>();
        itemCount = GetComponentInChildren<Text>();
    }

    public void Initialize(Item item, InventorySlot parent)
    {
        activeSlot = parent;
        activeSlot.myItem = this;
        //myItem = item;
        itemIcon.sprite = item.sprite;
    }

    public void Initialize(ItemComponent itemComponent, InventorySlot parent)
    {
        if (itemComponent == null || parent == null)
        {
            Debug.LogError("itemComponent or parent is null");
            return;
        }

        

        activeSlot = parent;
        activeSlot.myItem = this;

        if (Enum.TryParse(itemComponent.SetEquipType(), out Equipment_Type equipment_Type))
        {
            equip_type = equipment_Type;
        }

        //equip_type = 
        this.itemComponent = itemComponent;
        itemIcon.sprite = itemComponent.itemIcon;

        if (itemComponent.StackCurrent <= 1)
        {
            itemCount.gameObject.SetActive(false);
        }
        else
        {
            itemCount.gameObject.SetActive(true);
            itemCount.text = $"{itemComponent.StackCurrent}";
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Left)
        {
            Inven.SetCarriedItem(this);
        }
    }
} 