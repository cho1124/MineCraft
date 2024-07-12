using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    private InventoryItemControl InventoryItemControl_class;

    [Header("Á¶ÇÕ ½½·Ô"), SerializeField] private Transform[] crafting_slot;
    [Header("Á¶ÇÕ ½½·Ô °á°ú"), SerializeField] private GameObject new_item_obj;
    [Header("ÆË¾÷"), SerializeField] private GameObject on_off_obj;

    [Header("µð¹ö±×")]
    public bool on_off_tr = false;
    [SerializeField] private bool full_crafting_slot = false;

    private void Awake()
    {
        InventoryItemControl_class = FindObjectOfType<InventoryItemControl>();
    }

    private void Start()
    {
        on_off_obj.SetActive(false);
        new_item_obj.SetActive(false);
    }

    private void Update()
    {
        if (on_off_tr)
        {
            Crafting_slot();
            Delete_item();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            on_off_tr = !on_off_tr;

            on_off_obj.SetActive(on_off_tr);
        }
    }

    private void Crafting_slot()
    {
        if (crafting_slot[0].childCount == 1 && crafting_slot[1].childCount == 1 && crafting_slot[2].childCount == 1 && crafting_slot[3].childCount == 1)
        {
            full_crafting_slot = true;
            new_item_obj.SetActive(true);
        }
        else
        {
            full_crafting_slot = false;
            new_item_obj.SetActive(false);
            
        }
    }

    private void Delete_item()
    {
        if (InventoryItemControl_class.click_tr && full_crafting_slot)
        {
            for (int i = 0; i < crafting_slot.Length; i++)
            {
                Destroy(crafting_slot[i].GetChild(0).gameObject);
            }
        }
    }
}