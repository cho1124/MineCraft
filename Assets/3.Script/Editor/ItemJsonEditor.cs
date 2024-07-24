using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;

public class ItemJsonEditorWindow : EditorWindow
{
    private string jsonFileName = "ItemData.json";

    private ItemData itemData = new ItemData()
    {
        stackableItems = new List<StackableItem>(),
        consumableItems = new List<ConsumableItem>(),
        placeableItems = new List<PlaceableItem>(),
        equipmentItems = new List<EquipmentItem>()
    };

    [MenuItem("Window/JSON Item Editor")]
    public static void ShowWindow()
    {
        GetWindow<ItemJsonEditorWindow>("JSON Item Editor");
    }

    private void OnGUI()
    {
        GUILayout.Label("Create or Update Item Data", EditorStyles.boldLabel);

        jsonFileName = EditorGUILayout.TextField("File Name", jsonFileName);

        if (GUILayout.Button("Add Stackable Item"))
        {
            itemData.stackableItems.Add(new StackableItem());
        }

        if (GUILayout.Button("Add Consumable Item"))
        {
            itemData.consumableItems.Add(new ConsumableItem());
        }

        if (GUILayout.Button("Add Placeable Item"))
        {
            itemData.placeableItems.Add(new PlaceableItem());
        }

        if (GUILayout.Button("Add Equipment Item"))
        {
            itemData.equipmentItems.Add(new EquipmentItem());
        }

        if (GUILayout.Button("Save JSON"))
        {
            SaveJsonFile();
        }

        // Display current items
        GUILayout.Space(20);
        GUILayout.Label("Stackable Items", EditorStyles.boldLabel);
        foreach (var item in itemData.stackableItems)
        {
            DisplayItem(item);
        }

        GUILayout.Label("Consumable Items", EditorStyles.boldLabel);
        foreach (var item in itemData.consumableItems)
        {
            DisplayItem(item);
        }

        GUILayout.Label("Placeable Items", EditorStyles.boldLabel);
        foreach (var item in itemData.placeableItems)
        {
            DisplayItem(item);
        }

        GUILayout.Label("Equipment Items", EditorStyles.boldLabel);
        foreach (var item in itemData.equipmentItems)
        {
            DisplayItem(item);
        }
    }

    private void DisplayItem(OriginalItem item)
    {
        item.itemID = EditorGUILayout.IntField("Item ID", item.itemID);
        item.itemName = EditorGUILayout.TextField("Item Name", item.itemName);
        item.itemModelInWorld = EditorGUILayout.TextField("Model In World", item.itemModelInWorld);
        item.itemModelInInventory = EditorGUILayout.TextField("Model In Inventory", item.itemModelInInventory);

        if (item is StackableItem stackableItem)
        {
            stackableItem.stackMax = EditorGUILayout.IntField("Stack Max", stackableItem.stackMax);
            stackableItem.stackCurrent = EditorGUILayout.IntField("Stack Current", stackableItem.stackCurrent);
        }

        if (item is ConsumableItem consumableItem)
        {
            consumableItem.hungerAmount = EditorGUILayout.FloatField("Hunger Amount", consumableItem.hungerAmount);
            consumableItem.thirstAmount = EditorGUILayout.FloatField("Thirst Amount", consumableItem.thirstAmount);
            consumableItem.fatigueAmount = EditorGUILayout.FloatField("Fatigue Amount", consumableItem.fatigueAmount);
            consumableItem.freshmentMax = EditorGUILayout.FloatField("Freshment Max", consumableItem.freshmentMax);
            consumableItem.freshmentCurrent = EditorGUILayout.FloatField("Freshment Current", consumableItem.freshmentCurrent);
        }

        if (item is PlaceableItem placeableItem)
        {
            placeableItem.requireToolType = (EquipmentType)EditorGUILayout.EnumPopup("Require Tool Type", placeableItem.requireToolType);
            placeableItem.requireToolTier = EditorGUILayout.IntField("Require Tool Tier", placeableItem.requireToolTier);
            placeableItem.durabilityMax = EditorGUILayout.FloatField("Durability Max", placeableItem.durabilityMax);
            placeableItem.durabilityCurrent = EditorGUILayout.FloatField("Durability Current", placeableItem.durabilityCurrent);
            placeableItem.itemModelInPlace = EditorGUILayout.TextField("Model In Place", placeableItem.itemModelInPlace);
        }

        if (item is EquipmentItem equipmentItem)
        {
            equipmentItem.equipmentType = EditorGUILayout.TextField("Equipment Type", equipmentItem.equipmentType);
            equipmentItem.weight = EditorGUILayout.FloatField("Weight", equipmentItem.weight);
            equipmentItem.durabilityMax = EditorGUILayout.FloatField("Durability Max", equipmentItem.durabilityMax);
            equipmentItem.durabilityCurrent = EditorGUILayout.FloatField("Durability Current", equipmentItem.durabilityCurrent);
            equipmentItem.itemModelInEquip = EditorGUILayout.TextField("Model In Equip", equipmentItem.itemModelInEquip);
            equipmentItem.meleeDamage = EditorGUILayout.FloatField("Melee Damage", equipmentItem.meleeDamage);
            equipmentItem.meleeSpeed = EditorGUILayout.FloatField("Melee Speed", equipmentItem.meleeSpeed);
            equipmentItem.guardRate = EditorGUILayout.FloatField("Guard Rate", equipmentItem.guardRate);
            equipmentItem.toolTier = EditorGUILayout.IntField("Tool Tier", equipmentItem.toolTier);
            equipmentItem.armorDefense = EditorGUILayout.FloatField("Armor Defense", equipmentItem.armorDefense);
        }

        GUILayout.Space(10);
    }

    private void SaveJsonFile()
    {
        string path = Application.dataPath + "/" + jsonFileName;
        string jsonString = JsonConvert.SerializeObject(itemData, Formatting.Indented);

        File.WriteAllText(path, jsonString);
        AssetDatabase.Refresh();
        Debug.Log("JSON file saved at " + path);
    }
}

[System.Serializable]
public class OriginalItem
{
    public int itemID;
    public string itemName;
    public string itemModelInWorld;
    public string itemModelInInventory;
}

[System.Serializable]
public class StackableItem : OriginalItem
{
    public int stackMax;
    public int stackCurrent;
}

[System.Serializable]
public class ConsumableItem : StackableItem
{
    public float hungerAmount;
    public float thirstAmount;
    public float fatigueAmount;
    public float freshmentMax;
    public float freshmentCurrent;
}

[System.Serializable]
public class PlaceableItem : StackableItem
{
    public EquipmentType requireToolType;
    public int requireToolTier;
    public float durabilityMax;
    public float durabilityCurrent;
    public string itemModelInPlace;
}

[System.Serializable]
public class EquipmentItem : OriginalItem
{
    public string equipmentType;
    public float weight;
    public float durabilityMax;
    public float durabilityCurrent;
    public string itemModelInEquip;
    public float meleeDamage;
    public float meleeSpeed;
    public float guardRate;
    public int toolTier;
    public float armorDefense;
}

[System.Serializable]
public class ItemData
{
    public List<StackableItem> stackableItems;
    public List<ConsumableItem> consumableItems;
    public List<PlaceableItem> placeableItems;
    public List<EquipmentItem> equipmentItems;
}

public enum EquipmentType
{
    None,
    Tool,
    Weapon,
    Armor
}
