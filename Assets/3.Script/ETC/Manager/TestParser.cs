using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;

public class TestParser : MonoBehaviour
{

    public TextAsset text1;


    public static class Item_Dictionary
    {
        // 딕셔너리 초기화
        static Item_Dictionary()
        {
            item_dictionary = new Dictionary<int, StackableItem>();
        }

        public static Dictionary<int, StackableItem> item_dictionary { get; private set; }

        public static void Add(int item_ID, StackableItem item_data)
        {
            if (!item_dictionary.ContainsKey(item_ID))
            {
                item_dictionary.Add(item_ID, item_data);
            }
            else
            {
                Debug.LogWarning($"Item ID {item_ID} already exists in the dictionary.");
            }
        }
    }



    void Start()
    {
        string jsonString = @"
        {
            ""stackableItems"": [
                {
                    ""item_ID"": 1,
                    ""item_name"": ""Iron Ore"",
                    ""item_model_in_world"": ""Prefabs/IronOre"",
                    ""item_model_in_inventory"": ""Icons/IronOre"",
                    ""stack_max"": 64,
                    ""stack_current"": 10
                },
                {
                    ""item_ID"": 2,
                    ""item_name"": ""Gold Ore"",
                    ""item_model_in_world"": ""Prefabs/GoldOre"",
                    ""item_model_in_inventory"": ""Icons/GoldOre"",
                    ""stack_max"": 64,
                    ""stack_current"": 5
                }
            ],
            ""consumableItems"": [
                {
                    ""item_ID"": 3,
                    ""item_name"": ""Bread"",
                    ""item_model_in_world"": ""Prefabs/Bread"",
                    ""item_model_in_inventory"": ""Icons/Bread"",
                    ""stack_max"": 10,
                    ""stack_current"": 5,
                    ""hunger_amount"": 20.0,
                    ""thirst_amount"": 0.0,
                    ""fatigue_amount"": 5.0,
                    ""freshment_max"": 100.0,
                    ""freshment_current"": 50.0
                },
                {
                    ""item_ID"": 4,
                    ""item_name"": ""Apple"",
                    ""item_model_in_world"": ""Prefabs/Apple"",
                    ""item_model_in_inventory"": ""Icons/Apple"",
                    ""stack_max"": 20,
                    ""stack_current"": 10,
                    ""hunger_amount"": 10.0,
                    ""thirst_amount"": 5.0,
                    ""fatigue_amount"": 2.0,
                    ""freshment_max"": 80.0,
                    ""freshment_current"": 40.0
                }
            ],
            ""placeableItems"": [
                {
                    ""item_ID"": 5,
                    ""item_name"": ""Stone Block"",
                    ""item_model_in_world"": ""Prefabs/StoneBlock"",
                    ""item_model_in_inventory"": ""Icons/StoneBlock"",
                    ""stack_max"": 64,
                    ""stack_current"": 20,
                    ""require_tool_type"": ""PICKAXE"",
                    ""require_tool_tier"": 1,
                    ""durability_max"": 100.0,
                    ""durability_current"": 100.0,
                    ""item_model_in_place"": ""Models/StoneBlock"",
                    ""voxels"": [
                        {
                            ""x"": 0,
                            ""y"": 0,
                            ""z"": 0,
                            ""type"": ""stone""
                        },
                        {
                            ""x"": 1,
                            ""y"": 0,
                            ""z"": 0,
                            ""type"": ""stone""
                        }
                    ]
                },
                {
                    ""item_ID"": 6,
                    ""item_name"": ""Wooden Block"",
                    ""item_model_in_world"": ""Prefabs/WoodenBlock"",
                    ""item_model_in_inventory"": ""Icons/WoodenBlock"",
                    ""stack_max"": 64,
                    ""stack_current"": 15,
                    ""require_tool_type"": ""ONE_HANDED_AXE"",
                    ""require_tool_tier"": 1,
                    ""durability_max"": 50.0,
                    ""durability_current"": 50.0,
                    ""item_model_in_place"": ""Models/WoodenBlock"",
                    ""voxels"": [
                        {
                            ""x"": 0,
                            ""y"": 0,
                            ""z"": 0,
                            ""type"": ""wood""
                        },
                        {
                            ""x"": 1,
                            ""y"": 0,
                            ""z"": 0,
                            ""type"": ""wood""
                        }
                    ]
                }
            ],
            ""equipmentItems"": [
                {
                    ""item_ID"": 7,
                    ""item_name"": ""Iron Sword"",
                    ""item_model_in_world"": ""Prefabs/IronSword"",
                    ""item_model_in_inventory"": ""Icons/IronSword"",
                    ""equipment_type"": ""ONE_HANDED_SWORD"",
                    ""weight"": 5.0,
                    ""durability_max"": 100.0,
                    ""durability_current"": 100.0,
                    ""item_model_in_equip"": ""Models/IronSword"",
                    ""melee_damage"": 10.0,
                    ""melee_speed"": 1.5,
                    ""guard_rate"": 0.5,
                    ""tool_tier"": 2
                },
                {
                    ""item_ID"": 8,
                    ""item_name"": ""Steel Shield"",
                    ""item_model_in_world"": ""Prefabs/SteelShield"",
                    ""item_model_in_inventory"": ""Icons/SteelShield"",
                    ""equipment_type"": ""SHIELD"",
                    ""weight"": 8.0,
                    ""durability_max"": 120.0,
                    ""durability_current"": 120.0,
                    ""item_model_in_equip"": ""Models/SteelShield"",
                    ""armor_defense"": 15.0
                }
            ]
        }";



        // JSON 문자열을 ItemData 객체로 파싱
        ItemData itemData = JsonConvert.DeserializeObject<ItemData>(jsonString);

        // 파싱된 데이터 사용 예제
        foreach (var item in itemData.stackableItems)
        {
            Debug.Log("SADAUSHVUC : " +  item.item_ID);

            Item_Dictionary.Add(item.item_ID, item);
            


        }

        foreach (var item in itemData.consumableItems)
        {
            Debug.Log("Consumable Item Name: " + item.item_name);
        }

        foreach (var item in itemData.placeableItems)
        {
            Debug.Log("Placeable Item Name: " + item.item_name);
        }

        foreach (var item in itemData.equipmentItems)
        {
            Debug.Log("Equipment Item ID: " + item.item_ID);

            // equipment_type에 따라 열거형 값을 설정
            EquipmentItem equipmentItem = item as EquipmentItem;
            if (equipmentItem != null)
            {

                //열거형 타입 두 부분으로 나눠서 처리함
                //먼저 무기형 열거형과 일치하면 방어구 열거형은 null
                //반대의 경우도 존재

                // 문자열 비교를 통해 열거형 값을 설정
                if (equipmentItem.equipment_type == nameof(Equipment_Weapon_Type.ONE_HANDED_SWORD) ||
                    equipmentItem.equipment_type == nameof(Equipment_Weapon_Type.ONE_HANDED_AXE) ||
                    equipmentItem.equipment_type == nameof(Equipment_Weapon_Type.ONE_HANDED_HAMMER) ||
                    equipmentItem.equipment_type == nameof(Equipment_Weapon_Type.TWO_HANDED_SWORD) ||
                    equipmentItem.equipment_type == nameof(Equipment_Weapon_Type.TWO_HANDED_AXE) ||
                    equipmentItem.equipment_type == nameof(Equipment_Weapon_Type.TWO_HANDED_HAMMER) ||
                    equipmentItem.equipment_type == nameof(Equipment_Weapon_Type.BOW) ||
                    equipmentItem.equipment_type == nameof(Equipment_Weapon_Type.PICKAXE) ||
                    equipmentItem.equipment_type == nameof(Equipment_Weapon_Type.SHOVEL) ||
                    equipmentItem.equipment_type == nameof(Equipment_Weapon_Type.HOE))
                {
                    equipmentItem.Weapon_Type = (Equipment_Weapon_Type)Enum.Parse(typeof(Equipment_Weapon_Type), equipmentItem.equipment_type);
                    equipmentItem.Armor_Type = null; // 무기일 경우 방어구 타입은 null로 설정

                    // 무기 속성 설정
                    Debug.Log($"Weapon Item: {equipmentItem.item_name}, Damage: {equipmentItem.melee_damage}, Speed: {equipmentItem.melee_speed}, Guard Rate: {equipmentItem.guard_rate}, Tool Tier: {equipmentItem.tool_tier}");
                }
                else if (equipmentItem.equipment_type == nameof(Equipment_Armor_Type.HELMET) ||
                         equipmentItem.equipment_type == nameof(Equipment_Armor_Type.CHESTPLATE) ||
                         equipmentItem.equipment_type == nameof(Equipment_Armor_Type.LEGGINGS) ||
                         equipmentItem.equipment_type == nameof(Equipment_Armor_Type.BOOTS) ||
                         equipmentItem.equipment_type == nameof(Equipment_Armor_Type.SHIELD))
                {
                    equipmentItem.Armor_Type = (Equipment_Armor_Type)Enum.Parse(typeof(Equipment_Armor_Type), equipmentItem.equipment_type);
                    equipmentItem.Weapon_Type = null; // 방어구일 경우 무기 타입은 null로 설정

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
        Debug.Log(Item_Dictionary.item_dictionary[1].item_name);


    }
}
public class Voxel
{
    public int x { get; set; }
    public int y { get; set; }
    public int z { get; set; }
    public string type { get; set; }
}

public class StackableItem
{
    public int item_ID { get; set; }
    public string item_name { get; set; }
    public string item_model_in_world { get; set; }
    public string item_model_in_inventory { get; set; }
    public int stack_max { get; set; }
    public int stack_current { get; set; }

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
    public float hunger_amount { get; set; }
    public float thirst_amount { get; set; }
    public float fatigue_amount { get; set; }
    public float freshment_max { get; set; }
    public float freshment_current { get; set; }
}

public class PlaceableItem : StackableItem
{
    public Equipment_Type require_tool_type { get; set; }
    public int require_tool_tier { get; set; }
    public float durability_max { get; set; }
    public float durability_current { get; set; }
    public string item_model_in_place { get; set; }
    public List<Voxel> voxels { get; set; }
}

public class EquipmentItem : StackableItem
{
    public string equipment_type { get; set; }
    public float weight { get; set; }
    public float durability_max { get; set; }
    public float durability_current { get; set; }
    public string item_model_in_equip { get; set; }

    public Equipment_Armor_Type? Armor_Type { get; set; }
    public Equipment_Weapon_Type? Weapon_Type { get; set; }


    public float melee_damage { get; set; }
    public float melee_speed { get; set; }
    public float guard_rate { get; set; }
    public int tool_tier { get; set; }

    public float armor_defense { get; set; }
}

public class ItemData
{
    public List<StackableItem> stackableItems { get; set; }
    public List<ConsumableItem> consumableItems { get; set; }
    public List<PlaceableItem> placeableItems { get; set; }
    public List<EquipmentItem> equipmentItems { get; set; }
}