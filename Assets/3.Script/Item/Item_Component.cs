using UnityEngine;
using System.Collections.Generic;

public class ItemComponent : MonoBehaviour
{
    public int itemID { get; private set; }
    public int ItemID
    {
        get => itemID;
    }

    public string itemName { get; private set; }
    public Sprite itemIcon { get; private set; }
    public int stackMax { get; private set; }
    public int stackCurrent { get; private set; }

    public int StackCurrent
    {
        get => stackCurrent;
        set
        {
            
            stackCurrent = Mathf.Clamp(value, 0, stackMax);
            
        }

    }


    public int SetType { get; private set; } //열거형 만들기 귀찮아서 대충 //// original 0, stackable 1, consumable 2, placeable 3
    // ConsumableItem 필드
    public float hungerAmount { get; private set; }
    public float thirstAmount { get; private set; }
    public float fatigueAmount { get; private set; }
    public float freshmentMax { get; private set; }
    public float freshmentCurrent { get; private set; }

    // PlaceableItem 필드
    public Equipment_Type requireToolType { get; private set; }
    public int requireToolTier { get; private set; }

    public string itemModelInPlace { get; private set; }


    // EquipmentItem 필드
    [SerializeField] public string equipmentType;


    public float weight { get; private set; }
    public float durabilityMax { get; private set; }
    public float durabilityCurrent { get; private set; }
    public string itemModelInEquip { get; private set; }
    public Equipment_Armor_Type? armorType { get; private set; }
    public Equipment_Weapon_Type? weaponType { get; private set; }
    public float meleeDamage { get; private set; }
    public float meleeSpeed { get; private set; }
    public float guardRate { get; private set; }
    public int toolTier { get; private set; }
    public float armorDefense { get; private set; }
    public float drawPower { get; private set; }
    public float drawSpeed { get; private set; }
    public float aimAccuracy { get; private set; }

    // StackableItem 초기화 메서드

    public void Initialize(Original_Item item)
    {
        
        itemID = item.item_ID;
        itemName = item.item_name;
        itemIcon = item.item_model_in_inv;
        SetType = 0;


    }

    public void Initialize(StackableItem item)
    {
        
        Initialize((Original_Item)item);

        stackMax = item.stack_max;
        stackCurrent = item.stack_current;
        SetType = 1;
    }

    // ConsumableItem 초기화 메서드
    public void Initialize(ConsumableItem item)
    {
        Initialize((StackableItem)item);
        hungerAmount = item.hunger_amount;
        thirstAmount = item.thirst_amount;
        fatigueAmount = item.fatigue_amount;
        freshmentMax = item.freshment_max;
        freshmentCurrent = item.freshment_current;
        SetType = 2;
    }

    // PlaceableItem 초기화 메서드
    public void Initialize(PlaceableItem item)
    {
        Initialize((StackableItem)item);
        requireToolType = item.require_tool_type;
        requireToolTier = item.require_tool_tier;
        durabilityMax = item.durability_max;
        durabilityCurrent = item.durability_current;
        itemModelInPlace = item.item_model_in_place;
        SetType = 3;

    }

    // EquipmentItem 초기화 메서드
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
        drawPower = item.draw_power;
        drawSpeed = item.draw_speed;
        aimAccuracy = item.aim_accuracy;
        armorDefense = item.armor_defense;
        SetType = 4;
    }

    public void Initialize(BlockType item)
    {

    }


    public void DestroyItem()
    {
        gameObject.SetActive(false);
    }

    public int Get_Type()
    {
        return SetType;
    }


    public bool Check_Full()
    {
        if(stackCurrent < stackMax)
        {
            stackCurrent++;
            return true;
        }

        return false;

    }

    public string SetEquipType()
    {
        return equipmentType;
    }

    public ItemComponent thisItem()
    {
        return this;
    }


    



}
