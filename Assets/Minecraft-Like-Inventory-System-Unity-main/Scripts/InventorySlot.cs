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

        //TODO >> ó�� Ŭ���� ������ ���� �κ��丮����, �ֹ�����, �ƴϸ� ���ĭ������ ���� �Ǵ� ó��
        //�� �������� �� �ε��� �޾ƿ���
        //SWAP
        //���ĭ�϶��� �� �κ��丮�� �ִ� ITEMCOMPONENT ��ũ��Ʈ�� �޾ƿ��� Ÿ������ Ȯ���ϴ� ������ �ؾ� ��
        //�����۵� ��� �ϴ� �� >> �ֹٿ� ���� ó�� �ؾ���
        //


        // ���� ������ �ε����� ����
        int oldSlotIndex = System.Array.IndexOf(Inven.inventorySlots, item.activeSlot);

        Debug.Log("�������� ��������" + oldSlotIndex);

        // ���ο� ������ �ε����� ����
        int newSlotIndex = System.Array.IndexOf(Inven.inventorySlots, this);
        Debug.Log("�������� ����������¥" + newSlotIndex);

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

        //TODO >>> �ֹٿ� ���� �κ�, ��������� ���� �κ�, ��� ������, itemLayer�� �����ϰ� ��� ������ ���������� ��ȯ�� �� >>>> �ʼ� ����¯�߿�

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