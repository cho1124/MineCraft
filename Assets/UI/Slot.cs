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

            Item item = obj.GetComponent<Item>();

            item.tm = this.transform;
        }
    }
}