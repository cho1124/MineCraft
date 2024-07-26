using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class inventory : MonoBehaviour
{

    public static inventory instance;

    [SerializeField] private ItemComponent[] inv_Slot = new ItemComponent[27];
    [SerializeField] private ItemComponent[] Hotbar_Slot = new ItemComponent[9];
    [SerializeField] private ItemComponent[] Equipment_Slot = new ItemComponent[7];

    public List<ItemComponent> items = new List<ItemComponent>();

    //test
    public InventorySlot[] inv_Slot_ui = new InventorySlot[0];




    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    public delegate void OnChangeItem();
    public OnChangeItem onChangeItem;






    private int slotCount;

    public int SlotCount
    {
        get => slotCount;

        set
        {
            slotCount = value;

        }
    }



    // Start is called before the first frame update
    void Start()
    {
        SlotCount = 4; //일단 튜토따라 테스트
    }

    // Update is called once per frame
    void Update()
    {

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

