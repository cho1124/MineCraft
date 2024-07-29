using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class InventoryOnOff : MonoBehaviour
{
    [SerializeField] private CanvasGroup inventoryCanvasGroup = null;
    [HideInInspector] public bool on_off_tr = false; // 인벤토리, Dead UI ON, OFF 선언 (Bool)

    private bool isInventoryOpen = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            ToggleInventory();
        }
    }

    void ToggleInventory()
    {
        isInventoryOpen = !isInventoryOpen;
        if (isInventoryOpen)
        {
            OpenInventory();
        }
        else
        {
            CloseInventory();
        }
    }

    void OpenInventory()
    {
        inventoryCanvasGroup.alpha = 1;
        inventoryCanvasGroup.interactable = true;
        inventoryCanvasGroup.blocksRaycasts = true;
    }

    void CloseInventory()
    {
        inventoryCanvasGroup.alpha = 0;
        inventoryCanvasGroup.interactable = true;
        inventoryCanvasGroup.blocksRaycasts = false;
    }

}