using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventoryItem : MonoBehaviour, IPointerClickHandler
{
    Image itemIcon;
    public CanvasGroup canvasGroup { get; private set; }

    public Item myItem { get; set; }
    public InventorySlot activeSlot { get; set; }

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        itemIcon = GetComponent<Image>();
    }

    public void Initialize(Item item, InventorySlot parent)
    {
        if (parent == null)
        {
            Debug.LogError("Parent InventorySlot is null.");
            return;
        }

        if (item == null)
        {
            Debug.LogError("Item is null.");
            return;
        }

        activeSlot = parent;
        activeSlot.myItem = this;
        myItem = item;

        if (itemIcon == null)
        {
            itemIcon = GetComponent<Image>();
        }

        if (itemIcon != null)
        {
            itemIcon.sprite = item.sprite;
        }
        else
        {
            Debug.LogError("ItemIcon is null.");
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            Inventory.Singleton.SetCarriedItem(this);
        }
    }

    public void PickupItem()
    {
        if (Inventory.Singleton != null)
        {
            Debug.Log(this);

            Inventory.Singleton.AddItemToInventory(this);
            Destroy(gameObject); // 아이템을 먹었으므로 제거
        }
        else
        {
            Debug.LogError("Inventory.Singleton is null.");
        }
    }
}

/* using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventoryItem : MonoBehaviour, IPointerClickHandler
{
    Inventory inventory; // test

    Image itemIcon;
    public CanvasGroup canvasGroup { get; private set; }

    public Item myItem { get; set; }
    public InventorySlot activeSlot { get; set; }

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        itemIcon = GetComponent<Image>();
    }

    public void Initialize(Item item, InventorySlot parent)
    {
        activeSlot = parent;
        activeSlot.myItem = this;
        myItem = item;
        itemIcon.sprite = item.sprite;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Left)
        {
            Inventory.Singleton.SetCarriedItem(this);
        }
    }
} */