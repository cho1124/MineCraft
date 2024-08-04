using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public InventoryItem carriedItem;
    
    public Entity_Humanoid player;

    [Header("�κ��丮 ���� ����")]
    public InventorySlot[] inventorySlots; //1�� ���� �迭
    // 0: HEAD, 1: CHEST, 2: LEGGINGS, 3: FEET, 4: WEAPON, 5: Accessories
    public InventorySlot[] equipmentSlots; //�ۺ� ������ �ʹ� ���Ŵϴ�
    public CraftingSlot[] minicraftingSlots;

    [Header("�κ��丮 �ܺ� ����")]
    public InventorySlot[] HotBarSlots_Out;

    

    
    
    [Header("ETC")]
    [SerializeField] private Transform draggablesTransform;
    [SerializeField] private InventoryItem itemPrefab;
    public Slider hpbar;
    public Image staminabar;
    

    private void Start()
    {
        //hotitemSet = new InventoryItem[HotBarSlots_Out.Length];
        
        



        if (Inventory.instance != null)
        {

            Inventory.instance.OnChangedInv += SetHotBar;
            Inventory.instance.OnChangedInv += SetInventory;
        }
        else
        {
            Debug.LogError("Inventory instance is null. Ensure that Inventory is properly initialized.");
        }

        if(player != null)
        {
            player.OnChangedStatus += UpdateStatus;
        }


    }

    

    private void UpdateStatus()
    {
        //Debug.Log("player max : " + player.Health_max + "player current : " + player.Health_current);
        

        hpbar.value = player.Health_current / player.Health_max;
        staminabar.fillAmount = player.Posture_current / player.Posture_max;
    }

    void Update()
    {
        //�����ļ� ������ ����
        UpdateStatus();
       
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
        

        //Debug.Log($"item's name : {item.name}, item's id : {item.itemComponent.ItemID}");


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

                    TryRemoveItem(HotBarSlots_Out, i, out InventoryItem removedItem);
                    //Destroy(HotBarSlots_Out[i].myItem);
                }
                


            }
            else
            {
                var tempItem = Inventory.instance.inv_Slot[i];
                

                TryPlaceItem(HotBarSlots_Out, i, tempItem, itemPrefab);

                if (HotBarSlots_Out[i].myItem == null)
                {
                    
                    //TryRemoveItem(HotBarSlots_Out, hotitemSet, i, out InventoryItem removedItem);
                }




            }
        }
    }


    private void SetInventory()
    {
        if (Inventory.instance == null || Inventory.instance.inv_Slot == null)
        {
            Debug.LogError("�κ�������");
            return;
        }

        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (i >= Inventory.instance.inv_Slot.Length)
            {
                Debug.LogError($"Inventory slot index {i} is out of range.");
                continue;
            }

            var inventorySlot = Inventory.instance.inv_Slot[i];
            var uiSlot = inventorySlots[i];

            if (inventorySlot == null)
            {
                if (uiSlot.myItem != null)
                {
                    TryRemoveItem(inventorySlots, i, out InventoryItem removedItem);
                    Debug.Log("Removed item at index " + i);
                }
            }
            else
            {
                var tempItem = inventorySlot;
                TryPlaceItem(inventorySlots, i, tempItem, itemPrefab);
            }
        }
    }

    public bool TryPlaceItem(InventorySlot[] slots, int index, ItemComponent tempItem, InventoryItem itemPrefab)
    {
        if (index >= slots.Length)
        {
            Debug.LogError($"Index {index} is out of range for slots or itemSet array.");
            return false;
        }

        //Debug.Log($"Trying to place item at index {index}");

        // slots[index].myItem�� null�� ��쿡�� �۾��� ����
        if (slots[index].myItem == null)
        {

            Debug.Log("slots is null");


            slots[index].myItem = Instantiate(itemPrefab, slots[index].transform);

            // itemSet[index]�� �ʱ�ȭ�մϴ�.


        }


        slots[index].myItem.Initialize(tempItem, slots[index]);
        return true;
    }


    public bool TryRemoveItem(InventorySlot[] slots, int index, out InventoryItem removedItem)
    {
        removedItem = null;

        if (index >= slots.Length)
        {
            Debug.LogError($"Index {index} is out of range for slots or itemSet array.");
            return false;
        }

        
        if (slots[index].myItem != null)
        {
            
            
            Destroy(slots[index].myItem);
            
            slots[index].myItem = null;
            return true;
        }
        
        return false;
    }
}
