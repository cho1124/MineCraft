using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    
    public InventoryItem carriedItem;
   

    [SerializeField] public InventorySlot[] inventorySlots; //1�� ���� �迭
    [SerializeField] public InventorySlot[] hotbarSlots; //2�� ���� �迭

    // 0: HEAD, 1: CHEST, 2: LEGGINGS, 3: FEET, 4: WEAPON, 5: Accessories
    [SerializeField] public InventorySlot[] equipmentSlots; //�ۺ� ������ �ʹ� ���Ŵϴ�

    [SerializeField] private Transform draggablesTransform;
    [SerializeField] private InventoryItem itemPrefab;

    [Header("Item List")]
    
    [SerializeField] private List<Original_Item> item_list;
    [SerializeField] private InventoryItem[] itemSet;
    [SerializeField] private InventoryItem[] hotitemSet;

    private UIManager UIManager;

    //[SerializeField] private Button giveItemBtn;

    void Awake()
    {
        
        
        

        UIManager = GetComponentInParent<UIManager>();
        if(UIManager == null)
        {
            Debug.LogError("UI����");
        }

        


        itemSet = new InventoryItem[inventorySlots.Length];
        hotitemSet = new InventoryItem[hotbarSlots.Length];
        //SetEmptyItem();
    }

    private void OnEnable()
    {
        
        Debug.Log("enabled");
        
        SetInventory();
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

   
    public void SetInventory()
    {
        //inventory.instance.GetInv_Main()[0].

        ItemComponent[] inv = Inventory.instance.inv_Slot;
        
        int invLength = Inventory.instance.inv_Slot.Length;

        //hotitemSet = UIManager.hotitemSet;

        
        for (int i = 0; i < invLength; i++)
        {
            if(inv[i] != null)
            {
                if (i < hotbarSlots.Length) //�̰� ���̷��ĸ� ���� �κ��丮�� ui������ �κ��丮�� �ٸ� �κ��̱� ������
                {
                    if(hotbarSlots[i].myItem == null)
                    {
                        hotitemSet[i] = Instantiate(itemPrefab, hotbarSlots[i].transform);
                    }



                    hotitemSet[i].Initialize(inv[i], hotbarSlots[i]);
                    return;

                }
                else
                {
                    int inv_i = i - hotbarSlots.Length;

                    if(inventorySlots[inv_i].myItem == null)
                    {
                        itemSet[inv_i] = Instantiate(itemPrefab, inventorySlots[inv_i].transform);
                    }

                    if(itemSet[inv_i] == null)
                    {
                        Debug.Log("error");
                    }
                    else
                    itemSet[inv_i].Initialize(inv[inv_i], inventorySlots[inv_i]);
                    return;
                }

            }
            

        }

    }



    

    public void InitSlot()
    {
        

        itemSet = new InventoryItem[inventorySlots.Length];
        hotitemSet = new InventoryItem[hotbarSlots.Length];
    }


}