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
            if(Inven.carriedItem == null) return;
            if(myTag != SlotTag.None && Inven.carriedItem.myItem.itemTag != myTag) return;

            
            Debug.Log("Clicked slot index: " + Inven.carriedItem.name);

            SetItem(Inven.carriedItem);
        }
    }

    public void SetItem(InventoryItem item)
    {

        //TODO >> ó�� Ŭ���� ������ ���� �κ��丮����, �ֹ�����, �ƴϸ� ���ĭ������ ���� �Ǵ� ó��
        //�� �������� �� �ε��� �޾ƿ���
        //SWAP
        //���ĭ�϶��� �� �κ��丮�� �ִ� ITEMCOMPONENT ��ũ��Ʈ�� �޾ƿ��� Ÿ������ Ȯ���ϴ� ������ �ؾ� ��
        //�����۵� ��� �ϴ� �� >> �ֹٿ� ���� ó�� �ؾ���
        //

       
        //int oldSlotIndex = -1;
        bool isInHotbar = false; //���߿� �� ����

        // �������� �ֹ� ���Կ� �ִ��� Ȯ��
        int oldSlotIndex = System.Array.IndexOf(Inven.hotbarSlots, Inven.carriedItem.activeSlot);
        if (oldSlotIndex != -1)
        {
            isInHotbar = true;
        }
        else
        {
            // �������� �κ��丮 ���Կ� �ִ��� Ȯ��
            oldSlotIndex = System.Array.IndexOf(Inven.inventorySlots, Inven.carriedItem.activeSlot);
        }



        Debug.Log("oldSlotIndex" + oldSlotIndex);

        

        
        // ���ο� ������ �ε����� ����
        int newSlotIndex = System.Array.IndexOf(Inven.hotbarSlots, this);

        if (newSlotIndex != -1)
        {
            isInHotbar = true;
        }
        else
        {
            // �������� �κ��丮 ���Կ� �ִ��� Ȯ��
            newSlotIndex = System.Array.IndexOf(Inven.inventorySlots, this);
        }

        Debug.Log("newSlotIndex" + newSlotIndex);

        Inven.carriedItem = null;

        // Reset old slot
        item.activeSlot.myItem = null;

        
        //Debug.Log("Previous slot index: " + oldSlotIndex);


        // Set current slot
        myItem = item;
        myItem.activeSlot = this;
        myItem.transform.SetParent(transform);
        myItem.canvasGroup.blocksRaycasts = true;

        //Swap
        ItemComponent itemCom  = Inventory.instance.inv_Slot[oldSlotIndex];

        Inventory.instance.inv_Slot[oldSlotIndex] = Inventory.instance.inv_Slot[newSlotIndex];

        Inventory.instance.inv_Slot[newSlotIndex] = itemCom;

        Inventory.instance.ChangeEvent();
        //TODO >>> �ֹٿ� ���� �κ�, ��������� ���� �κ�, ��� ������, itemLayer�� �����ϰ� ��� ������ ���������� ��ȯ�� �� >>>> �ʼ� ����¯�߿�



        if (myTag != SlotTag.None)
        { 
            Inven.EquipEquipment(myTag, myItem);
        }
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