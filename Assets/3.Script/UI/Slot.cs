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

            Slot_item item = obj.GetComponent<Slot_item>();

            item.tm = this.transform;
        }
    }
}