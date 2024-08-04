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

        //TODO >> 처음 클릭한 슬롯이 메인 인벤토리인지, 핫바인지, 아니면 장비칸인지에 대한 판단 처리
        //그 다음으로 그 인덱스 받아오기
        //SWAP
        //
        //

        //int oldSlotIndex = -1;
        
        bool isInEquip = false; //나중에 쓸 무언가
        bool isInEquipNew = false;
        
        
      
        int oldSlotIndex = System.Array.IndexOf(Inven.inventorySlots, Inven.carriedItem.activeSlot);

        Debug.Log("oldSlotIndex : " + oldSlotIndex);


        if (oldSlotIndex == -1)
        {
            oldSlotIndex = System.Array.IndexOf(Inven.equipmentSlots, Inven.carriedItem.activeSlot);
            isInEquip = true;
        }
        

        // 새로운 슬롯의 인덱스를 저장
        

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
                // isInEquip는 false이고 isInEquipNew는 true인 경우
                Swap(ref Inventory.instance.inv_Slot[oldSlotIndex], ref Inventory.instance.Equipment_Slot[newSlotIndex]);
            }
            else
            {
                // isInEquip는 true이고 isInEquipNew는 false인 경우
                Swap(ref Inventory.instance.Equipment_Slot[oldSlotIndex], ref Inventory.instance.inv_Slot[newSlotIndex]);

            }
        }
        else
        {
            if (isInEquip && isInEquipNew)
            {
                // isInEquip와 isInEquipNew가 모두 true인 경우
                Swap(ref Inventory.instance.Equipment_Slot[oldSlotIndex], ref Inventory.instance.Equipment_Slot[newSlotIndex]);
            }
            else
            {
                // isInEquip와 isInEquipNew가 모두 false인 경우
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