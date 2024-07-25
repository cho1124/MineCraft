using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public static InventoryUI Instance;

    [SerializeField] private InventorySlot[] inventorySlots;
    [SerializeField] private InventoryItem itemPrefab;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnEnable()
    {
        UpdateUI(InventoryManager.Instance.GetInventoryItems(), InventoryManager.Instance.GetItemComponents());
    }

    public void UpdateUI(List<Item> inventoryItems, List<ItemComponent> itemComponents)
    {
        // Clear existing UI
        foreach (InventorySlot slot in inventorySlots)
        {
            if (slot.myItem != null)
            {
                Destroy(slot.myItem.gameObject);
                slot.myItem = null;
            }
        }

        // Populate inventory slots with items
        int i = 0;
        foreach (Item item in inventoryItems)
        {
            if (i < inventorySlots.Length)
            {
                InventoryItem newItem = Instantiate(itemPrefab, inventorySlots[i].transform);
                newItem.Initialize(item, inventorySlots[i]);
                i++;
            }
        }

        // Populate inventory slots with item components if needed
        // Add similar logic for item components if required
    }
}
