using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        if (this.transform.childCount == 0)
        {
            GameObject obj = eventData.pointerDrag;

            SlotItem item = obj.GetComponent<SlotItem>();

            item.tm = this.transform;
        }
    }
}