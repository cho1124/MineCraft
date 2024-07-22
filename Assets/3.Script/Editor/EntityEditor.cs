using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;

[System.Serializable]
public class Entity
{
    public string type;
    public string name;
    public int health;
    public int damage;
}

[System.Serializable]
public class Player : Entity
{
    public int experience;
}

[System.Serializable]
public class Animal : Entity
{
    public bool isDomestic;
}

[System.Serializable]
public class Monster : Entity
{
    public string spawnLocation;
}

[System.Serializable]
public class EntityData
{
    public Entity[] entities;
}




public class EntityEditor : EditorWindow
{
    private EntityData entityData;
    private string jsonFilePath = "Entities.json";

    [MenuItem("Window/Entity Editor")]
    public static void ShowWindow()
    {
        GetWindow<EntityEditor>("Entity Editor");
    }

    private void OnEnable()
    {
        LoadEntities();
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Load Entities"))
        {
            LoadEntities();
        }

        if (entityData != null && entityData.entities != null)
        {
            foreach (var entity in entityData.entities)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Type:");
                entity.type = GUILayout.TextField(entity.type);
                GUILayout.Label("Name:");
                entity.name = GUILayout.TextField(entity.name);
                GUILayout.Label("Health:");
                entity.health = EditorGUILayout.IntField(entity.health);
                GUILayout.Label("Damage:");
                entity.damage = EditorGUILayout.IntField(entity.damage);

                if (entity is Player player)
                {
                    GUILayout.Label("Experience:");
                    player.experience = EditorGUILayout.IntField(player.experience);
                }
                else if (entity is Animal animal)
                {
                    GUILayout.Label("Is Domestic:");
                    animal.isDomestic = EditorGUILayout.Toggle(animal.isDomestic);
                }
                else if (entity is Monster monster)
                {
                    GUILayout.Label("Spawn Location:");
                    monster.spawnLocation = GUILayout.TextField(monster.spawnLocation);
                }

                GUILayout.EndHorizontal();
            }

            if (GUILayout.Button("Save Entities"))
            {
                SaveEntities();
            }
        }
    }

    private void LoadEntities()
    {
        entityData = JsonHelper.LoadFromJson(jsonFilePath);
    }

    private void SaveEntities()
    {
        JsonHelper.SaveToJson(entityData, jsonFilePath);
    }
}

public static class JsonHelper
{
    public static EntityData LoadFromJson(string path)
    {
        string fullPath = Path.Combine(Application.dataPath, path);
        if (File.Exists(fullPath))
        {
            string json = File.ReadAllText(fullPath);
            return JsonUtility.FromJson<EntityData>(json);
        }
        else
        {
            Debug.LogError("JSON file not found");
            return null;
        }
    }

    public static void SaveToJson(EntityData data, string path)
    {
        string fullPath = Path.Combine(Application.dataPath, path);
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(fullPath, json);
    }
}
