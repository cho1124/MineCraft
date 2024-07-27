using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    // Start is called before the first frame update
    //일단 테스트용이니까 체력 경험치 등등등 전부 플레이어에 맞춰서 이벤트로 등록해놓으시면 될듯합니다.
    
    public InventorySlot[] HotBarSlots;
    //public Inventory inventory; //이 부분이 조금 애매한데, 몬스터 인벤토리가 있다고 가정했을 때 Find를 사용할때 문제가 될수 있어서 임시로 직접참조 때려 넣을게요

    [SerializeField] private InventoryItem[] hotitemSet;
    [SerializeField] private InventoryItem itemPrefab;


    private void Awake()
    {
        hotitemSet = new InventoryItem[HotBarSlots.Length];
    }

    private void Start()
    {
        //Inventory.instance.OnHotBarChanged += InitHotBar;
        Inventory.instance.OnHotBarChanged += SetHotBar;
        
    }

    private void InitHotBar()
    {
        for (int i = 0; i < HotBarSlots.Length; i++)
        {
            if (TryRemoveItem(HotBarSlots, hotitemSet, i, out InventoryItem removedItem))
            {
                //Debug.Log("내가 사용할 아이템 히히" + removedItem.name);
            }
        }
    }

    private void SetHotBar()
    {

        InitHotBar();

        //핫바 초기화하는 과정

        for (int i = 0; i < HotBarSlots.Length; i++)
        {
           
            if (Inventory.instance.inv_Slot[i] != null)
            {
                var tempItem = Inventory.instance.inv_Slot[i];
                if (TryPlaceItem(HotBarSlots, hotitemSet, i, tempItem, itemPrefab)) return;
            }
            
        }
    }


    public bool TryPlaceItem(InventorySlot[] slots, InventoryItem[] itemSet, int index, ItemComponent tempItem, InventoryItem itemPrefab)
    {
        if (slots[index].myItem == null)
        {
            if (itemSet.Length > index && itemSet[index] == null)
            {
                itemSet[index] = Instantiate(itemPrefab, slots[index].transform);
                itemSet[index].Initialize(tempItem, slots[index]);
                return true; // 아이템을 배치했으므로 true 반환
            }
        }
        return false; // 아이템 배치 실패
    }

    public bool TryRemoveItem(InventorySlot[] slots, InventoryItem[] itemSet, int index, out InventoryItem removedItem)
    {
        // out 매개변수를 초기화합니다
        removedItem = null;

        // 지정된 슬롯에 아이템이 있는지 확인합니다
        if (slots[index].myItem != null)
        {
            Debug.Log("removesetSlots : " + index);

            // itemSet에서 지정된 인덱스에 아이템이 있는지 확인합니다
            //if (itemSet.Length > index && itemSet[index] != null)
            //{
            //    // itemSet에서 아이템을 가져옵니다
            //    removedItem = itemSet[index];
            //
            //    // itemSet과 슬롯에서 아이템을 제거합니다
            //    itemSet[index] = null;
            //    slots[index].myItem = null;
            //
            //    // 필요에 따라 아이템의 GameObject를 파괴합니다
            //    Destroy(removedItem.gameObject);
            //
            //    return true; // 아이템이 성공적으로 제거됨을 반환
            //}

            return true;
        }

        return false; // 아이템 제거 실패를 반환
    }


}
