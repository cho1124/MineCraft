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


    /// <summary>
    /// hotbar -> inven -> 그 다음에 자유롭게 이동 가능 -> 추가로 equipment는 ㅁㄴ엄녀온먀ㅕ온ㅁ어
    /// </summary>
    /// <param name="item"></param>

    public void AddItem(ItemComponent item)
    {
        for (int i = 0; i < Hotbar_Slot.Length; i++)
        {
            if (Hotbar_Slot[i] == null)
            {
                Hotbar_Slot[i] = item;
                item.gameObject.SetActive(false);
                return;
            }
            else if (Hotbar_Slot[i].Get_Type() == item.Get_Type() && Hotbar_Slot[i].Get_Type() != 4)
            {
                while (item.StackCurrent > 0 && Hotbar_Slot[i].Check_Full())
                {
                    item.StackCurrent--;
                    //inv_Slot[i].StackCurrent++;
                    if (item.StackCurrent == 0)
                    {
                        Debug.Log(Hotbar_Slot[i].StackCurrent);
                        item.gameObject.SetActive(false);
                        return;
                    }
                }
            }


        }

        for(int i = 0; i < inv_Slot.Length; i++)
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
                //스태커블 체크
            }
        }

            

        
    }
    public ItemComponent[] GetInv_Main()
    {
        return inv_Slot;
    }
}

