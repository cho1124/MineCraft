using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory_manager : MonoBehaviour
{
    public static Inventory_manager instance = null;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;

            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    [SerializeField] Transform[] crafting_slot;
    [SerializeField] GameObject[] on_off_obj;
    [SerializeField] GameObject new_item_obj;
    [SerializeField] GameObject player_pos_obj;

    public bool click_tr = false;
    [SerializeField] bool full_crafting_slot = false;
    bool on_off_tr = false;

    private void Update()
    {
        Crafting_slot();
        Delete_item();

        if (Input.GetKeyDown(KeyCode.E))
        {
            on_off_tr = !on_off_tr;

            on_off_obj[0].SetActive(on_off_tr);
            on_off_obj[1].SetActive(on_off_tr);

            if (on_off_tr)
            {
                player_pos_obj.transform.position = new Vector3(-1.84f, 2.87f, -0.58f);
            }
            else
            {
                player_pos_obj.transform.position = Vector3.zero;
            }
        }
    }

    public void Crafting_slot()
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

    public void Delete_item()
    {
        if (click_tr && full_crafting_slot)
        {
            for (int i = 0; i < crafting_slot.Length; i++)
            {
                Destroy(crafting_slot[i].GetChild(0).gameObject);
            }
        }
    }
}