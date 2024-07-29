using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public InventoryItem carriedItem;

    [Header("�κ��丮 ���� ����")]
    public InventorySlot[] inventorySlots; //1�� ���� �迭
    public InventorySlot[] hotbarSlots; //2�� ���� �迭
    // 0: HEAD, 1: CHEST, 2: LEGGINGS, 3: FEET, 4: WEAPON, 5: Accessories
    public InventorySlot[] equipmentSlots; //�ۺ� ������ �ʹ� ���Ŵϴ�

    [Header("�κ��丮 �ܺ� ����")]
    public InventorySlot[]  HotBarSlots_Out;
   


    [Header("������ ����(���������� �ӽ÷� ���̴� ��)")]
    [SerializeField] private InventoryItem[] hotitemSet;
    [SerializeField] private InventoryItem[] hotitemSet_inv;
    [SerializeField] private InventoryItem[] invenitemSet;

    [Header("ETC")]
    [SerializeField] private Transform draggablesTransform;
    [SerializeField] private InventoryItem itemPrefab;

    private void Awake()
    {
        
    }

    private void Start()
    {
        hotitemSet = new InventoryItem[HotBarSlots_Out.Length];
        hotitemSet_inv = new InventoryItem[hotbarSlots.Length];
        invenitemSet = new InventoryItem[inventorySlots.Length];

        if (Inventory.instance != null)
        {
            Inventory.instance.OnChangedInv += SetHotBarInv;
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
            if (item.activeSlot != null && item.activeSlot.myItem != null && item.activeSlot.myItem.equip_type != Equipment_Type.NONE && item.activeSlot.myItem.equip_type != carriedItem.equip_type) return;
            item.activeSlot.SetItem(carriedItem);
        }

        if (item != null &&
            item.activeSlot != null &&
            item.activeSlot.myItem != null &&
            item.activeSlot.myItem.equip_type != Equipment_Type.NONE)
        {
            EquipEquipment(item.activeSlot.Equip_Type, null);
        }
        else
        {
            // ��� �κп��� null�� �߻��ߴ��� ����� �α� �߰�
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
                    //���߿� �Ұž� �̰�
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
            case Equipment_Type.ONE_HANDED_SWORD: //�̰� �� �� �پ缺 �߰��� ����
                break;
            case Equipment_Type.SHIELD: //��ű� ĭ�ε� ��� �ɵ�?
                break;
        }
    }

    private void SetHotBarInv()
    {
        Debug.Log("HotBarChanged123");

        for(int i = 0; i < hotbarSlots.Length; i++)
        {
            if (hotbarSlots[i] == null)
            {
                Debug.LogWarning($"HotBarSlot at index {i} is null.");
                continue;
            }

            if (Inventory.instance.inv_Slot[i] == null)
            {
                Debug.Log("inv_Slot is nulllllllllll");
                if (hotbarSlots[i].myItem != null)
                {
                    Debug.Log("in : remove");
                    TryRemoveItem(hotbarSlots, hotitemSet_inv, i, out InventoryItem removedItem);
                    //Destroy(HotBarSlots_Out[i].myItem);
                }
            }
            else
            {
                var tempItem = Inventory.instance.inv_Slot[i];

                TryPlaceItem(hotbarSlots, hotitemSet_inv, i, tempItem, itemPrefab);

                if (hotbarSlots[i].myItem == null)
                {
                    Debug.Log($"After TryPlaceItem, hotbarSlots[{i}].myItem is still null.");
                    //TryRemoveItem(hotbarSlots, hotitemSet_inv, i, out InventoryItem removedItem);
                }
            }
        }

    }

    private void SetHotBar()
    {
        Debug.Log("HotBarChanged");

        for (int i = 0; i < HotBarSlots_Out.Length; i++)
        {
            if (HotBarSlots_Out[i] == null)
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
                
                if (HotBarSlots_Out[i].myItem != null)
                {
                    Debug.Log("out : remove");
                    TryRemoveItem(HotBarSlots_Out, hotitemSet, i, out InventoryItem removedItem);
                    hotitemSet_inv[i] = hotitemSet[i];

                    //Destroy(HotBarSlots_Out[i].myItem);
                }

                
            }
            else
            {
                var tempItem = Inventory.instance.inv_Slot[i];
                Debug.Log($"Trying to place item {tempItem} at index {i}");

                TryPlaceItem(HotBarSlots_Out, hotitemSet, i, tempItem, itemPrefab);
                
                if (HotBarSlots_Out[i].myItem == null)
                {
                    Debug.Log($"After TryPlaceItem, HotBarSlots_Out[{i}].myItem is still null.");
                    //TryRemoveItem(HotBarSlots_Out, hotitemSet, i, out InventoryItem removedItem);
                }

                

                
            }
        }
    }


    private void SetInventory()
    {
        int index;

        for(int i = 0; i < inventorySlots.Length; i++)
        {
            index = i + hotbarSlots.Length;
            if(Inventory.instance.inv_Slot == null)
            {
                Debug.LogError("�κ�������");
                return;
            }

            if(Inventory.instance.inv_Slot[index] == null)
            {
                if (inventorySlots[i].myItem != null)
                {
                    TryRemoveItem(inventorySlots, invenitemSet, i, out InventoryItem removedItem);
                }

                
            }
            else
            {
                var tempItem = Inventory.instance.inv_Slot[index];
                TryPlaceItem(inventorySlots, invenitemSet, i, tempItem, itemPrefab);
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

        //if (slots[index].myItem == null && itemSet[index] == null) //�ƹ��͵� ������
        //{
        //    itemSet[index] = Instantiate(itemPrefab, slots[index].transform);
        //    itemSet[index].Initialize(tempItem, slots[index]);
        //    return true;
        //}
        //else if(slots[index].myItem != null && itemSet[index] == null) 
        //{
        //    Debug.Log("����");
        //    
        //    itemSet[index].Initialize(tempItem, slots[index]);
        //    return true;
        //}
        //else
        //{
        //    Debug.Log("shit");
        //    itemSet[index].Initialize(tempItem, slots[index]);
        //    return true;
        //}


        if(itemSet[index] == null)
        {
            
            if (slots[index].myItem == null)
            {
                itemSet[index] = Instantiate(itemPrefab, slots[index].transform);
                itemSet[index].Initialize(tempItem, slots[index]);
                return true;
            }
            
        }
        else
        {
            if (slots[index].myItem == null)
            {

                itemSet[index].Initialize(tempItem, slots[index]);
                return true;
            }
            else
            {
                Debug.Log("����");

                itemSet[index].Initialize(tempItem, slots[index]);
                return true;
            }
        }


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
