using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class inventory : MonoBehaviour
{

    public static inventory instance;
    private void Awake()
    {
        if(instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    public delegate void OnChangeItem();
    public OnChangeItem onChangeItem;
    

    public List<ItemComponent> items = new List<ItemComponent>();



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

    public bool AddItem(ItemComponent item)
    {
        if(items.Count < SlotCount)
        {
            items.Add(item);

            if(onChangeItem != null)
            {
                onChangeItem.Invoke();

            }
            return true;
        }
        return false;
    }

    

}
