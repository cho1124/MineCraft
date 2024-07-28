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

        //TODO >> ó�� Ŭ���� ������ ���� �κ��丮����, �ֹ�����, �ƴϸ� ���ĭ������ ���� �Ǵ� ó��
        //�� �������� �� �ε��� �޾ƿ���
        //SWAP
        //���ĭ�϶��� �� �κ��丮�� �ִ� ITEMCOMPONENT ��ũ��Ʈ�� �޾ƿ��� Ÿ������ Ȯ���ϴ� ������ �ؾ� �� >>>>> �� ������, �̹� Ÿ�� �޾ƿ� ���� ������Ʈ �޾ƿ����� ���˸�û��������, ��¥ ��û��
        //�����۵� ��� �ϴ� �� >> �ֹٿ� ���� ó�� �ؾ��� >> �ֹ� ó�� �Ϸ�, ���� �� �� �ֹٿ����� Ȱ��ȭ ĭ �ٵ� ������ ���⶧���� �����ؾ� ���� �𸣰ڳ� >> �̰͵� ���߿� �����غ��� >>
        //


        //int oldSlotIndex = -1;
        bool isInHotBar = false;
        bool isInHotBarNew = false;
        bool isInEquip = false; //���߿� �� ����
        bool isInEquipNew = false;
        

        // �������� �ֹ� ���Կ� �ִ��� Ȯ��
        int oldSlotIndex = System.Array.IndexOf(Inven.hotbarSlots, Inven.carriedItem.activeSlot);
        if (oldSlotIndex != -1)
        {
            //Debug.Log("�ֹ� �ε��� : " + oldSlotIndex);
            //���߿� ����ž� ����
            isInHotBar = true;
        }
        else
        {
            // �������� �κ��丮 ���Կ� �ִ��� Ȯ��
            oldSlotIndex = System.Array.IndexOf(Inven.inventorySlots, Inven.carriedItem.activeSlot);

            if(oldSlotIndex == -1)
            {
                oldSlotIndex = System.Array.IndexOf(Inven.equipmentSlots, Inven.carriedItem.activeSlot);
                isInEquip = true;
            }

        }
        //�������� ���̱� �ѵ� �Ӹ��� �ȵ��ư���
        //Debug.Log("oldSlotIndex" + oldSlotIndex);


        // ���ο� ������ �ε����� ����
        int newSlotIndex = System.Array.IndexOf(Inven.hotbarSlots, this);
        Debug.Log("new hot bat slot" + newSlotIndex);

        if (newSlotIndex != -1)
        {
            //Debug.Log("�ֹ� �ε���new : " + newSlotIndex);
            isInHotBarNew = true;
        }
        else
        {
            // �������� �κ��丮 ���Կ� �ִ��� Ȯ��
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
                


                //Debug.Log("���ĭ~~~" + newSlotIndex);



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
        //�� �� ��û�� �Ǽ� �ߴ�
        if (isInEquip != isInEquipNew)
        {
            if (!isInEquip && isInEquipNew)
            {
                
                // isInEquip�� false�̰� isInEquipNew�� true�� ���
                Swap(ref Inventory.instance.inv_Slot[oldSlotIndex], ref Inventory.instance.Equipment_Slot[newSlotIndex]);
                

            }
            else if (isInEquip && !isInEquipNew)
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


        //itemCom  = Inventory.instance.inv_Slot[oldSlotIndex];

        Inventory.instance.ChangeEvent();
        //TODO >>> �ֹٿ� ���� �κ�, ��������� ���� �κ�, ��� ������, itemLayer�� �����ϰ� ��� ������ ���������� ��ȯ�� �� >>>> �ʼ� ����¯�߿�



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