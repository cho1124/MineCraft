/* using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryBarSlotChoice : MonoBehaviour
{
    // ========== Inspector public ==========

    [SerializeField] private GameObject[] item_obj = null;
    [SerializeField] private Inventory Inventory_class = null;
    [SerializeField] private Transform[] inventory_bar_slot = null;
    [SerializeField] private GameObject player = null;
    [SerializeField] private int throw_power = 0;
    [SerializeField] private float throw_distance = 0;

    // ========== Inspector private ==========

    private Vector3[] slot_choice_pos = new Vector3[9];

    private void Start()
    {
        // 설계 사정으로 수동으로 좌표를 구했습니다.

        slot_choice_pos[0] = new Vector3(-400, -485, 0);
        slot_choice_pos[1] = new Vector3(-300, -485, 0);
        slot_choice_pos[2] = new Vector3(-200, -485, 0);
        slot_choice_pos[3] = new Vector3(-100, -485, 0);
        slot_choice_pos[4] = new Vector3(0, -485, 0);
        slot_choice_pos[5] = new Vector3(100, -485, 0);
        slot_choice_pos[6] = new Vector3(200, -485, 0);
        slot_choice_pos[7] = new Vector3(300, -485, 0);
        slot_choice_pos[8] = new Vector3(400, -485, 0);

        this.transform.localPosition = slot_choice_pos[0];
    }

    private void Update()
    {
        DropItem();
    }

    private void DropItem()
    {
        if (!Inventory_class.on_off_tr)
        {
            for (int i = 0; i < slot_choice_pos.Length; i++)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1 + i))
                {
                    this.transform.localPosition = slot_choice_pos[i];

                    break;
                }

                if (Input.GetKeyDown(KeyCode.Q))
                {
                    if (this.transform.localPosition == slot_choice_pos[i] && inventory_bar_slot[i].childCount == 1)
                    {
                        Transform itemTransform = inventory_bar_slot[i].GetChild(0);

                        if (itemTransform == null)
                        {
                            return;
                        }

                        GameObject item = itemTransform.gameObject;

                        ItemInfo iteminfo = item.GetComponent<ItemInfo>();

                        if (iteminfo == null)
                        {
                            Destroy(item);
                        }

                        if (item_obj[iteminfo.item_id] == null)
                        {
                            Debug.Log(item_obj[iteminfo.item_id]);

                            // Destroy(item);
                        }

                        GameObject new_item_obj = Instantiate(item_obj[iteminfo.item_id]);

                        Vector3 playerPosition = player.transform.position;
                        Vector3 playerForward = player.transform.forward;
                        Vector3 dropPosition = playerPosition + playerForward * throw_distance;

                        new_item_obj.transform.position = dropPosition;

                        new_item_obj.transform.SetParent(null);

                        Rigidbody ry = new_item_obj.GetComponent<Rigidbody>();

                        ry.AddForce(playerForward * throw_power, ForceMode.Impulse);

                        Destroy(item);

                        Debug.Log($"{item.name} 아이템을 버렸습니다.");

                        break;
                    }
                }
            }
        }
    }
} */