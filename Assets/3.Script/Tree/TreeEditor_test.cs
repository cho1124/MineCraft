using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;

public class TreeEditor_test : EditorWindow
{

    private string jsonFileName = "TreeData.json";
    private TreeData treedata = new TreeData();
  
    [MenuItem("Window/Tree Editor")]
    public static void ShowWindow() 
    {
        GetWindow<TreeEditor_test>("Tree Editor");
    
    }

    private void OnGUI()
    {
        GUILayout.Label("Create or Update Tree(or Plant) Data", EditorStyles.boldLabel);

        jsonFileName = EditorGUILayout.TextField("File Name", jsonFileName);
        treedata.tree_key = EditorGUILayout.IntField("Tree Key", treedata.tree_key);
        treedata.tree_hp = EditorGUILayout.IntField("Tree HP", treedata.tree_hp);
        treedata.tree_itemcount = EditorGUILayout.IntField("Tree Item Count", treedata.tree_itemcount);

        if(GUILayout.Button("Save to Json"))
        {
            SaveJsonFile();
        }
    }

    private void SaveJsonFile()
    {
        string path = Application.dataPath + "/" + jsonFileName;
        TreeDictionaryWrapper wrapper = new TreeDictionaryWrapper();

        if(File.Exists(path))
        {
            string existingJson=File.ReadAllText(path);
            wrapper = JsonConvert.DeserializeObject<TreeDictionaryWrapper>(existingJson) ?? new TreeDictionaryWrapper();

        }

        if(wrapper.TreeDictionary.ContainsKey(treedata.tree_key))
        {
            wrapper.TreeDictionary[treedata.tree_key] = treedata;
        }
        else
        {
            wrapper.TreeDictionary.Add(treedata.tree_key, treedata);
        }

        string jsonString = JsonConvert.SerializeObject(wrapper, Newtonsoft.Json.Formatting.Indented);

        File.WriteAllText(path, jsonString);
        AssetDatabase.Refresh();

    } 
} 
  
   [System.Serializable]
   public class TreeData 
    {
    public int tree_key; //나무 키값
    public int tree_hp;  //나무 hp
    public int tree_itemcount;  // 나무가 주는 아이템 갯수

    public void TakeDamage(int damage)
    {
        tree_hp -= damage;
        if(tree_hp<=0)
        {
            DropItems();
        }
    }
    private void DropItems()
    {
        //드롭할 아이템 구상중
    }
    }

[System.Serializable]
public class TreeDictionaryWrapper
{
    public Dictionary<int, TreeData> TreeDictionary = new Dictionary<int, TreeData>();
}