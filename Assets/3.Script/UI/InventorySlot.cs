using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;



public class InventorySlot : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    private Color slot_color;

    private Image slot_image = null;
    public InventoryItem myItem { get; set; }

    private UIManager Inven;

    
    public Equipment_Type Equip_Type;

    private float slot_color_value = 0.2f;
    

    public delegate void ItemChanged();
    public event ItemChanged OnItemChanged;


    private void Awake()
    {
        slot_image = GetComponent<Image>();
        Inven = FindObjectOfType<UIManager>();

    }


    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Left)
        {
            if(Inven.carriedItem == null) return;
            if(Equip_Type != Equipment_Type.NONE && (Inven.carriedItem.equip_type & Equip_Type) == 0) return;

            
            
            //Debug.Log("Clicked slot index: " + Inven.carriedItem.name);

            

            SetItem(Inven.carriedItem);
            
        }
    }

    public void SetItem(InventoryItem item)
    {

        //TODO >> ó�� Ŭ���� ������ ���� �κ��丮����, �ֹ�����, �ƴϸ� ���ĭ������ ���� �Ǵ� ó��
        //�� �������� �� �ε��� �޾ƿ���
        //SWAP
        //
        //

        //int oldSlotIndex = -1;
        
        bool isInEquip = false; //���߿� �� ����
        bool isInEquipNew = false;
        
        
      
        int oldSlotIndex = System.Array.IndexOf(Inven.inventorySlots, Inven.carriedItem.activeSlot);

        Debug.Log("oldSlotIndex : " + oldSlotIndex);


        if (oldSlotIndex == -1)
        {
            oldSlotIndex = System.Array.IndexOf(Inven.equipmentSlots, Inven.carriedItem.activeSlot);
            isInEquip = true;
        }
        

        // ���ο� ������ �ε����� ����
        

        int newSlotIndex = System.Array.IndexOf(Inven.inventorySlots, this);
        
        if (newSlotIndex == -1)
        {
            newSlotIndex = System.Array.IndexOf(Inven.equipmentSlots, this);
            isInEquipNew = true;

        }
        Debug.Log("newSlotIndex : " + newSlotIndex);


        // Set current slot
        Inven.carriedItem = null;

        // Reset old slot
        item.activeSlot.myItem = null;

        myItem = item;
        myItem.activeSlot = this;
        myItem.transform.SetParent(transform);
        myItem.canvasGroup.blocksRaycasts = true;


        //Swap
        
        if (isInEquip != isInEquipNew)
        {
            if (!isInEquip && isInEquipNew)
            {
                // isInEquip�� false�̰� isInEquipNew�� true�� ���
                Swap(ref Inventory.instance.inv_Slot[oldSlotIndex], ref Inventory.instance.Equipment_Slot[newSlotIndex]);
            }
            else
            {
                // isInEquip�� true�̰� isInEquipNew�� false�� ���
                Swap(ref Inventory.instance.Equipment_Slot[oldSlotIndex], ref Inventory.instance.inv_Slot[newSlotIndex]);

            }
        }
        else
        {
            if (isInEquip && isInEquipNew)
            {
                // isInEquip�� isInEquipNew�� ��� true�� ���
                Swap(ref Inventory.instance.Equipment_Slot[oldSlotIndex], ref Inventory.instance.Equipment_Slot[newSlotIndex]);
            }
            else
            {
                // isInEquip�� isInEquipNew�� ��� false�� ���
                Swap(ref Inventory.instance.inv_Slot[oldSlotIndex], ref Inventory.instance.inv_Slot[newSlotIndex]);
                
            }

        }

        if (Equip_Type != Equipment_Type.NONE)
        { 
            Inven.EquipEquipment(Equip_Type, myItem);
        }
        Inventory.instance.ChangeEvent();
        OnItemChanged?.Invoke();
    }

    private void Swap(ref ItemComponent Old_Item, ref ItemComponent New_Item)
    {
        ItemComponent temp = Old_Item;
        Old_Item = New_Item;
        New_Item = temp;

    }

    private void Swap(ref InventoryItem oldItem, ref InventoryItem newItem)
    {
        InventoryItem temp = oldItem;
        oldItem = newItem;
        newItem = temp;
    }


   public void CheckCurrentSlot()
    {
        //if()
    }


    public void ClearSlot()
    {
        myItem = null;
        slot_image.sprite = null;
        slot_image.color = Color.clear;
        OnItemChanged?.Invoke();
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        SlotColor(slot_color_value);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        SlotColor(0);
    }

    private void OnDisable()
    {
        SlotColor(0);
    }

    private void SlotColor(float value)
    {
        slot_color = slot_image.color;
        slot_color.a = value;
        slot_image.color = slot_color;
    }
}