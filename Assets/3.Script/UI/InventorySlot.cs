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

    private InventoryUI Inven;

    

    public Equipment_Type Equip_Type;

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
            if(Inven.carriedItem == null) return;
            if(Equip_Type != Equipment_Type.NONE && Inven.carriedItem.equip_type != Equip_Type) return;

            
            //Debug.Log("Clicked slot index: " + Inven.carriedItem.name);

            SetItem(Inven.carriedItem);
        }
    }

    public void SetItem(InventoryItem item)
    {

        //TODO >> 처음 클릭한 슬롯이 메인 인벤토리인지, 핫바인지, 아니면 장비칸인지에 대한 판단 처리
        //그 다음으로 그 인덱스 받아오기
        //SWAP
        //장비칸일때는 그 인벤토리에 있는 ITEMCOMPONENT 스크립트를 받아오고 타입인지 확인하는 절차를 해야 함 >>>>> 개 웃긴점, 이미 타입 받아와 놓고 컴포넌트 받아오려는 개똥멍청이조영준, 진짜 멍청함
        //아이템들 사용 하는 것 >> 핫바에 대한 처리 해야함 >> 핫바 처리 완료, 다음 할 건 핫바에서도 활성화 칸 근데 문제는 무기때문에 어찌해야 할지 모르겠네 >> 이것도 나중에 생각해보자 >>
        //


        //int oldSlotIndex = -1;
        bool isInHotBar = false;
        bool isInHotBarNew = false;
        bool isInEquip = false; //나중에 쓸 무언가
        bool isInEquipNew = false;
        

        // 아이템이 핫바 슬롯에 있는지 확인
        int oldSlotIndex = System.Array.IndexOf(Inven.hotbarSlots, Inven.carriedItem.activeSlot);
        if (oldSlotIndex != -1)
        {
            //Debug.Log("핫바 인덱스 : " + oldSlotIndex);
            //나중에 지울거야 수벌
            isInHotBar = true;
        }
        else
        {
            // 아이템이 인벤토리 슬롯에 있는지 확인
            oldSlotIndex = System.Array.IndexOf(Inven.inventorySlots, Inven.carriedItem.activeSlot);

            if(oldSlotIndex == -1)
            {
                oldSlotIndex = System.Array.IndexOf(Inven.equipmentSlots, Inven.carriedItem.activeSlot);
                isInEquip = true;
            }

        }
        //개쓰레기 논리이긴 한데 머리가 안돌아가요
        //Debug.Log("oldSlotIndex" + oldSlotIndex);


        // 새로운 슬롯의 인덱스를 저장
        int newSlotIndex = System.Array.IndexOf(Inven.hotbarSlots, this);
        Debug.Log("new hot bat slot" + newSlotIndex);

        if (newSlotIndex != -1)
        {
            //Debug.Log("핫바 인덱스new : " + newSlotIndex);
            isInHotBarNew = true;
        }
        else
        {
            // 아이템이 인벤토리 슬롯에 있는지 확인
            newSlotIndex = System.Array.IndexOf(Inven.inventorySlots, this);
            if (newSlotIndex == -1)
            {
                if (Inven.carriedItem.activeSlot == null)
                {
                    Debug.LogError("activeSlot is null");
                }
                if (Inven.equipmentSlots == null || Inven.equipmentSlots.Length == 0)
                {
                    Debug.LogError("equipmentSlots array is null or empty");
                }
                if (System.Array.IndexOf(Inven.equipmentSlots, this) == -1)
                {
                    Debug.LogError("activeSlot not found in equipmentSlots");
                }

                newSlotIndex = System.Array.IndexOf(Inven.equipmentSlots, this);
                


                //Debug.Log("장비칸~~~" + newSlotIndex);



                isInEquipNew = true;
            }
        }

        //
        //
        //
        //
        //("newSlotIndex" + newSlotIndex);

        Inven.carriedItem = null;

        // Reset old slot
        item.activeSlot.myItem = null;

        
        //Debug.Log("Previous slot index: " + oldSlotIndex);


        // Set current slot
        myItem = item;
        myItem.activeSlot = this;
        myItem.transform.SetParent(transform);
        myItem.canvasGroup.blocksRaycasts = true;

        if (!isInHotBar && !isInEquip)
        {
            oldSlotIndex += Inven.hotbarSlots.Length;
            //Debug.Log(oldSlotIndex);
        }

        if (!isInHotBarNew && !isInEquipNew)
        {
            newSlotIndex += Inven.hotbarSlots.Length;
            //Debug.Log(newSlotIndex);
        }

        //Swap
        //아 또 멍청한 실수 했다
        if (isInEquip != isInEquipNew)
        {
            if (!isInEquip && isInEquipNew)
            {
                
                // isInEquip는 false이고 isInEquipNew는 true인 경우
                Swap(ref Inventory.instance.inv_Slot[oldSlotIndex], ref Inventory.instance.Equipment_Slot[newSlotIndex]);
                

            }
            else if (isInEquip && !isInEquipNew)
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


        //itemCom  = Inventory.instance.inv_Slot[oldSlotIndex];

        Inventory.instance.ChangeEvent();
        //TODO >>> 핫바에 대한 부분, 장비장착에 대한 부분, 장비 장착시, itemLayer를 제거하고 사용 가능한 아이템으로 변환할 것 >>>> 필수 정말짱중요



        if (Equip_Type != Equipment_Type.NONE)
        { 
            Inven.EquipEquipment(Equip_Type, myItem);
        }
    }

    private void Swap(ref ItemComponent Old_Item, ref ItemComponent New_Item)
    {
        ItemComponent temp = Old_Item;
        Old_Item = New_Item;
        New_Item = temp;

    }

    private void Swap(ref InventorySlot oldSlot, ref InventorySlot newSlot)
    {
        InventorySlot temp = oldSlot;
        oldSlot = newSlot;
        newSlot = temp;
    }


   public void CheckCurrentSlot()
    {
        //if()
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