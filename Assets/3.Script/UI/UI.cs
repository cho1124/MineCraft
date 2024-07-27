using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI : MonoBehaviour
{
    public Player player;
    //private void Start()
    //{
    //    player = GameObject.Find("Player").GetComponent<Player>();
    //}
    

    void Update()
    {
        UIActive();
    }

    void UIActive()
    {
        if(gameObject.activeSelf)
        {

            player.isOpeningUI = true;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;


            if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Escape))
            {
                Cursor.visible = false;
                player.isOpeningUI = false;
                Cursor.lockState = CursorLockMode.Locked;
                gameObject.SetActive(false);

            }
        }
    }
}
