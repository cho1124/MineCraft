using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Inventory : MonoBehaviour
{

    public static Inventory instance;

    public ItemComponent[] inv_Slot = new ItemComponent[36]; // 핫바랑 합쳐서 한번에 처리하자
    
    public ItemComponent[] Equipment_Slot = new ItemComponent[7];

    public List<ItemComponent> items = new List<ItemComponent>();

    public Transform Inventory_obj;

    public delegate void HotBarUpdate();
    public event HotBarUpdate OnHotBarChanged;


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
       
        for(int i = 0; i < inv_Slot.Length; i++)
        {
            if (inv_Slot[i] == null)
            {
                inv_Slot[i] = item;
                item.transform.SetParent(Inventory_obj);
                item.gameObject.SetActive(false);
                Debug.Log(inv_Slot[i].StackCurrent);
                ChangeEvent();

                return;
            }
            else if (inv_Slot[i].Get_Type() == item.Get_Type() && inv_Slot[i].Get_Type() != 4) //4가 뭐였지
            {
                while (item.StackCurrent > 0 && inv_Slot[i].Check_Full())
                {
                    item.StackCurrent--;
                    //inv_Slot[i].StackCurrent++;
                    if (item.StackCurrent == 0)
                    {
                        Debug.Log(inv_Slot[i].StackCurrent);

                        item.transform.SetParent(Inventory_obj);
                        item.gameObject.SetActive(false);
                        ChangeEvent(); //카운트도 할거야
                        return;
                    }
                }
                //스태커블 체크
            }
        }

           
    }

    public void ChangeEvent()
    {
        OnHotBarChanged?.Invoke();
    }

    public ItemComponent[] GetInv_Main()
    {
        return inv_Slot;
    }



}

