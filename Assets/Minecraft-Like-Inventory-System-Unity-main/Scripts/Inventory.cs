using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance; // ½Ì±ÛÅæ ÆÐÅÏ
    public static InventoryItem carriedItem;
    private Item_Manager itemManager;

    [SerializeField] private InventorySlot[] inventorySlots; //1¹ø ½½·Ô ¹è¿­
    [SerializeField] private InventorySlot[] hotbarSlots; //2¹ø ½½·Ô ¹è¿­

    // 0: HEAD, 1: CHEST, 2: LEGGINGS, 3: FEET, 4: WEAPON, 5: Accessories
    [SerializeField] private InventorySlot[] equipmentSlots;

    [SerializeField] private Transform draggablesTransform;
    [SerializeField] private InventoryItem itemPrefab;

    [Header("Item List")]
    [SerializeField] private Item[] items;
    [SerializeField] private List<Original_Item> item_list;

    [Header("Debug")]
    [SerializeField] private Button giveItemBtn;

    [SerializeField] private List<ItemComponent> itemList = new List<ItemComponent>();
    


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

        giveItemBtn.onClick.AddListener(delegate { SpawnInventoryItem(); });
        // inventory.instance.GetInv_Main()
        //SetEmptyItem();




    }

    private void OnEnable()
    {
        SetInventory();
    }


    private void Start()
    {
        Debug.Log("°³¶Ë¸ÛÃ»ÀÌÁ¶¿µÁØ" + Item_Dictionary.item_dictionary.Count);
        Add_All_Item();

       

    }

    private void SetEmptyItem()
    {
        for(int i = 0; i < inventorySlots.Length; i++)
        {
            Instantiate(itemPrefab, inventorySlots[i].transform);
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

    public void SpawnInventoryItem(Item item = null)
    {
        Item _item = item ?? PickRandomItem();

        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (inventorySlots[i].myItem == null)
            {
                


                Instantiate(itemPrefab, inventorySlots[i].transform).Initialize(_item, inventorySlots[i]);
                break;
            }
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
                Debug.Log("SpawnCollidedItem : " + item.itemIcon);
                //itemList.Add(item);

                if(inventorySlots[i].transform.childCount == 0)
                {
                    Instantiate(itemPrefab, inventorySlots[i].transform).Initialize(item, inventorySlots[i]);
                }

                
                break;
            }
        }
    }

    public void SetInventory()
    {
        //inventory.instance.GetInv_Main()[0].

        ItemComponent[] inv = inventory.instance.GetInv_Main();
        int invLength = inventory.instance.GetInv_Main().Length;



        for(int i = 0; i < invLength; i++)
        {
            if(inv[i] != null)
            {
                SpawnCollidedItem(inventory.instance.GetInv_Main()[i]);
            }


        }


    }

    private Item PickRandomItem()
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