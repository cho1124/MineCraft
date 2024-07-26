using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public enum SlotTag { None, Head, Chest, Legs, Feet, Weapon, Accessories, }

public class InventorySlot : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    private Color slot_color;

    private Image slot_image = null;
    public InventoryItem myItem { get; set; }

    public InventoryUI Inven;

    public SlotTag myTag;

    private float slot_color_value = 0.2f;

    private void Awake()
    {
        slot_image = GetComponent<Image>();
        Inven = FindObjectOfType<InventoryUI>();

    }


    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Left)
        {
            if(InventoryUI.carriedItem == null) return;
            if(myTag != SlotTag.None && InventoryUI.carriedItem.myItem.itemTag != myTag) return;

            
            Debug.Log("Clicked slot index: " + InventoryUI.carriedItem.equip_type);

            SetItem(InventoryUI.carriedItem);
        }
    }

    public void SetItem(InventoryItem item)
    {

        //TODO >> 처음 클릭한 슬롯이 메인 인벤토리인지, 핫바인지, 아니면 장비칸인지에 대한 판단 처리
        //그 다음으로 그 인덱스 받아오기
        //SWAP
        //장비칸일때는 그 인벤토리에 있는 ITEMCOMPONENT 스크립트를 받아오고 타입인지 확인하는 절차를 해야 함
        //아이템들 사용 하는 것 >> 핫바에 대한 처리 해야함
        //


        // 이전 슬롯의 인덱스를 저장
        int oldSlotIndex = System.Array.IndexOf(Inven.inventorySlots, item.activeSlot);

        Debug.Log("조영준은 개쓰레기" + oldSlotIndex);

        // 새로운 슬롯의 인덱스를 저장
        int newSlotIndex = System.Array.IndexOf(Inven.inventorySlots, this);
        Debug.Log("조영준은 개쓰레기진짜" + newSlotIndex);

        InventoryUI.carriedItem = null;

        // Reset old slot
        item.activeSlot.myItem = null;

        
        //Debug.Log("Previous slot index: " + oldSlotIndex);


        // Set current slot
        myItem = item;
        myItem.activeSlot = this;
        myItem.transform.SetParent(transform);
        myItem.canvasGroup.blocksRaycasts = true;


        ItemComponent itemCom  = Inventory.instance.inv_Slot[oldSlotIndex];

        Inventory.instance.inv_Slot[oldSlotIndex] = Inventory.instance.inv_Slot[newSlotIndex];

        Inventory.instance.inv_Slot[newSlotIndex] = itemCom;

        //TODO >>> 핫바에 대한 부분, 장비장착에 대한 부분, 장비 장착시, itemLayer를 제거하고 사용 가능한 아이템으로 변환할 것 >>>> 필수 정말짱중요

        //Inventory.Instance.inventorySlots[oldSlotIndex] = null;
        //Inventory.Instance.inventorySlots[newSlotIndex] = myItem;

        //Debug.Log("New slot index: " + newSlotIndex);

        if (myTag != SlotTag.None)
        { 
            Inven.EquipEquipment(myTag, myItem);
        }
    }

   
    public void ClearSlot()
    {
        if (myItem != null)
        {
            Destroy(myItem.gameObject);
            myItem = null;
        }
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