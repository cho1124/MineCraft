using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public InventorySlot[] HotBarSlots;
    public InventorySlot[] InventoryBarSlots;

    [SerializeField] private InventoryItem[] hotitemSet;
    [SerializeField] private InventoryItem[] invenitemSet;
    [SerializeField] private InventoryItem itemPrefab;

    private void Awake()
    {
        hotitemSet = new InventoryItem[HotBarSlots.Length];
        invenitemSet = new InventoryItem[InventoryBarSlots.Length];
    }

    private void Start()
    {
        if (Inventory.instance != null)
        {
            Inventory.instance.OnHotBarChanged += SetHotBar;
        }
        else
        {
            Debug.LogError("Inventory instance is null. Ensure that Inventory is properly initialized.");
        }
    }

    private void SetHotBar()
    {
        Debug.Log("HotBarChanged");

        for (int i = 0; i < HotBarSlots.Length; i++)
        {
            if (HotBarSlots[i] == null)
            {
                Debug.LogWarning($"HotBarSlot at index {i} is null.");
                continue;
            }

            if (Inventory.instance.inv_Slot == null)
            {
                Debug.LogError("Inventory slots are null. Ensure that inv_Slot array is initialized.");
                return;
            }

            if (Inventory.instance.inv_Slot[i] == null)
            {
                if (HotBarSlots[i].myItem != null)
                {
                    TryRemoveItem(HotBarSlots, hotitemSet, i, out InventoryItem removedItem);
                }
            }
            else
            {
                var tempItem = Inventory.instance.inv_Slot[i];
                TryPlaceItem(HotBarSlots, hotitemSet, i, tempItem, itemPrefab);
            }
        }
    }

    public bool TryPlaceItem(InventorySlot[] slots, InventoryItem[] itemSet, int index, ItemComponent tempItem, InventoryItem itemPrefab)
    {
        if (index >= slots.Length || index >= itemSet.Length)
        {
            Debug.LogError($"Index {index} is out of range for slots or itemSet array.");
            return false;
        }

        if (slots[index].myItem == null && itemSet[index] == null) //아무것도 없을때
        {
            itemSet[index] = Instantiate(itemPrefab, slots[index].transform);
            itemSet[index].Initialize(tempItem, slots[index]);
            return true;
        }
        else if(slots[index].myItem != null && itemSet[index] == null) 
        {
            //Debug.Log("ㅎㅎ");
            itemSet[index] = Instantiate(itemPrefab, slots[index].transform);
            itemSet[index].Initialize(tempItem, slots[index]);
            return true;
        }
        else
        {
            Debug.Log("shit");
            itemSet[index].Initialize(tempItem, slots[index]);
            return true;
        }
        
        
    }

    public bool TryRemoveItem(InventorySlot[] slots, InventoryItem[] itemSet, int index, out InventoryItem removedItem)
    {
        removedItem = null;

        if (index >= slots.Length || index >= itemSet.Length)
        {
            Debug.LogError($"Index {index} is out of range for slots or itemSet array.");
            return false;
        }

        if (slots[index].myItem != null && itemSet[index] != null)
        {
            removedItem = itemSet[index];
            itemSet[index] = null;
            slots[index].myItem = null;
            Destroy(removedItem.gameObject);
            return true;
        }
        return false;
    }
}
