using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{

    public static Inventory instance;

    public ItemComponent[] inv_Slot = new ItemComponent[27];
    public ItemComponent[] Hotbar_Slot = new ItemComponent[9];
    public ItemComponent[] Equipment_Slot = new ItemComponent[7];

    public List<ItemComponent> items = new List<ItemComponent>();

    

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    private int slotCount;

    public int SlotCount
    {
        get => slotCount;

        set
        {
            slotCount = value;

        }
    }
    public void AddItem(ItemComponent item)
    {
        for (int i = 0; i < inv_Slot.Length; i++)
        {
            if (inv_Slot[i] == null)
            {
                inv_Slot[i] = item;
                item.gameObject.SetActive(false);
                Debug.Log(inv_Slot[i].StackCurrent);
                
                return;
            }
            else if (inv_Slot[i].Get_Type() == item.Get_Type() && inv_Slot[i].Get_Type() != 4)
            {
                while (item.StackCurrent > 0 && inv_Slot[i].Check_Full())
                {
                    item.StackCurrent--;
                    //inv_Slot[i].StackCurrent++;
                    if (item.StackCurrent == 0)
                    {
                        Debug.Log(inv_Slot[i].StackCurrent);
                        item.gameObject.SetActive(false);
                        return;
                    }
                }
            }

        }
    }
    public ItemComponent[] GetInv_Main()
    {
        return inv_Slot;
    }
}

