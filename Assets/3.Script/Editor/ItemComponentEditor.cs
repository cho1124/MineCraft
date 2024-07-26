using UnityEditor;
using UnityEngine;


/*
[CustomEditor(typeof(ItemComponent))]
public class ItemComponentEditor : Editor
{
    SerializedProperty itemID;
    SerializedProperty itemName;
    SerializedProperty itemIcon;
    SerializedProperty stackMax;
    SerializedProperty stackCurrent;

    SerializedProperty hungerAmount;
    SerializedProperty thirstAmount;
    SerializedProperty fatigueAmount;
    SerializedProperty freshmentMax;
    SerializedProperty freshmentCurrent;

    SerializedProperty requireToolType;
    SerializedProperty requireToolTier;
    SerializedProperty durabilityMax;
    SerializedProperty durabilityCurrent;
    SerializedProperty itemModelInPlace;
    

    SerializedProperty equipmentType;
    SerializedProperty weight;
    SerializedProperty itemDurabilityMax;
    SerializedProperty itemDurabilityCurrent;
    SerializedProperty itemModelInEquip;
    SerializedProperty armorType;
    SerializedProperty weaponType;
    SerializedProperty meleeDamage;
    SerializedProperty meleeSpeed;
    SerializedProperty guardRate;
    SerializedProperty toolTier;
    SerializedProperty armorDefense;

    void OnEnable()
    {
        itemID = serializedObject.FindProperty("itemID");
        itemName = serializedObject.FindProperty("itemName");
        itemIcon = serializedObject.FindProperty("itemIcon");
        stackMax = serializedObject.FindProperty("stackMax");
        stackCurrent = serializedObject.FindProperty("stackCurrent");

        hungerAmount = serializedObject.FindProperty("hungerAmount");
        thirstAmount = serializedObject.FindProperty("thirstAmount");
        fatigueAmount = serializedObject.FindProperty("fatigueAmount");
        freshmentMax = serializedObject.FindProperty("freshmentMax");
        freshmentCurrent = serializedObject.FindProperty("freshmentCurrent");

        requireToolType = serializedObject.FindProperty("requireToolType");
        requireToolTier = serializedObject.FindProperty("requireToolTier");
        durabilityMax = serializedObject.FindProperty("durabilityMax");
        durabilityCurrent = serializedObject.FindProperty("durabilityCurrent");
        itemModelInPlace = serializedObject.FindProperty("itemModelInPlace");
        

        equipmentType = serializedObject.FindProperty("equipmentType");
        weight = serializedObject.FindProperty("weight");
        itemDurabilityMax = serializedObject.FindProperty("durabilityMax");
        itemDurabilityCurrent = serializedObject.FindProperty("durabilityCurrent");
        itemModelInEquip = serializedObject.FindProperty("itemModelInEquip");
        armorType = serializedObject.FindProperty("armorType");
        weaponType = serializedObject.FindProperty("weaponType");
        meleeDamage = serializedObject.FindProperty("meleeDamage");
        meleeSpeed = serializedObject.FindProperty("meleeSpeed");
        guardRate = serializedObject.FindProperty("guardRate");
        toolTier = serializedObject.FindProperty("toolTier");
        armorDefense = serializedObject.FindProperty("armorDefense");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(itemID);
        EditorGUILayout.PropertyField(itemName);
        EditorGUILayout.PropertyField(itemIcon);
        EditorGUILayout.PropertyField(stackMax);
        EditorGUILayout.PropertyField(stackCurrent);

        ItemComponent itemComponent = (ItemComponent)target;

        if (IsConsumableItem(itemComponent))
        {
            DrawConsumableItemFields();
        }
        else if (IsPlaceableItem(itemComponent))
        {
            DrawPlaceableItemFields();
        }
        else if (IsEquipmentItem(itemComponent))
        {
            DrawEquipmentItemFields();
        }

        serializedObject.ApplyModifiedProperties();
    }

    private bool IsConsumableItem(ItemComponent itemComponent)
    {
        return itemComponent.hungerAmount > 0 || itemComponent.thirstAmount > 0 || itemComponent.fatigueAmount > 0;
    }

    private bool IsPlaceableItem(ItemComponent itemComponent)
    {
        return itemComponent.requireToolType != 0 || itemComponent.durabilityMax > 0;
    }

    private bool IsEquipmentItem(ItemComponent itemComponent)
    {
        return !string.IsNullOrEmpty(itemComponent.equipmentType);
    }

    private void DrawConsumableItemFields()
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Consumable Item Properties", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(hungerAmount);
        EditorGUILayout.PropertyField(thirstAmount);
        EditorGUILayout.PropertyField(fatigueAmount);
        EditorGUILayout.PropertyField(freshmentMax);
        EditorGUILayout.PropertyField(freshmentCurrent);
    }

    private void DrawPlaceableItemFields()
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Placeable Item Properties", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(requireToolType);
        EditorGUILayout.PropertyField(requireToolTier);
        EditorGUILayout.PropertyField(durabilityMax);
        EditorGUILayout.PropertyField(durabilityCurrent);
        EditorGUILayout.PropertyField(itemModelInPlace);
        
    }

    private void DrawEquipmentItemFields()
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Equipment Item Properties", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(equipmentType);
        EditorGUILayout.PropertyField(weight);
        EditorGUILayout.PropertyField(itemDurabilityMax);
        EditorGUILayout.PropertyField(itemDurabilityCurrent);
        EditorGUILayout.PropertyField(itemModelInEquip);
        EditorGUILayout.PropertyField(armorType);
        EditorGUILayout.PropertyField(weaponType);
        EditorGUILayout.PropertyField(meleeDamage);
        EditorGUILayout.PropertyField(meleeSpeed);
        EditorGUILayout.PropertyField(guardRate);
        EditorGUILayout.PropertyField(toolTier);
        EditorGUILayout.PropertyField(armorDefense);
    }
}
*/