    using UnityEngine;
    using UnityEditor;
    using System.IO;
    using System.Collections.Generic;
    using Newtonsoft.Json;

public class ItemJsonEditorWindow : EditorWindow
{
    private string jsonFileName = "ItemData.json";
    private string itemName = "";
    private ItemType itemType = ItemType.Stackable;
    private StackableItem stackableItem = new StackableItem();
    private ConsumableItem consumableItem = new ConsumableItem();
    private PlaceableItem placeableItem = new PlaceableItem();
    private EquipmentItem equipmentItem = new EquipmentItem();
    private ItemData itemData = new ItemData();
    
    private Equipment_Type equipmentType = Equipment_Type.NONE;

    [MenuItem("Window/JSON Item Editor")]
    public static void ShowWindow()
    {
        GetWindow<ItemJsonEditorWindow>("JSON Item Editor");
    }

    private void OnGUI()
    {
        GUILayout.Label("Create or Update Item Data", EditorStyles.boldLabel);

        jsonFileName = EditorGUILayout.TextField("File Name", jsonFileName);
        //itemName = EditorGUILayout.TextField("Item Name", itemName);
        itemType = (ItemType)EditorGUILayout.EnumPopup("Item Type", itemType);

        switch (itemType)
        {
            case ItemType.Stackable:
                DrawStackableItemFields(stackableItem);
                break;
            case ItemType.Consumable:
                DrawConsumableItemFields();
                break;
            case ItemType.Placeable:
                DrawPlaceableItemFields();
                break;
            case ItemType.Equipment:
                DrawEquipmentItemFields();
                break;
        }

        if (GUILayout.Button("Save JSON"))
        {
            SaveJsonFile();
        }
    }

    private void DrawBasicItemFields(Original_Item item)
    {
        item.item_ID = EditorGUILayout.IntField("Item ID", item.item_ID);
        item.item_name = EditorGUILayout.TextField("Item Name", item.item_name);
        if (string.IsNullOrEmpty(item.item_model_in_world))
        {
            item.item_model_in_world = "Prefabs/";
        }
        if (string.IsNullOrEmpty(item.item_model_in_inventory))
        {
            item.item_model_in_inventory = "Icons/";
        }
        item.item_model_in_world = EditorGUILayout.TextField("Model In World", item.item_model_in_world);
        item.item_model_in_inventory = EditorGUILayout.TextField("Model In Inventory", item.item_model_in_inventory);

    }

    private void DrawStackableItemFields(StackableItem item)
    {
        DrawBasicItemFields(item);
        item.stack_max = EditorGUILayout.IntField("Stack Max", stackableItem.stack_max);
        item.stack_current = EditorGUILayout.IntField("Stack Current", stackableItem.stack_current);
    }

    private void DrawConsumableItemFields()
    {
        DrawStackableItemFields(consumableItem);
        consumableItem.hunger_amount = EditorGUILayout.FloatField("Hunger Amount", consumableItem.hunger_amount);
        consumableItem.thirst_amount = EditorGUILayout.FloatField("Thirst Amount", consumableItem.thirst_amount);
        consumableItem.fatigue_amount = EditorGUILayout.FloatField("Fatigue Amount", consumableItem.fatigue_amount);
        consumableItem.freshment_max = EditorGUILayout.FloatField("Freshment Max", consumableItem.freshment_max);
        consumableItem.freshment_current = EditorGUILayout.FloatField("Freshment Current", consumableItem.freshment_current);

    }

    private void DrawPlaceableItemFields()
    {
        DrawStackableItemFields(placeableItem);
        placeableItem.require_tool_type = (Equipment_Type)EditorGUILayout.EnumPopup("Require Tool Type", placeableItem.require_tool_type);
        
        placeableItem.require_tool_tier = EditorGUILayout.IntField("Require Tool Tier", placeableItem.require_tool_tier);
        placeableItem.durability_max = EditorGUILayout.FloatField("Durability Max", placeableItem.durability_max);
        placeableItem.durability_current = EditorGUILayout.FloatField("Durability Current", placeableItem.durability_current);
        placeableItem.item_model_in_place = EditorGUILayout.TextField("Item Model In Place", placeableItem.item_model_in_place);

    }

    private void DrawEquipmentItemFields()
    {
        DrawBasicItemFields(equipmentItem);

        equipmentType = (Equipment_Type)EditorGUILayout.EnumPopup("Equipment Type", equipmentType);
        equipmentItem.equipment_type = equipmentType.ToString();

        equipmentItem.weight = EditorGUILayout.FloatField("Weight", equipmentItem.weight);
        equipmentItem.durability_max = EditorGUILayout.FloatField("Durability Max", equipmentItem.durability_max);
        equipmentItem.durability_current = EditorGUILayout.FloatField("Durability Current", equipmentItem.durability_current);
        equipmentItem.item_model_in_equip = EditorGUILayout.TextField("Item Model In Equip", equipmentItem.item_model_in_equip);

        if (equipmentType == Equipment_Type.SHIELD || equipmentType == Equipment_Type.HELMET || equipmentType == Equipment_Type.CHESTPLATE || equipmentType == Equipment_Type.LEGGINGS || equipmentType == Equipment_Type.BOOTS)
        {
            equipmentItem.armor_defense = EditorGUILayout.FloatField("Armor Defense", equipmentItem.armor_defense);
        }
        else
        {
            equipmentItem.melee_damage = EditorGUILayout.FloatField("Melee Damage", equipmentItem.melee_damage);
            equipmentItem.melee_speed = EditorGUILayout.FloatField("Melee Speed", equipmentItem.melee_speed);
            equipmentItem.guard_rate = EditorGUILayout.FloatField("Guard Rate", equipmentItem.guard_rate);
            equipmentItem.tool_tier = EditorGUILayout.IntField("Tool Tier", equipmentItem.tool_tier);
        }
    }


    private void SaveJsonFile()
    {
        string path = Application.dataPath + "/Resources/" + jsonFileName;

        // 기존 파일이 존재하는 경우 데이터 읽기
        if (File.Exists(path))
        {
            string existingJson = File.ReadAllText(path);
            itemData = JsonConvert.DeserializeObject<ItemData>(existingJson) ?? new ItemData();
        }
        else
        {
            itemData = new ItemData();
        }

        // 각 리스트가 null인지 확인하고 초기화
        if (itemData.stackableItems == null) itemData.stackableItems = new List<StackableItem>();
        if (itemData.consumableItems == null) itemData.consumableItems = new List<ConsumableItem>();
        if (itemData.placeableItems == null) itemData.placeableItems = new List<PlaceableItem>();
        if (itemData.equipmentItems == null) itemData.equipmentItems = new List<EquipmentItem>();

        // 새로운 아이템 데이터 추가 또는 업데이트
        switch (itemType)
        {
            case ItemType.Stackable:
                AddOrUpdateItem(stackableItem, itemData.stackableItems);
                break;
            case ItemType.Consumable:
                AddOrUpdateItem(consumableItem, itemData.consumableItems);
                break;
            case ItemType.Placeable:
                AddOrUpdateItem(placeableItem, itemData.placeableItems);
                break;
            case ItemType.Equipment:
                AddOrUpdateItem(equipmentItem, itemData.equipmentItems);
                break;
        }

        // JSON 파일로 저장
        string jsonString = JsonConvert.SerializeObject(itemData, Formatting.Indented);
        File.WriteAllText(path, jsonString);
        AssetDatabase.Refresh();
        Debug.Log("JSON file saved at " + path);
    }

    private void AddOrUpdateItem<T>(T item, List<T> itemList) where T : Original_Item
    {
        if (itemList == null)
        {
            itemList = new List<T>();
        }

        int index = itemList.FindIndex(i => i.item_name == item.item_name);
        if (index >= 0)
        {
            itemList[index] = item;
        }
        else
        {
            itemList.Add(item);
        }
    }
}

public enum ItemType
    {
        Stackable,
        Consumable,
        Placeable,
        Equipment
    }

