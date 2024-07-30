using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class InventoryOnOff : MonoBehaviour
{

    [SerializeField] private CanvasGroup backGroundCanvasGroup = null;
    [SerializeField] private CanvasGroup inventoryCanvasGroup = null;
    [SerializeField] private CanvasGroup craftingCanvasGroup = null;
    [SerializeField] private CanvasGroup chestCanvasGroup = null;
    [SerializeField] private CanvasGroup hwaroGroup = null;
    [HideInInspector] public bool on_off_tr = false; // 인벤토리, Dead UI ON, OFF 선언 (Bool)

    private bool isInventoryOpen = false;
    private bool isCraftingOpen = false;
    private bool isChestOpen = false;
    private bool isHwaroOpen = false;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            ToggleInventory();
        }
        //임시로 만든 것
        if(Input.GetKeyDown(KeyCode.C))
        {
            ToggleCrafting();
        }

    }

    void ToggleInventory()
    {
        isInventoryOpen = !isInventoryOpen;

        SwitchBackGroundCanvas(isInventoryOpen);

        SwitchInventory(isInventoryOpen);
    }

    void ToggleCrafting()
    {
        isCraftingOpen = !isCraftingOpen;
        SwitchBackGroundCanvas(isCraftingOpen);
        if (isCraftingOpen)
        {
            OpenCrafting();
        }
        else
        {
            CloseCrafting();
        }
    }

    void SwitchBackGroundCanvas(bool isOpen)
    {
        if(isOpen)
        {
            backGroundCanvasGroup.alpha = 1;
        }
        else
        {
            backGroundCanvasGroup.alpha = 0;
        }

        
        backGroundCanvasGroup.interactable = isOpen;
        backGroundCanvasGroup.blocksRaycasts = isOpen;
    }

    void SwitchInventory(bool isOpen)
    {
        if(isOpen)
        {
            inventoryCanvasGroup.alpha = 1;
        }
        else
        {
            inventoryCanvasGroup.alpha = 0;
        }

        
        inventoryCanvasGroup.interactable = isOpen;
        inventoryCanvasGroup.blocksRaycasts = isOpen;
    }

    void OpenCrafting()
    {
        craftingCanvasGroup.alpha = 1;
        craftingCanvasGroup.interactable = true;
        craftingCanvasGroup.blocksRaycasts = true;
    }

    void CloseCrafting()
    {
        craftingCanvasGroup.alpha = 0;
        craftingCanvasGroup.interactable = false;
        craftingCanvasGroup.blocksRaycasts = false;
    }


}