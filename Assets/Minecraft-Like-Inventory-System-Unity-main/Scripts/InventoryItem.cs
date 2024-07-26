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

    public Equipment_Type equip_type = Equipment_Type.NONE;
   
    public CanvasGroup canvasGroup { get; private set; }

    public Item myItem { get; set; }
    public ItemComponent itemComponent { get; set; }
    
    public InventorySlot activeSlot { get; set; }

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        itemIcon = GetComponent<Image>();
        Inven = FindObjectOfType<InventoryUI>();
    }

    public void Initialize(Item item, InventorySlot parent)
    {
        activeSlot = parent;
        activeSlot.myItem = this;
        myItem = item;
        itemIcon.sprite = item.sprite;
    }

    public void Initialize(ItemComponent itemComponent, InventorySlot parent)
    {
        if (itemComponent == null || parent == null)
        {
            Debug.LogError("itemComponent or parent is null");
            return;
        }

        Debug.Log("Init : " + itemComponent.itemIcon);

        activeSlot = parent;
        activeSlot.myItem = this;

        if (Enum.TryParse(itemComponent.SetEquipType(), out Equipment_Type equipment_Type))
        {
            equip_type = equipment_Type;
        }

        //equip_type = 
        this.itemComponent = itemComponent;
        itemIcon.sprite = itemComponent.itemIcon;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Left)
        {
            Inven.SetCarriedItem(this);
        }
    }
} 