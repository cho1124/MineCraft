using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;
using UnityEngine.UI;

public class Item_Manager : MonoBehaviour
{

    public TextAsset text1;
    [Header("�׽�Ʈ �غ�����")]
    public int TestKey = 7;

    public static class Item_Dictionary
    {
        public static Dictionary<int, Original_Item> item_dictionary { get; private set; }
        // ��ųʸ� �ʱ�ȭ
        static Item_Dictionary()
        {
            item_dictionary = new Dictionary<int, Original_Item>();
            LoadResources();
        }

        private static void LoadResources()
        {
            // ��� �������� ���ҽ��� �̸� �ε�
            foreach (var item in item_dictionary.Values)
            {
                item.item_model_in_worlds = Resources.Load<GameObject>(item.item_model_in_world);
                item.item_model_in_inv = Resources.Load<Sprite>(item.item_model_in_inventory);
            }
        }



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
                        Debug.LogWarning($"Ÿ���� ���� �𸣰ھ��� {itemID}.");
                    }

                }
                else
                {
                    Debug.LogWarning($"Item ID {itemID} ������ ������");
                }
            }
            else
            {
                Debug.LogWarning($"Item ID {itemID} ������");
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

        TextAsset jsonFile = Resources.Load<TextAsset>("Test"); // �׽�Ʈ��
        if (jsonFile == null)
        {
            Debug.LogError("Could not find JSON file in Resources folder.");
            return;
        }

        // JSON ������ �ؽ�Ʈ ������ ���ڿ��� ��ȯ
        string jsonString = jsonFile.text;

        // JSON ���ڿ��� ItemData ��ü�� �Ľ�
        ItemData itemData = JsonConvert.DeserializeObject<ItemData>(jsonString);

        
        Debug.Log("consume : " +itemData.consumableItems[0].item_name);
        Debug.Log("place : " + itemData.placeableItems.Count);
        Debug.Log("equipmentitemcount : " + itemData.equipmentItems.Count);

        // �Ľ̵� ������ ��� ����

        Dic_Add(itemData.stackableItems);

        Dic_Add(itemData.consumableItems);

        Dic_Add(itemData.placeableItems);

        Dic_Add(itemData.equipmentItems);

        Debug.Log(Item_Dictionary.item_dictionary[7].item_ID);

        Item_Dictionary.SpawnItem(TestKey, Vector3.zero);



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

            // equipment_type�� ���� ������ ���� ����
            EquipmentItem equipmentItem = item;
            if (equipmentItem != null)
            {
                // TryParse�� ����Ͽ� �����ϰ� ���������� ��ȯ
                equipmentItem.Weapon_Type = ParseWeaponType(equipmentItem.equipment_type);
                equipmentItem.Armor_Type = ParseArmorType(equipmentItem.equipment_type);

                if (equipmentItem.Weapon_Type.HasValue)
                {
                    // ���� �Ӽ� ����
                    Debug.Log($"Weapon Item: {equipmentItem.item_name}, Damage: {equipmentItem.melee_damage}, Speed: {equipmentItem.melee_speed}, Guard Rate: {equipmentItem.guard_rate}, Tool Tier: {equipmentItem.tool_tier}");
                }
                else if (equipmentItem.Armor_Type.HasValue)
                {
                    // �� �Ӽ� ����
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

    //enum parse ������ ��
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

public class Original_Item
{
    public int item_ID { get;  set; } //������ ���̵�
    public string item_name { get;  set; } //������ �̸�
    public string item_model_in_world { get;  set; } //�ٴڿ� ���������� �� ������ �������� ����
    public string item_model_in_inventory { get;  set; } //�κ��丮���� ������ �������� ����

    public GameObject item_model_in_worlds;
    public Sprite item_model_in_inv;
}


public class StackableItem : Original_Item
{

    public int stack_max { get; set; } = 64;
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
    public float hunger_amount { get; set; } = 0.0f;
    public float thirst_amount { get; set; } = 0.0f;
    public float fatigue_amount { get; set; } = 0.0f;
    public float freshment_max { get; set; } = 100.0f;
    public float freshment_current { get; set; } = 100.0f;
}

public class PlaceableItem : StackableItem
{
    public Equipment_Type require_tool_type { get; set; } = Equipment_Type.NONE;
    public int require_tool_tier { get; set; } = 1;
    public float durability_max { get; set; } = 100.0f;
    public float durability_current { get; set; } = 100.0f;
    public string item_model_in_place { get; set; } = "";

}

public class EquipmentItem : Original_Item
{
    public string equipment_type { get; set; } = "";
    public float weight { get; set; } = 1.0f;
    public float durability_max { get; set; } = 100.0f;
    public float durability_current { get; set; } = 100.0f;
    public string item_model_in_equip { get; set; } = "";

    public Equipment_Armor_Type? Armor_Type { get; set; } = null;
    public Equipment_Weapon_Type? Weapon_Type { get; set; } = null;

    public float melee_damage { get; set; } = 0.0f;
    public float melee_speed { get; set; } = 1.0f;
    public float guard_rate { get; set; } = 0.0f;
    public int tool_tier { get; set; } = 1;

    public float armor_defense { get; set; } = 0.0f;
}

public class ItemData
{
    public List<StackableItem> stackableItems { get; set; }
    public List<ConsumableItem> consumableItems { get; set; }
    public List<PlaceableItem> placeableItems { get; set; }
    public List<EquipmentItem> equipmentItems { get; set; }
}