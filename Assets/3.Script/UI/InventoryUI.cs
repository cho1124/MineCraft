using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    
    public InventoryItem carriedItem;
   

    [SerializeField] public InventorySlot[] inventorySlots; //1번 슬롯 배열
    [SerializeField] public InventorySlot[] hotbarSlots; //2번 슬롯 배열

    // 0: HEAD, 1: CHEST, 2: LEGGINGS, 3: FEET, 4: WEAPON, 5: Accessories
    [SerializeField] public InventorySlot[] equipmentSlots; //퍼블릭 마음이 너무 아픕니다

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
            Debug.LogError("UI없음");
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
            // 어느 부분에서 null이 발생했는지 디버그 로그 추가
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
                    //나중에 할거야 이건
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
            case Equipment_Type.ONE_HANDED_SWORD: //이거 좀 더 다양성 추가할 예정
                break;
            case Equipment_Type.SHIELD: //장신구 칸인데 없어도 될듯?
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
                if (i < hotbarSlots.Length) //이거 왜이러냐면 실제 인벤토리와 ui에서의 인벤토리는 다른 부분이기 때문에
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