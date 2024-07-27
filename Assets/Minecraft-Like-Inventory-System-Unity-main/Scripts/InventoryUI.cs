using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    
    public static InventoryItem carriedItem;
    private Item_Manager itemManager;

    [SerializeField] public InventorySlot[] inventorySlots; //1번 슬롯 배열
    [SerializeField] public InventorySlot[] hotbarSlots; //2번 슬롯 배열

    // 0: HEAD, 1: CHEST, 2: LEGGINGS, 3: FEET, 4: WEAPON, 5: Accessories
    [SerializeField] public InventorySlot[] equipmentSlots; //퍼블릭 마음이 너무 아픕니다

    [SerializeField] private Transform draggablesTransform;
    [SerializeField] private InventoryItem itemPrefab;

    [Header("Item List")]
    
    [SerializeField] private List<Original_Item> item_list;
    [SerializeField] private InventoryItem[] itemSet;
    [SerializeField] private InventoryItem[] hotitemSet;

    //[SerializeField] private Button giveItemBtn;

    void Awake()
    {
        
        itemSet = new InventoryItem[inventorySlots.Length];
        hotitemSet = new InventoryItem[hotbarSlots.Length];
        //SetEmptyItem();
    }

    private void OnEnable()
    {
        SetInventory();
    }


    private void Start()
    {
        Debug.Log("개똥멍청이조영준" + Item_Dictionary.item_dictionary.Count);
        Add_All_Item();

      
    }
    private void Add_All_Item()
    {
        foreach (int key in Item_Dictionary.item_dictionary.Keys)
        {
            item_list.Add(Item_Dictionary.item_dictionary[key]);
        }
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
                    Debug.Log("Unequipped helmet on " + tag);
                }
                else
                {
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

    public void SpawnCollidedItem(ItemComponent item)
    {
        if (item == null)
        {
            Debug.LogError("ItemComponent is null");
            return;
        }

         //Hotbar 슬롯에서 빈 슬롯을 찾아 아이템 배치
        for (int i = 0; i < hotbarSlots.Length; i++)
        {
            if (hotbarSlots[i].myItem == null)
            {
                if (Inventory.instance.Hotbar_Slot[i] != null)
                {
                    var tempItem = Inventory.instance.Hotbar_Slot[i];
                    //Debug.Log(tempItem.name);
        
                    if (hotitemSet.Length > i && hotitemSet[i] == null)
                    {
                        hotitemSet[i] = Instantiate(itemPrefab, hotbarSlots[i].transform);
                        hotitemSet[i].Initialize(tempItem, hotbarSlots[i]);
                        return; // 아이템을 배치했으므로 종료
                    }
                }
            }
        }


        //아마도 itemset에 대한 처리 필요
        // Inventory 슬롯에서 빈 슬롯을 찾아 아이템 배치
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (inventorySlots[i].myItem == null)
            {
                if (Inventory.instance.inv_Slot[i] != null)
                {
                    var tempItem = Inventory.instance.inv_Slot[i];
                    Debug.Log("Tlqkf" +tempItem.name);

                    if (itemSet.Length > i && itemSet[i] == null)
                    {
                        itemSet[i] = Instantiate(itemPrefab, inventorySlots[i].transform);
                        itemSet[i].Initialize(tempItem, inventorySlots[i]);
                        return; // 아이템을 배치했으므로 종료
                    }
                }
            }
        }
    }


    public void SetInventory()
    {
        //inventory.instance.GetInv_Main()[0].

        ItemComponent[] inv = Inventory.instance.inv_Slot;
        ItemComponent[] hot_inv = Inventory.instance.Hotbar_Slot;
        int invLength = Inventory.instance.inv_Slot.Length;
        int hot_invLength = Inventory.instance.Hotbar_Slot.Length;


        for (int i = 0; i < hot_invLength; i++)
        {
            if (hot_inv[i] != null)
            {
                SpawnCollidedItem(hot_inv[i]);
            }

        }

        for (int i = 0; i < invLength; i++)
        {
            if(inv[i] != null)
            {
                //Debug.Log("invindex : " + i);
                SpawnCollidedItem(inv[i]); 
            }

        }

    }

    
    public void AddItemToInventory(InventoryItem item)
    {
        if (item == null)
        {
            Debug.LogError("InventoryItem is null.");
            return;
        }

        if(Inventory.instance.Hotbar_Slot.Length != 0)
        {

        }


        if(Inventory.instance.inv_Slot.Length != 0)
        {
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
        }


        

        Debug.LogWarning("No empty slot found for the item.");
    }
}