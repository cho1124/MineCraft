using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;

public class EntityManager : MonoBehaviour //JSON 데이터를 로드하고 엔티티들을 초기화
{
    private string jsonFilePath = "Entities.json";
    private Dictionary<string, Entity> entities = new Dictionary<string, Entity>();

    void Start() {
        LoadEntities();
        InitializeEntities();
    }

    private void LoadEntities() {
        string fullPath = Path.Combine(Application.dataPath, jsonFilePath);
        if (File.Exists(fullPath)) {
            string json = File.ReadAllText(fullPath);
            EntityData entityData = JsonConvert.DeserializeObject<EntityData>(json);
            foreach (var entity in entityData.entities) {
                entities[entity.name] = entity;
            }
        }
        else {
            Debug.LogError("JSON file not found");
        }
    }

    private void InitializeEntities() {
        Entity[] sceneEntities = FindObjectsOfType<Entity>();
        foreach (var sceneEntity in sceneEntities) {
            if (entities.ContainsKey(sceneEntity.name)) {
                Entity jsonEntity = entities[sceneEntity.name];
                if (sceneEntity is Player player && jsonEntity is Player jsonPlayer) {
                    player.Initialize(jsonPlayer);
                }
                else if (sceneEntity is Animal animal && jsonEntity is Animal jsonAnimal) {
                    animal.Initialize(jsonAnimal);
                }
                else if (sceneEntity is Monster monster && jsonEntity is Monster jsonMonster) {
                    monster.Initialize(jsonMonster);
                }
                else {
                    sceneEntity.Initialize(jsonEntity);
                }
            }
        }
    }
}
