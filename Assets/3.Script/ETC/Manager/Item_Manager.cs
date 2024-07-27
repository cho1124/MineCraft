using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;
using UnityEngine.UI;

public static class Item_Dictionary
{
    public static Dictionary<int, Original_Item> item_dictionary { get; private set; }
    // 딕셔너리 초기화
    static Item_Dictionary()
    {
        item_dictionary = new Dictionary<int, Original_Item>();
        LoadResources();
    }

    private static void LoadResources()
    {
        // 모든 아이템의 리소스를 미리 로드
        foreach (var item in item_dictionary.Values)
        {
            item.item_model_in_worlds = Resources.Load<GameObject>(item.item_model_in_world);
            item.item_model_in_inv = Resources.Load<Sprite>(item.item_model_in_inventory);
        }
    }

    public static void Add(int item_ID, Original_Item item_data)
    {
        if (!item_dictionary.ContainsKey(item_ID))
        {
            item_dictionary.Add(item_ID, item_data);

            item_dictionary[item_ID].item_model_in_worlds = Resources.Load<GameObject>(item_dictionary[item_ID].item_model_in_world);
            item_dictionary[item_ID].item_model_in_inv = Resources.Load<Sprite>(item_dictionary[item_ID].item_model_in_inventory);

        }
        else
        {
            Debug.LogWarning($"Item ID {item_ID} already exists in the dictionary.");
        }
    }

}





public class Item_Manager : MonoBehaviour
{
    public TextAsset text1;
    [Header("테스트 해보세용")]
    public int TestKey = 7;

    private void Awake()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("ItemData"); // 테스트용
        if (jsonFile == null)
        {
            Debug.LogError("Could not find JSON file in Resources folder.");
            return;
        }

        // JSON 파일의 텍스트 내용을 문자열로 변환
        string jsonString = jsonFile.text;

        // JSON 문자열을 ItemData 객체로 파싱
        ItemData itemData = JsonConvert.DeserializeObject<ItemData>(jsonString);


        Debug.Log("consume : " + itemData.consumableItems[0].item_name);
        Debug.Log("place : " + itemData.placeableItems.Count);
        Debug.Log("equipmentitemcount : " + itemData.equipmentItems.Count);

        // 파싱된 데이터 사용 예제

        Dic_Add(itemData.stackableItems);

        Dic_Add(itemData.consumableItems);

        Dic_Add(itemData.placeableItems);

        Dic_Add(itemData.equipmentItems);

        //Debug.Log(Item_Dictionary.item_dictionary[7].item_ID);

        SpawnItem(TestKey, transform.position);
    }



    void Start()
    {
        for(int i = 0; i < 400; i++)
        {
            SpawnItem(i, transform.position);
        }


    }


    //테스트용 업데이트 ->>>> 디버그할 땐 꼭 지울 것
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            SpawnItem(TestKey, transform.position);
        }

       
    }


    public void SetLayerToItem(GameObject obj, string layer)
    {

        int itemLayer = LayerMask.NameToLayer(layer);

        if (itemLayer != -1)
        {
            obj.layer = itemLayer;
            Debug.Log($"{obj.name} layer set to {layer}");
        }
        else
        {
            Debug.LogError($"Layer {layer} does not exist!");
        }
    }


    public void SpawnItem(int itemID, Vector3 position)
    {
        if (Item_Dictionary.item_dictionary.ContainsKey(itemID))
        {
            GameObject itemPrefab = Item_Dictionary.item_dictionary[itemID].item_model_in_worlds;
            if (itemPrefab != null)
            {
                GameObject spawnedItem = Instantiate(itemPrefab, position, Quaternion.identity);
                SetLayerToItem(spawnedItem, "Item");
                ItemComponent itemComponent = spawnedItem.AddComponent<ItemComponent>();

                Original_Item itemData = Item_Dictionary.item_dictionary[itemID];
                if (itemData is ConsumableItem consumableItem)
                {
                    itemComponent.Initialize(consumableItem);
                }
                else if (itemData is PlaceableItem placeableItem)
                {
                    itemComponent.Initialize(placeableItem);
                }
                else if (itemData is EquipmentItem equipmentItem)
                {
                    itemComponent.Initialize(equipmentItem);
                }
                else if (itemData is StackableItem stackableItem)
                {
                    itemComponent.Initialize(stackableItem);
                }
                else
                {
                    Debug.LogWarning($"타입이 뭔지 모르겠어잉 {itemID}.");
                }

            }
            else
            {
                Debug.LogWarning($"Item ID {itemID} 프리팹 없어잉");
            }
        }
        else
        {
            Debug.LogWarning($"Item ID {itemID} 없어잉");
        }
    }


    private void Dic_Add(List<StackableItem> stackableItems)
    {
        foreach (var item in stackableItems)
        {
            Debug.Log("SADAUSHVUC : " + item.item_name);




            Item_Dictionary.Add(item.item_ID, item);


        }
    }

    private void Dic_Add(List<ConsumableItem> consumableItems)
    {
        foreach (var item in consumableItems)
        {
            Debug.Log("Consumable Item Name: " + item.item_name);

            Item_Dictionary.Add(item.item_ID, item);

        }
    }

    private void Dic_Add(List<PlaceableItem> placeableItems)
    {

        foreach (var item in placeableItems)
        {
            Debug.Log("Placeable Item Name: " + item.item_name);

            Item_Dictionary.Add(item.item_ID, item);
        }


    }

    private void Dic_Add(List<EquipmentItem> equipmentItems)
    {
        foreach (var item in equipmentItems)
        {
            Debug.Log("Equipment Item ID: " + item.item_ID);

            // equipment_type에 따라 열거형 값을 설정
            EquipmentItem equipmentItem = item;
            if (equipmentItem != null)
            {
                // TryParse를 사용하여 안전하게 열거형으로 변환
                equipmentItem.Weapon_Type = ParseWeaponType(equipmentItem.equipment_type);
                equipmentItem.Armor_Type = ParseArmorType(equipmentItem.equipment_type);

                if (equipmentItem.Weapon_Type.HasValue)
                {
                    if (equipmentItem.Weapon_Type == Equipment_Weapon_Type.BOW) Debug.Log($"Weapon Item: {equipmentItem.item_name}, draw_power: {equipmentItem.draw_power}, draw_speed: {equipmentItem.draw_speed}, aim_accuracy: {equipmentItem.aim_accuracy}");
                    // 무기 속성 설정
                    else Debug.Log($"Weapon Item: {equipmentItem.item_name}, Damage: {equipmentItem.melee_damage}, Speed: {equipmentItem.melee_speed}, Guard Rate: {equipmentItem.guard_rate}, Tool Tier: {equipmentItem.tool_tier}");
                }
                else if (equipmentItem.Armor_Type.HasValue)
                {
                    // 방어구 속성 설정
                    Debug.Log($"Armor Item: {equipmentItem.item_name}, Defense: {equipmentItem.armor_defense}");
                }
                else
                {
                    Debug.LogWarning($"Unknown equipment type: {equipmentItem.equipment_type}");
                }
            }

            Item_Dictionary.Add(item.item_ID, item);
        }
    }

    //enum parse 개편한 것
    private Equipment_Weapon_Type? ParseWeaponType(string type)
    {
        return Enum.TryParse(type, out Equipment_Weapon_Type result) ? result : (Equipment_Weapon_Type?)null;
    }

    private Equipment_Armor_Type? ParseArmorType(string type)
    {
        return Enum.TryParse(type, out Equipment_Armor_Type result) ? result : (Equipment_Armor_Type?)null;
    }


    public Sprite GetItemImage(string itemName_Inv)
    {
        Sprite image = Resources.Load<Sprite>(itemName_Inv);
        return image;
    }


}
[System.Serializable]
public class Original_Item
{
    [JsonProperty(Order = 1)]
    public int item_ID { get;  set; } //아이템 아이디
    [JsonProperty(Order = 2)]
    public string item_name { get;  set; } //아이템 이름
    [JsonProperty(Order = 3)]
    public string item_model_in_world { get;  set; } //바닥에 떨어져있을 때 보여질 아이템의 형태
    [JsonProperty(Order = 4)]
    public string item_model_in_inventory { get;  set; } //인벤토리에서 보여질 아이템의 형태

    [JsonIgnore]
    public GameObject item_model_in_worlds;
    [JsonIgnore]
    public Sprite item_model_in_inv;
}


public class StackableItem : Original_Item
{
    [JsonProperty(Order = 5)]
    public int stack_max { get; set; } = 64;
    [JsonProperty(Order = 6)]
    public int stack_current { get; set; } = 1;

    //public StackableItem(string item_name, string item_model_in_world, string item_model_in_inventory, int stack_max, int stack_current)
    //{
    //    this.item_name = item_name;
    //    this.item_model_in_world = item_model_in_world;
    //    this.item_model_in_inventory = item_model_in_inventory;
    //    this.stack_max = stack_max;
    //    this.stack_current = stack_current;
    //}

}

public class ConsumableItem : StackableItem
{
    [JsonProperty(Order = 7)]
    public float hunger_amount { get; set; } = 0.0f;
    [JsonProperty(Order = 8)]
    public float thirst_amount { get; set; } = 0.0f;
    [JsonProperty(Order = 9)]
    public float fatigue_amount { get; set; } = 0.0f;
    [JsonProperty(Order = 10)]
    public float freshment_max { get; set; } = 100.0f;
    [JsonProperty(Order = 11)]
    public float freshment_current { get; set; } = 100.0f;

    

}

public class PlaceableItem : StackableItem
{
    [JsonProperty(Order = 7)]
    public Equipment_Type require_tool_type { get; set; } = Equipment_Type.NONE;
    [JsonProperty(Order = 8)]
    public int require_tool_tier { get; set; } = 1;
    [JsonProperty(Order = 9)]
    public float durability_max { get; set; } = 100.0f;
    [JsonProperty(Order = 10)]
    public float durability_current { get; set; } = 100.0f;
    [JsonProperty(Order = 11)]
    public string item_model_in_place { get; set; } = "";

}

public class EquipmentItem : Original_Item
{
    [JsonProperty(Order = 5)]
    public string equipment_type { get; set; } = "";
    [JsonProperty(Order = 6)]
    public float weight { get; set; } = 1.0f;
    [JsonProperty(Order = 7)]
    public float durability_max { get; set; } = 100.0f;
    [JsonProperty(Order = 8)]
    public float durability_current { get; set; } = 100.0f;
    [JsonProperty(Order = 9)]
    public string item_model_in_equip { get; set; } = "";

    [JsonIgnore]
    public Equipment_Armor_Type? Armor_Type { get; set; } = null;
    [JsonIgnore]
    public Equipment_Weapon_Type? Weapon_Type { get; set; } = null;

    [JsonProperty(Order = 12)]
    public float melee_damage { get; set; } = 0.0f;
    [JsonProperty(Order = 13)]
    public float melee_speed { get; set; } = 1.0f;
    [JsonProperty(Order = 14)]
    public float guard_rate { get; set; } = 0.0f;
    [JsonProperty(Order = 15)]
    public int tool_tier { get; set; } = 1;

    [JsonProperty(Order = 16)]
    public float armor_defense { get; set; } = 0.0f;

    [JsonProperty(Order = 17)]
    public float draw_power { get; set; } = 0.0f;
    [JsonProperty(Order = 18)]
    public float draw_speed { get; set; } = 0.0f;
    [JsonProperty(Order = 19)]
    public float aim_accuracy { get; set; } = 0.0f;
}

public class ItemData
{
    public List<StackableItem> stackableItems { get; set; }
    public List<ConsumableItem> consumableItems { get; set; }
    public List<PlaceableItem> placeableItems { get; set; }
    public List<EquipmentItem> equipmentItems { get; set; }
}