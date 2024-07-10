using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Game_UI_manager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;

    // [SerializeField] TextMeshProUGUI[] text;

    private void Awake()
    {
        text = this.gameObject.GetComponent<TextMeshProUGUI>();

        /* for (int i = 0; i < text.Length; i++)
        {
            text[i] = this.gameObject.GetComponent<TextMeshProUGUI>();
        } */
    }

    private void Start()
    {
        text.text += Game_UI_data_manager.instance.shame_data.level.ToString();

        // text[0].text += Game_UI_data_manager.instance.shame_data.blood;
        // text[1].text += Game_UI_data_manager.instance.shame_data.level;
        // text[2].text += Game_UI_data_manager.instance.shame_data.exp;
    }


}