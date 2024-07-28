using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Inventory : MonoBehaviour
{

    public static Inventory instance;

    public ItemComponent[] inv_Slot = new ItemComponent[36]; // �ֹٶ� ���ļ� �ѹ��� ó������
    
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

    

    /// <summary>
    /// hotbar -> inven -> �� ������ �����Ӱ� �̵� ����
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
                
                ChangeEvent();

                return;
            }
            else if (inv_Slot[i].ItemID == item.ItemID && inv_Slot[i].Get_Type() != 4)
            {
                while (item.StackCurrent > 0 && inv_Slot[i].Check_Full())
                {
                    item.StackCurrent--;
                    //inv_Slot[i].StackCurrent++;
                    if (item.StackCurrent == 0)
                    {
                        Debug.Log($"{i}�� ������ ���� ���� ī��Ʈ : " + inv_Slot[i].StackCurrent);

                        item.transform.SetParent(Inventory_obj);
                        item.gameObject.SetActive(false);
                        ChangeEvent(); //ī��Ʈ�� �Ұž�
                        return;
                    }
                }
                //����Ŀ�� üũ
            }
        }

           
    }

    public void ChangeEvent()
    {
        OnHotBarChanged?.Invoke();
    }

}

