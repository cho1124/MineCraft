using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public static Inventory Singleton;
    public static InventoryItem carriedItem;
    private Item_Manager itemManager;


    [SerializeField] InventorySlot[] inventorySlots;
    [SerializeField] InventorySlot[] hotbarSlots;

    // 0: HEAD, 1: CHEST, 2: LEGGINGS, 3: FEET, 4: WEAPON, 5: Accessories
    [SerializeField] InventorySlot[] equipmentSlots;

    [SerializeField] Transform draggablesTransform;
    [SerializeField] InventoryItem itemPrefab;

    [Header("Item List")]
    [SerializeField] Item[] items;

    
    [Header("Debug")]
    [SerializeField] Button giveItemBtn;

    void Awake()
    {
        Singleton = this;
        //Item_Manager.Item_Dictionary.SpawnItem
        giveItemBtn.onClick.AddListener(delegate { SpawnInventoryItem(); });
    }

    void Update()
    {
        if (carriedItem == null) return;

        carriedItem.transform.position = Input.mousePosition;
    }

    public void SetCarriedItem(InventoryItem item)
    {
        if (carriedItem != null)
        {
            if (item.activeSlot.myTag != SlotTag.None && item.activeSlot.myTag != carriedItem.myItem.itemTag) return;
            item.activeSlot.SetItem(carriedItem);
        }

        if (item.activeSlot.myTag != SlotTag.None)
        {
            EquipEquipment(item.activeSlot.myTag, null);
        }

        carriedItem = item;
        carriedItem.canvasGroup.blocksRaycasts = false;
        item.transform.SetParent(draggablesTransform);
    }

    public void EquipEquipment(SlotTag tag, InventoryItem item = null)
    {
        switch (tag)
        {
            case SlotTag.Head:
                if (item == null)
                {
                    // Destroy item.equipmentPrefab on the Player Object;
                    Debug.Log("Unequipped helmet on " + tag);
                }
                else
                {
                    // Instantiate item.equipmentPrefab on the Player Object;
                    Debug.Log("Equipped " + item.myItem.name + " on " + tag);
                }
                break;
            case SlotTag.Chest:
                break;
            case SlotTag.Legs:
                break;
            case SlotTag.Feet:
                break;
            case SlotTag.Weapon:
                break;
            case SlotTag.Accessories:
                break;
        }
    }

    public void SpawnInventoryItem(Item item = null)
    {
        Item _item = item;
        if (_item == null)
        {
            _item = PickRandomItem();
        }

        for (int i = 0; i < inventorySlots.Length; i++)
        {
            // Check if the slot is empty
            if (inventorySlots[i].myItem == null)
            {
                Instantiate(itemPrefab, inventorySlots[i].transform).Initialize(_item, inventorySlots[i]);
                break;
            }
        }
    }

    Item PickRandomItem()
    {
        int random = Random.Range(0, items.Length);
        return items[random];
    }

    public void AddItemToInventory(InventoryItem item)
    {
        if (item == null)
        {
            Debug.LogError("InventoryItem is null.");
            return;
        }

        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (inventorySlots[i].myItem == null)
            {
                Debug.Log(item.name);
                item.transform.SetParent(inventorySlots[i].transform);
                item.Initialize(item.myItem, inventorySlots[i]);
                return;
            }
        }

        Debug.LogWarning("No empty slot found for the item.");
    }
}

/* using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public static Inventory Singleton;
    public static InventoryItem carriedItem;

    [SerializeField] InventorySlot[] inventorySlots;
    [SerializeField] InventorySlot[] hotbarSlots;

    // 0: Head, 1: Chest, 2: Legs, 3: Feet, 4: Weapon, 5: Accessories
    [SerializeField] InventorySlot[] equipmentSlots;

    [SerializeField] Transform draggablesTransform;
    [SerializeField] InventoryItem itemPrefab;

    [Header("Item List")]
    [SerializeField] Item[] items;

    [Header("Debug")]
    [SerializeField] Button giveItemBtn;

    void Awake()
    {
        Singleton = this;
        giveItemBtn.onClick.AddListener( delegate { SpawnInventoryItem(); } );
    }

    void Update()
    {
        if(carriedItem == null) return;

        carriedItem.transform.position = Input.mousePosition;
    }

    public void SetCarriedItem(InventoryItem item)
    {
        if(carriedItem != null)
        {
            if(item.activeSlot.myTag != SlotTag.None && item.activeSlot.myTag != carriedItem.myItem.itemTag) return;
            item.activeSlot.SetItem(carriedItem);
        }

        if(item.activeSlot.myTag != SlotTag.None)
        { EquipEquipment(item.activeSlot.myTag, null); }

        carriedItem = item;
        carriedItem.canvasGroup.blocksRaycasts = false;
        item.transform.SetParent(draggablesTransform);
    }

    public void EquipEquipment(SlotTag tag, InventoryItem item = null)
    {
        switch (tag)
        {
            case SlotTag.Head:
                if(item == null)
                {
                    // Destroy item.equipmentPrefab on the Player Object;
                    Debug.Log("Unequipped helmet on " + tag);
                }
                else
                {
                    // Instantiate item.equipmentPrefab on the Player Object;
                    Debug.Log("Equipped " + item.myItem.name + " on " + tag);
                }
                break;
            case SlotTag.Chest:
                break;
            case SlotTag.Legs:
                break;
            case SlotTag.Feet:
                break;
            case SlotTag.Weapon:
                break;
            case SlotTag.Accessories:
                break;
        }
    }

    public void SpawnInventoryItem(Item item = null)
    {
        Item _item = item;
        if(_item == null)
        { _item = PickRandomItem(); }

        for (int i = 0; i < inventorySlots.Length; i++)
        {
            // Check if the slot is empty
            if(inventorySlots[i].myItem == null)
            {
                Instantiate(itemPrefab, inventorySlots[i].transform).Initialize(_item, inventorySlots[i]);
                break;
            }
        }
    }
    Item PickRandomItem()
    {
        int random = Random.Range(0, items.Length);
        return items[random];
    }
} */