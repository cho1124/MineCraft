using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Trash : MonoBehaviour, IPointerClickHandler
{
    private UIManager Inven;


    private void Awake()
    {
        Inven = FindObjectOfType<UIManager>();
    }

    // Start is called before the first frame update
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (Inven.carriedItem == null) return;


            Destroy(Inven.carriedItem);

            //Debug.Log("Clicked slot index: " + Inven.carriedItem.name);



            //SetItem(Inven.carriedItem);

        }
    }
}
