using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    private List<Item> inventoryItems = new List<Item>();
    private List<ItemComponent> itemComponents = new List<ItemComponent>();

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

    public void AddItem(Item item)
    {
        inventoryItems.Add(item);
        UpdateInventoryUI();
    }

    public void AddItemComponent(ItemComponent itemComponent)
    {
        itemComponents.Add(itemComponent);
        UpdateInventoryUI();
    }

    public List<Item> GetInventoryItems()
    {
        return inventoryItems;
    }

    public List<ItemComponent> GetItemComponents()
    {
        return itemComponents;
    }

    private void UpdateInventoryUI()
    {
        if (InventoryUI.Instance != null && InventoryUI.Instance.isActiveAndEnabled)
        {
            InventoryUI.Instance.UpdateUI(inventoryItems, itemComponents);
        }
    }
}
