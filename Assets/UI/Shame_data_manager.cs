using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;

public class Data_manager : MonoBehaviour
{
    public static Data_manager instance = null;

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

    [SerializeField] 

    Shame_data shame_data = new Shame_data();

    private void Start()
    {
        Load_data();
    }

    public void Save_data()
    {
        string save_data = JsonUtility.ToJson(shame_data);

        File.WriteAllText(Application.persistentDataPath + "/Shame data", save_data);
    }

    public void Load_data()
    {
        string load_data = File.ReadAllText(Application.persistentDataPath + "/Shame data");

        shame_data = JsonUtility.FromJson<Shame_data>(load_data);
    }

    public void Set_level()
    {
        shame_data.level += 1;

        Debug.Log($"레벨 업!\n레벨: {shame_data.level}");
    }

    public void Set_blood()
    {

    }
}

public class Shame_data
{
    public int level = 1;
    public float blood = 100;
    public float exp = 0;
}