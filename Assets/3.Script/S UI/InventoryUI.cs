using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    // ========== Inspector public ==========

    public Slot[] slot_count = null;
    public Inventory inventory_class = null;
    public Transform[] slotHolders = null; // Transform 배열로 변경

    private void Start()
    {
        List<Slot> slots = new List<Slot>();

        // 각 slotHolder에서 Slot 배열을 가져와 리스트에 추가
        foreach (Transform slotHolder in slotHolders)
        {
            slots.AddRange(slotHolder.GetComponentsInChildren<Slot>());
        }

        slot_count = slots.ToArray();

        inventory_class.onchangeitem += RedrawSlotUI;
    }

    private void RedrawSlotUI()
    {
        for (int i = 0; i < slot_count.Length; i++) // slot_count 크기만큼 실행
        {
            slot_count[i].RemoveSlot();
        }

        for (int i = 0; i < inventory_class.item_list.Count; i++) // 인벤토리 아이템 개수만큼 실행
        {
            slot_count[i].iteminfo_class = inventory_class.item_list[i];
            slot_count[i].UpdateSlotUI();
        }
    }
}
