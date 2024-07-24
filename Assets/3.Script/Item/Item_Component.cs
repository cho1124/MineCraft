using UnityEngine;
using System.Collections.Generic;

public class ItemComponent : MonoBehaviour
{
    public int itemID;
    public string itemName;
    public Sprite itemIcon;
    public int stackMax;
    public int stackCurrent;

    // ConsumableItem �ʵ�
    public float hungerAmount;
    public float thirstAmount;
    public float fatigueAmount;
    public float freshmentMax;
    public float freshmentCurrent;

    // PlaceableItem �ʵ�
    public Equipment_Type requireToolType;
    public int requireToolTier;
    
    public string itemModelInPlace;
    public List<Voxel> voxels;

    // EquipmentItem �ʵ�
    public string equipmentType;
    public float weight;
    public float durabilityMax;
    public float durabilityCurrent;
    public string itemModelInEquip;
    public Equipment_Armor_Type? armorType;
    public Equipment_Weapon_Type? weaponType;
    public float meleeDamage;
    public float meleeSpeed;
    public float guardRate;
    public int toolTier;
    public float armorDefense;

    // StackableItem �ʱ�ȭ �޼���

    public void Initialize(Original_Item item)
    {
        itemID = item.item_ID;
        itemName = item.item_name;
        itemIcon = item.item_model_in_inv;
    }

    public void Initialize(StackableItem item)
    {
        
        Initialize((Original_Item)item);

        stackMax = item.stack_max;
        stackCurrent = item.stack_current;
    }

    // ConsumableItem �ʱ�ȭ �޼���
    public void Initialize(ConsumableItem item)
    {
        Initialize((StackableItem)item);
        hungerAmount = item.hunger_amount;
        thirstAmount = item.thirst_amount;
        fatigueAmount = item.fatigue_amount;
        freshmentMax = item.freshment_max;
        freshmentCurrent = item.freshment_current;
    }

    // PlaceableItem �ʱ�ȭ �޼���
    public void Initialize(PlaceableItem item)
    {
        Initialize((StackableItem)item);
        requireToolType = item.require_tool_type;
        requireToolTier = item.require_tool_tier;
        durabilityMax = item.durability_max;
        durabilityCurrent = item.durability_current;
        itemModelInPlace = item.item_model_in_place;
        voxels = item.voxels;
    }

    // EquipmentItem �ʱ�ȭ �޼���
    public void Initialize(EquipmentItem item)
    {
        Initialize((Original_Item)item);
        equipmentType = item.equipment_type;
        weight = item.weight;
        durabilityMax = item.durability_max;
        durabilityCurrent = item.durability_current;
        itemModelInEquip = item.item_model_in_equip;
        armorType = item.Armor_Type;
        weaponType = item.Weapon_Type;
        meleeDamage = item.melee_damage;
        meleeSpeed = item.melee_speed;
        guardRate = item.guard_rate;
        toolTier = item.tool_tier;
        armorDefense = item.armor_defense;
    }
}
