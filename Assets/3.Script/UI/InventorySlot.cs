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
    private bool isCrafting;

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
        isCrafting = false;
        bool isCraftingNew = false;
        

        // �������� �ֹ� ���Կ� �ִ��� Ȯ��
        

        int oldSlotIndex = System.Array.IndexOf(Inven.inventorySlots, Inven.carriedItem.activeSlot);

        Debug.Log("�̰� ���̿� :" + oldSlotIndex);
        if (oldSlotIndex == -1)
        {
            oldSlotIndex = System.Array.IndexOf(Inven.equipmentSlots, Inven.carriedItem.activeSlot);
            isInEquip = true;
        }
        if(oldSlotIndex == -1)
        {
            oldSlotIndex = System.Array.IndexOf(Inven.minicraftingSlots, Inven.carriedItem.activeSlot);
            isInEquip = false;
            isCrafting = true;
        }


        
        //Debug.Log("oldSlotIndex" + oldSlotIndex);


        // ���ο� ������ �ε����� ����
        

        int newSlotIndex = System.Array.IndexOf(Inven.inventorySlots, this);
        Debug.Log("�̰� ���̿�2 :" + newSlotIndex);
        if (newSlotIndex == -1)
        {
            newSlotIndex = System.Array.IndexOf(Inven.equipmentSlots, this);
            isInEquipNew = true;

        }
        if(newSlotIndex == -1)
        {
            newSlotIndex = System.Array.IndexOf(Inven.minicraftingSlots, this);
            isCraftingNew = true;
            isInEquipNew = false;
        }

        
        //Debug.Log("newSlotIndex" + newSlotIndex);




        //Debug.Log("Previous slot index: " + oldSlotIndex);
        Debug.Log("carriedItem " + Inven.carriedItem);

        // Set current slot

        if(!isCrafting)
        {
            Inven.carriedItem = null;

            // Reset old slot
            item.activeSlot.myItem = null;

            myItem = item;
            myItem.activeSlot = this;
            myItem.transform.SetParent(transform);
            myItem.canvasGroup.blocksRaycasts = true;
        }
        else
        {
            Debug.Log("crafting block");
            Inven.carriedItem = item;
            myItem = item;
            myItem.activeSlot = this;
            myItem.transform.SetParent(transform);
            myItem.transform.position = Vector2.zero;

            myItem.canvasGroup.blocksRaycasts = true;
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
                if(!isCrafting)
                {
                    Swap(ref Inventory.instance.Equipment_Slot[oldSlotIndex], ref Inventory.instance.inv_Slot[newSlotIndex]);
                }

                // isInEquip�� true�̰� isInEquipNew�� false�� ���
                
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
                if (isCrafting)
                {
                    Inventory.instance.inv_Slot[oldSlotIndex].StackCurrent--;
                    Inventory.instance.Crafting_Mini_Slot[newSlotIndex] = Inventory.instance.inv_Slot[oldSlotIndex];
                    Inventory.instance.Crafting_Mini_Slot[newSlotIndex].StackCurrent++;
                }


                // isInEquip�� isInEquipNew�� ��� false�� ���
                Swap(ref Inventory.instance.inv_Slot[oldSlotIndex], ref Inventory.instance.inv_Slot[newSlotIndex]);
                
                


            }

        }


        //itemCom  = Inventory.instance.inv_Slot[oldSlotIndex];

        
        //TODO >>> �ֹٿ� ���� �κ�, ��������� ���� �κ�, ��� ������, itemLayer�� �����ϰ� ��� ������ ���������� ��ȯ�� �� >>>> �ʼ� ����¯�߿�



        if (Equip_Type != Equipment_Type.NONE)
        { 
            Inven.EquipEquipment(Equip_Type, myItem);
        }
        OnItemChanged?.Invoke();
        Inventory.instance.ChangeEvent();
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