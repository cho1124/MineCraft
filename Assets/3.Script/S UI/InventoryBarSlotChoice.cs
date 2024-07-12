using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class InventoryBarSlotChoice : MonoBehaviour
{
    private Vector3[] slot_choice_pos = null;

    private void Start()
    {
        slot_choice_pos = new Vector3[9];

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
        for (int i = 1; i <= 9; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha0 + i))
            {
                this.transform.localPosition = slot_choice_pos[i - 1];
            }
        }
    }
}







/* using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class InventoryBarSlotChoice : MonoBehaviour
{
    private Vector3[] slot_choice_pos = null;

    [Header("µð¹ö±×")]
    [SerializeField] private bool[] slot_choice_tr = { false };

    private void Start()
    {
        slot_choice_pos = new Vector3[9];
        slot_choice_tr = new bool[9];

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
        for (int i = 1; i <= 9; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha0 + i))
            {
                slot_choice_tr[i - 1] = true;

                this.transform.localPosition = slot_choice_pos[i - 1];

                Debug.Log(slot_choice_tr[i - 1]);

                slot_choice_tr[i - 1] = false;
            }
        }
    }
} */