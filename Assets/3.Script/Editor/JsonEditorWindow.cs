using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;

public class JsonEditorWindow : EditorWindow
{
    private string jsonFileName = "WeaponData.json";
    private string weaponName = "";
    private WeaponData weaponData = new WeaponData();

    [MenuItem("Window/JSON Weapon Editor")] //����Ƽ�� ��/�׸��̸�
    public static void ShowWindow()
    {
        GetWindow<JsonEditorWindow>("JSON Weapon Editor"); //������ �������� ��� ��
    }

    private void OnGUI()
    {
        GUILayout.Label("Create or Update Weapon Data", EditorStyles.boldLabel);

        jsonFileName = EditorGUILayout.TextField("File Name", jsonFileName);
        weaponName = EditorGUILayout.TextField("Weapon Name", weaponName);

        weaponData.attack_damage = EditorGUILayout.FloatField("Attack Damage", weaponData.attack_damage);
        weaponData.attack_speed = EditorGUILayout.FloatField("Attack Speed", weaponData.attack_speed);
        weaponData.guard_rate = EditorGUILayout.FloatField("Guard Rate", weaponData.guard_rate);
        weaponData.weight = EditorGUILayout.FloatField("Weight", weaponData.weight);
        weaponData.durability_max = EditorGUILayout.FloatField("Durability Max", weaponData.durability_max);
        weaponData.durability_current = EditorGUILayout.FloatField("Durability Current", weaponData.durability_current);

        if (GUILayout.Button("Save JSON"))
        {
            SaveJsonFile();
        }
    }

    private void SaveJsonFile()
    {
        string path = Application.dataPath + "/" + jsonFileName;
        WeaponDictionaryWrapper wrapper = new WeaponDictionaryWrapper();

        // ���� ������ �����ϴ� ��� ������ �б�
        if (File.Exists(path))
        {
            string existingJson = File.ReadAllText(path);
            wrapper = JsonConvert.DeserializeObject<WeaponDictionaryWrapper>(existingJson) ?? new WeaponDictionaryWrapper();
            //      '??' : null ���� �����ڷ� null�� �ƴϸ� ���ʰ��� ��ȯ / null�̸� ������ ���� ��ȯ
        }

        // ���ο� ���� ������ �߰� �Ǵ� ������Ʈ
        if (wrapper.weaponDictionary.ContainsKey(weaponName))
        {
            wrapper.weaponDictionary[weaponName] = weaponData;
        }
        else
        {
            wrapper.weaponDictionary.Add(weaponName, weaponData);
        }

        // ��ųʸ��� JSON �������� ��ȯ
        string jsonString = JsonConvert.SerializeObject(wrapper, Formatting.Indented);

        // JSON ���Ϸ� ����
        File.WriteAllText(path, jsonString);
        AssetDatabase.Refresh();
        Debug.Log("JSON file saved at " + path);
    }
}

[System.Serializable]
public class WeaponData
{
    public float attack_damage;
    public float attack_speed;
    public float guard_rate;
    public float weight;
    public float durability_max;
    public float durability_current;
}

[System.Serializable]
public class WeaponDictionaryWrapper
{
    public Dictionary<string, WeaponData> weaponDictionary = new Dictionary<string, WeaponData>();
}
