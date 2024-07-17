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

    [MenuItem("Window/JSON Weapon Editor")] //유니티의 탭/항목이름
    public static void ShowWindow()
    {
        GetWindow<JsonEditorWindow>("JSON Weapon Editor"); //에디터 켜졌을때 상단 탭
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

        // 기존 파일이 존재하는 경우 데이터 읽기
        if (File.Exists(path))
        {
            string existingJson = File.ReadAllText(path);
            wrapper = JsonConvert.DeserializeObject<WeaponDictionaryWrapper>(existingJson) ?? new WeaponDictionaryWrapper();
            //      '??' : null 병합 연산자로 null이 아니면 왼쪽값을 반환 / null이면 오른쪽 값을 반환
        }

        // 새로운 무기 데이터 추가 또는 업데이트
        if (wrapper.weaponDictionary.ContainsKey(weaponName))
        {
            wrapper.weaponDictionary[weaponName] = weaponData;
        }
        else
        {
            wrapper.weaponDictionary.Add(weaponName, weaponData);
        }

        // 딕셔너리를 JSON 형식으로 변환
        string jsonString = JsonConvert.SerializeObject(wrapper, Formatting.Indented);

        // JSON 파일로 저장
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
