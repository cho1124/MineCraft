using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ESC : MonoBehaviour
{
    [SerializeField] private GameObject manu_obj = null;

    private bool on_off_tr = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            on_off_tr = !on_off_tr;

            manu_obj.SetActive(on_off_tr);
        }
    }
}
