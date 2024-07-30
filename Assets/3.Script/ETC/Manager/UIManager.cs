using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public InventoryItem carriedItem;

    [Header("인벤토리 내부 슬롯")]
    public InventorySlot[] inventorySlots; //1번 슬롯 배열
    // 0: HEAD, 1: CHEST, 2: LEGGINGS, 3: FEET, 4: WEAPON, 5: Accessories
    public InventorySlot[] equipmentSlots; //퍼블릭 마음이 너무 아픕니다
    public CraftingSlot[] minicraftingSlots;

    [Header("인벤토리 외부 슬롯")]
    public InventorySlot[] HotBarSlots_Out;

    

    [Header("아이템 세터(디버깅용으로 임시로 보이는 것)")]
    [SerializeField] private InventoryItem[] hotitemSet;
    [SerializeField] private InventoryItem[] hotitemSet_inv;
    [SerializeField] private InventoryItem[] invenitemSet;
    
    [Header("ETC")]
    [SerializeField] private Transform draggablesTransform;
    [SerializeField] private InventoryItem itemPrefab;
    
    public CanvasGroup TrashCan;

    private void Awake()
    {

    }

    private void Start()
    {
        //hotitemSet = new InventoryItem[HotBarSlots_Out.Length];
        hotitemSet = new InventoryItem[HotBarSlots_Out.Length];
        invenitemSet = new InventoryItem[inventorySlots.Length];
        
       

        if (Inventory.instance != null)
        {

            Inventory.instance.OnChangedInv += SetHotBar;
            Inventory.instance.OnChangedInv += SetInventory;
        }
        else
        {
            Debug.LogError("Inventory instance is null. Ensure that Inventory is properly initialized.");
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
            if (item.activeSlot != null && item.activeSlot.myItem != null && item.activeSlot.myItem.equip_type != Equipment_Type.NONE && item.activeSlot.myItem.equip_type != carriedItem.equip_type)
            {
                
                return;
            }


            

            item.activeSlot.SetItem(carriedItem);
            
        }
        

        Debug.Log($"item's name : {item.name}, item's id : {item.itemComponent.ItemID}");


        if (item != null &&
            item.activeSlot != null &&
            item.activeSlot.myItem != null &&
            item.activeSlot.myItem.equip_type != Equipment_Type.NONE)
        {
            EquipEquipment(item.activeSlot.Equip_Type, null);
        }
        else
        {
            // 어느 부분에서 null이 발생했는지 디버그 로그 추가
            if (item == null)
            {
                Debug.LogError("Item is null.");
            }
            else if (item.activeSlot == null)
            {
                Debug.LogError("item.activeSlot is null.");
            }
            else if (item.activeSlot.myItem == null)
            {
                Debug.LogError("item.activeSlot.myItem is null.");
            }
            else if (item.activeSlot.myItem.equip_type == Equipment_Type.NONE)
            {
                Debug.LogError("item.activeSlot.myItem.equip_type is NONE.");
            }
        }

        carriedItem = item;
        //Inventory.instance.tempItemSlot = 
        carriedItem.canvasGroup.blocksRaycasts = false;
        item.transform.SetParent(draggablesTransform);
        


    }

    public void EquipEquipment(Equipment_Type tag, InventoryItem item = null)
    {
        switch (tag)
        {
            case Equipment_Type.HELMET:
                if (item == null)
                {
                    //나중에 할거야 이건
                }
                else
                {
                    //Debug.Log("Equipped " + item.myItem.name + " on " + tag);
                }
                break;
            case Equipment_Type.CHESTPLATE:
                break;
            case Equipment_Type.LEGGINGS:
                break;
            case Equipment_Type.BOOTS:
                break;
            case Equipment_Type.ONE_HANDED_SWORD: //이거 좀 더 다양성 추가할 예정
                break;
            case Equipment_Type.SHIELD: //장신구 칸인데 없어도 될듯?
                break;
        }
    }
    private void SetHotBar()
    {
        
        for (int i = 0; i < HotBarSlots_Out.Length; i++)
        {
            if (HotBarSlots_Out[i] == null)
            {
                Debug.LogWarning($"HotBarSlot at index {i} is null.");
                continue;
            }

            if (Inventory.instance.inv_Slot[i] == null)
            {

                if (HotBarSlots_Out[i].myItem != null)
                {



                    TryRemoveItem(HotBarSlots_Out, hotitemSet, i, out InventoryItem removedItem);
                    //Destroy(HotBarSlots_Out[i].myItem);
                }
                


            }
            else
            {
                var tempItem = Inventory.instance.inv_Slot[i];
                

                TryPlaceItem(HotBarSlots_Out, hotitemSet, i, tempItem, itemPrefab);

                if (HotBarSlots_Out[i].myItem == null)
                {
                    
                    //TryRemoveItem(HotBarSlots_Out, hotitemSet, i, out InventoryItem removedItem);
                }




            }
        }
    }


    private void SetInventory()
    {

        if (Inventory.instance.inv_Slot == null)
        {
            Debug.LogError("인벤없데이");
            return;
        }

        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (Inventory.instance.inv_Slot[i] == null)
            {

                if (inventorySlots[i].myItem != null)
                {
                    

                    //Destroy(HotBarSlots_Out[i].myItem);
                }

                TryRemoveItem(inventorySlots, invenitemSet, i, out InventoryItem removedItem);
                

                //TryRemoveItem(CraftingSlots, craftitemSet, i, out removedItem);
                //if(inven)


            }
            else
            {
                var tempItem = Inventory.instance.inv_Slot[i];
                TryPlaceItem(inventorySlots, invenitemSet, i, tempItem, itemPrefab);
                //var tempItem2 = Inventory.instance.inv_Slot[i];
                //TryPlaceItem(CraftingSlots, craftitemSet, i, tempItem2, itemPrefab);

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

        //Debug.Log($"Trying to place item at index {index}");

        // slots[index].myItem이 null인 경우에만 작업을 수행
        if (slots[index].myItem == null)
        {
            if (itemSet[index] == null)
            {
                //Debug.Log("itemset index : " + index);
                // itemSet[index]가 null이면 새 인스턴스를 생성합니다.
                itemSet[index] = Instantiate(itemPrefab, slots[index].transform);
            }
            

            // itemSet[index]를 초기화합니다.
            
            
        }
        else
        {
            Debug.Log("버그찾기" + index);
        }

        itemSet[index].Initialize(tempItem, slots[index]);
        return false;
    }


    public bool TryRemoveItem(InventorySlot[] slots, InventoryItem[] itemSet, int index, out InventoryItem removedItem)
    {
        removedItem = null;

        if (index >= slots.Length || index >= itemSet.Length)
        {
            Debug.LogError($"Index {index} is out of range for slots or itemSet array.");
            return false;
        }

        
        if (itemSet[index] != null && slots[index].myItem != null)
        {
            removedItem = itemSet[index];
            Debug.Log(itemSet[index].name);
            Destroy(removedItem.gameObject);
            itemSet[index] = null;
            slots[index].myItem = null;
            return true;
        }
        
        return false;
    }
}
