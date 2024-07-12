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
            // ������ �ߺ� üũ ���� ��û
        }
    }
}