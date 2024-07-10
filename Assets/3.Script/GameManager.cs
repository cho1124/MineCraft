using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    public static GameManager instance = null;

    [SerializeField] GameObject Inventory_obj;
    [SerializeField] GameObject[] inventory_player; 

    bool on_off = false;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            on_off = !on_off;

            for (int i = 0; i < inventory_player.Length; i++)
            {
                inventory_player[i].SetActive(on_off);
            }

            Inventory_obj.SetActive(on_off);
        }
    }
}