using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        if (this.transform.childCount == 0)
        {
            GameObject obj = eventData.pointerDrag;

            InventoryItemControl item = obj.GetComponent<InventoryItemControl>();

            item.tm = this.transform;
        }
        else
        {
            // 아이템 중복 체크 로직 요청
        }
    }
}