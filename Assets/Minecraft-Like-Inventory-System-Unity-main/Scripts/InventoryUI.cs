using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    
    public static InventoryItem carriedItem;
    private Item_Manager itemManager;

    [SerializeField] public InventorySlot[] inventorySlots; //1번 슬롯 배열
    [SerializeField] private InventorySlot[] hotbarSlots; //2번 슬롯 배열

    // 0: HEAD, 1: CHEST, 2: LEGGINGS, 3: FEET, 4: WEAPON, 5: Accessories
    [SerializeField] private InventorySlot[] equipmentSlots;

    [SerializeField] private Transform draggablesTransform;
    [SerializeField] private InventoryItem itemPrefab;

    [Header("Item List")]
    
    [SerializeField] private List<Original_Item> item_list;
    [SerializeField] private InventoryItem[] itemSet;

    
    //[SerializeField] private Button giveItemBtn;

    
    
    void Awake()
    {
        

        //giveItemBtn.onClick.AddListener(delegate { SpawnInventoryItem(); });
        // inventory.instance.GetInv_Main()
        itemSet = new InventoryItem[inventorySlots.Length];
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

    private void SetEmptyItem()
    {
        for(int i = 0; i < inventorySlots.Length; i++)
        {
            InventoryItem item = Instantiate(itemPrefab, inventorySlots[i].transform);
            item.gameObject.SetActive(false);

            itemSet[i] = item;

            //따로 배열 만들고 넣고 비활성화 그리고 필요할때마다 해당 인덱스 활성화
        }
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

        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (inventorySlots[i].myItem == null)
            {
                //Debug.Log("SpawnCollidedItem : " + item.itemIcon);
                //itemList.Add(item);

                if (Inventory.instance.inv_Slot[i] != null)
                {
                    item = Inventory.instance.inv_Slot[i];
                    Debug.Log(Inventory.instance.inv_Slot[i].name);

                    if(itemSet[i] == null)
                    {
                        itemSet[i] = Instantiate(itemPrefab, inventorySlots[i].transform);
                        itemSet[i].Initialize(item, inventorySlots[i]);
                        break;

                    }
                }
      
            }
        }
    }

    public void SetInventory()
    {
        //inventory.instance.GetInv_Main()[0].

        ItemComponent[] inv = Inventory.instance.GetInv_Main();
        int invLength = Inventory.instance.GetInv_Main().Length;


        for(int i = 0; i < invLength; i++)
        {
            if(inv[i] != null)
            {
                SpawnCollidedItem(Inventory.instance.GetInv_Main()[i]);
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