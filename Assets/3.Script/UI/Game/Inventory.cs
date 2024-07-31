using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;

public class Inventory : MonoBehaviour
{

    public static Inventory instance;


    

    public ItemComponent[] inv_Slot = new ItemComponent[36]; // �ֹٶ� ���ļ� �ѹ��� ó������
    public ItemComponent[] Equipment_Slot = new ItemComponent[6]; //0 : head, 1 : chest, 2 : legs, 3 : feet, 4 : weapon1, 5 : weapon2
    public ItemComponent[] Crafting_Mini_Slot = new ItemComponent[4];
    public Transform[] Equipment_Part = new Transform[6];

    private Item_Manager itemManager;


    public Transform Inventory_obj;

    public delegate void HotBarUpdate();
    public event HotBarUpdate OnChangedInv;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        itemManager = FindObjectOfType<Item_Manager>();

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
                        Debug.Log($"{i}�� ������ ���� id : {inv_Slot[i].ItemID}");

                        item.transform.SetParent(Inventory_obj);
                        item.gameObject.SetActive(false);
                        //Destroy(item.gameObject);
                        ChangeEvent(); //ī��Ʈ�� �Ұž�

                        return;
                    }
                }
                //����Ŀ�� üũ
            }
        }

           
    }

    public GameObject DestroyItem(ItemComponent item)
    {
        //�κ��丮 ���Կ��� ������ ���� �߰�
        GameObject newobj = itemManager.SpawnItem(item.ItemID, transform.position);
        ChangeEvent();
        return newobj;
    }

    public void ChangeEvent()
    {
        OnChangedInv?.Invoke();
    }

    
}




