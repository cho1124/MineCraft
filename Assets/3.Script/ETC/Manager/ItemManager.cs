using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;
using UnityEngine.UI;

public class ItemManager : MonoBehaviour
{

    public TextAsset text1;

    public static class Item_Dictionary
    {
        // 딕셔너리 초기화
        static Item_Dictionary()
        {
            item_dictionary = new Dictionary<int, Original_Item>();
        }

        public static Dictionary<int, Original_Item> item_dictionary { get; private set; }

        public static void SpawnItem(int itemID, Vector3 position)
        {
            if (item_dictionary.ContainsKey(itemID))
            {
                GameObject itemPrefab = item_dictionary[itemID].item_model_in_worlds;
                if (itemPrefab != null)
                {
                    GameObject spawnedItem = Instantiate(itemPrefab, position, Quaternion.identity);
                    ItemComponent itemComponent = spawnedItem.AddComponent<ItemComponent>();

                    Original_Item itemData = item_dictionary[itemID];
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


    void Start()
    {

        TextAsset jsonFile = Resources.Load<TextAsset>("Test"); // 테스트용
        if (jsonFile == null)
        {
            Debug.LogError("Could not find JSON file in Resources folder.");
            return;
        }

        // JSON 파일의 텍스트 내용을 문자열로 변환
        string jsonString = jsonFile.text;

        // JSON 문자열을 ItemData 객체로 파싱
        ItemData itemData = JsonConvert.DeserializeObject<ItemData>(jsonString);

        
        Debug.Log("consume : " +itemData.consumableItems[0].item_name);
        Debug.Log("place : " + itemData.placeableItems.Count);
        Debug.Log("equipmentitemcount : " + itemData.equipmentItems.Count);

        // 파싱된 데이터 사용 예제

        Dic_Add(itemData.stackableItems);

        Dic_Add(itemData.consumableItems);

        Dic_Add(itemData.placeableItems);

        Dic_Add(itemData.equipmentItems);

        Debug.Log(Item_Dictionary.item_dictionary[7].item_ID);

        Item_Dictionary.SpawnItem(7, Vector3.zero);



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
    }

    public Sprite GetItemImage(string itemName_Inv)
    {
        Sprite image = Resources.Load<Sprite>(itemName_Inv);
        return image;
    }


}
public class Voxel
{
    public int x { get; set; }
    public int y { get; set; }
    public int z { get; set; }
    public string type { get; set; }
}


public class Original_Item
{
    public int item_ID { get;  set; } //아이템 아이디
    public string item_name { get;  set; } //아이템 이름
    public string item_model_in_world { get;  set; } //바닥에 떨어져있을 때 보여질 아이템의 형태
    public string item_model_in_inventory { get;  set; } //인벤토리에서 보여질 아이템의 형태

    public GameObject item_model_in_worlds;
    public Sprite item_model_in_inv;
}


public class StackableItem : Original_Item
{
    
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

public class EquipmentItem : Original_Item
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