using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Equipment_Type
{
    HELMET,
    CHESTPLATE,
    LEGGINGS,
    BOOTS,
    SHIELD,
    ONE_HANDED_SWORD,
    ONE_HANDED_AXE,
    ONE_HANDED_HAMMER,
    TWO_HANDED_SWORD,
    TWO_HANDED_AXE,
    TWO_HANDED_HAMMER,
    BOW,
    PICKAXE,
    SHOVEL,
    HOE
}

public class Item_Data : MonoBehaviour
{
    public int item_ID { get; protected set; } //������ ���̵�
    public string item_name { get; protected set; } //������ �̸�
    public GameObject item_model_in_world { get; protected set; } //�ٴڿ� ���������� �� ������ �������� ����
    public GameObject item_model_in_inventory { get; protected set; } //�κ��丮���� ������ �������� ����

    public Item_Data(int item_ID, string item_name, GameObject item_model_in_world, GameObject item_model_in_inventory)
    {
        this.item_ID = item_ID;
        this.item_name = item_name;
        this.item_model_in_world = item_model_in_world;
        this.item_model_in_inventory = item_model_in_inventory;
    }
}

public class Item_Stackable_Data : Item_Data
{
    public int stack_max { get; protected set; } //��ĥ �� �ִ� �ִ� ����
    public int stack_current { get; protected set; }

    public Item_Stackable_Data(int item_ID, string item_name, GameObject item_model_in_world, GameObject item_model_in_inventory, int stack_max, int stack_current) : 
        base(item_ID, item_name, item_model_in_world,item_model_in_inventory)
    {
        this.item_ID = item_ID;
        this.item_name = item_name;
        this.item_model_in_world = item_model_in_world;
        this.item_model_in_inventory = item_model_in_inventory;
        
        this.stack_max = stack_max;
        this.stack_current = stack_current;
    }
}
public class Item_Consumable_Data : Item_Stackable_Data
{
    public float hunger_amount { get; protected set; } //�Ծ��� �� ȸ���Ǵ� ����� ��ġ
    public float thirst_amount { get; protected set; } //�Ծ��� �� ȸ���Ǵ� �񸶸� ��ġ
    public float fatigue_amount { get; protected set; } //�Ծ��� �� ȸ���Ǵ� �Ƿε� ��ġ

    public float freshment_max { get; protected set; } //�ִ� �������
    public float freshment_current { get; protected set; } //���� �������

    public Item_Consumable_Data(int item_ID, string item_name, GameObject item_model_in_world, GameObject item_model_in_inventory, int stack_max, int stack_current, float hunger_amount, float thirst_amount, float fatigue_amount, float freshment_max, float freshment_current) :
        base(item_ID, item_name, item_model_in_world, item_model_in_inventory, stack_max, stack_current)
    {
        this.item_ID = item_ID;
        this.item_name = item_name;
        this.item_model_in_world = item_model_in_world;
        this.item_model_in_inventory = item_model_in_inventory;
        
        this.stack_max = stack_max;
        this.stack_current = stack_current;

        this.hunger_amount = hunger_amount;
        this.thirst_amount = thirst_amount;
        this.fatigue_amount = fatigue_amount;
        this.freshment_max = freshment_max;
        this.freshment_current = freshment_current;
    }
}

public class Item_Placeable_Data : Item_Stackable_Data
{
    public Equipment_Type require_tool_type { get; protected set; } //�ش� ���� �ı��ϱ� ���� �ʿ��� ������ Ÿ��
    public int require_tool_tier { get; protected set; } //�ش� ���� �ı��ϱ� ���� �ʿ��� ������ Ƽ��

    public float durability_max { get; protected set; } //�ش� ���� ü��
    public float durability_current { get; protected set; }

    public GameObject item_model_in_place { get; protected set; } //��ġ�Ǿ��� �� ������ �������� ���� (�ٵ� �̰� �ؽ��ķ� �س��� �ǳ�)

    //���� ����� �ִ� ���� ��� �̺�Ʈ�� �߰��ϴ� ������
    public Item_Placeable_Data(int item_ID, string item_name, GameObject item_model_in_world, GameObject item_model_in_inventory, int stack_max, int stack_current, Equipment_Type require_tool_type, int require_tool_tier, float durability_max, float durability_current, GameObject item_model_in_place) :
        base(item_ID, item_name, item_model_in_world, item_model_in_inventory, stack_max, stack_current)
    {
        this.item_ID = item_ID;
        this.item_name = item_name;
        this.item_model_in_world = item_model_in_world;
        this.item_model_in_inventory = item_model_in_inventory;

        this.stack_max = stack_max;
        this.stack_current = stack_current;

        this.require_tool_type = require_tool_type;
        this.require_tool_tier = require_tool_tier;
        this.durability_max = durability_max;
        this.durability_current = durability_current;
        this.item_model_in_place = item_model_in_place;
    }
}


public class Item_Equipment_Data : Item_Data
{
    public Equipment_Type equipment_type { get; protected set; } //����� Ÿ��

    public float weight { get; protected set; } //����� ����
    public float durability_max { get; protected set; } //����� �ִ� ������
    public float durability_current { get; protected set; } //����� ���� ������

    public GameObject item_model_in_equip { get; protected set; } // �������� ���� �𵨸�

    public Item_Equipment_Data(int item_ID, string item_name, GameObject item_model_in_world, GameObject item_model_in_inventory, Equipment_Type equipment_type, float weight, float durability_max, float durability_current, GameObject item_model_in_equip) :
        base(item_ID, item_name, item_model_in_world, item_model_in_inventory)
    {
        this.item_ID = item_ID;
        this.item_name = item_name;
        this.item_model_in_world = item_model_in_world;
        this.item_model_in_inventory = item_model_in_inventory;

        this.equipment_type = equipment_type;
        this.weight = weight;
        this.durability_max = durability_max;
        this.durability_current = durability_current;
        this.item_model_in_equip = item_model_in_equip;
    }
}

public class Item_Armor_Data : Item_Equipment_Data
{
    public float armor_defense { get; protected set; } //���� ����

    public Item_Armor_Data(int item_ID, string item_name, GameObject item_model_in_world, GameObject item_model_in_inventory, Equipment_Type equipment_type, float weight, float durability_max, float durability_current, GameObject item_model_in_equip, float armor_defense) :
        base(item_ID, item_name, item_model_in_world, item_model_in_inventory, equipment_type, weight, durability_max, durability_current, item_model_in_equip)
    {
        this.item_ID = item_ID;
        this.item_name = item_name;
        this.item_model_in_world = item_model_in_world;
        this.item_model_in_inventory = item_model_in_inventory;

        this.equipment_type = equipment_type;
        this.weight = weight;
        this.durability_max = durability_max;
        this.durability_current = durability_current;
        this.item_model_in_equip = item_model_in_equip;

        this.armor_defense = armor_defense;
    }
}

public class Item_Gear_Data : Item_Equipment_Data
{
    public float melee_damage { get; protected set; } //���� �Ǵ� ������ ���ݷ�
    public float melee_speed { get; protected set; } //���� �Ǵ� ������ ���ݼӵ�
    public float guard_rate { get; protected set; } //���� �Ǵ� ������ ������

    public int tool_tier { get; protected set; } //���� �Ǵ� ������ Ƽ�� (�� �ı� �ܿ��� ���� �����̼� ��� ����Ҽ���?)

    public Item_Gear_Data(int item_ID, string item_name, GameObject item_model_in_world, GameObject item_model_in_inventory, Equipment_Type equipment_type, float weight, float durability_max, float durability_current, GameObject item_model_in_equip, float melee_damage, float melee_speed, float guard_rate, int tool_tier) :
        base(item_ID, item_name, item_model_in_world, item_model_in_inventory, equipment_type, weight, durability_max, durability_current, item_model_in_equip)
    {
        this.item_ID = item_ID;
        this.item_name = item_name;
        this.item_model_in_world = item_model_in_world;
        this.item_model_in_inventory = item_model_in_inventory;

        this.equipment_type = equipment_type;
        this.weight = weight;
        this.durability_max = durability_max;
        this.durability_current = durability_current;
        this.item_model_in_equip = item_model_in_equip;

        this.melee_damage = melee_damage;
        this.melee_speed = melee_speed;
        this.guard_rate = guard_rate;
        this.tool_tier = tool_tier;
    }
}

public class Item_Bow_Data : Item_Equipment_Data
{
    public float draw_power { get; protected set; } //�߻�ü�� ���ӵ�
    public float draw_speed { get; protected set; } //���� �ӵ�
    public float aim_accuracy { get; protected set; } //������ ��Ȯ��

    public Item_Bow_Data(int item_ID, string item_name, GameObject item_model_in_world, GameObject item_model_in_inventory, Equipment_Type equipment_type, float weight, float durability_max, float durability_current, GameObject item_model_in_equip, float draw_power, float draw_speed, float aim_accuracy) :
        base(item_ID, item_name, item_model_in_world, item_model_in_inventory, equipment_type, weight, durability_max, durability_current, item_model_in_equip)
    {
        this.item_ID = item_ID;
        this.item_name = item_name;
        this.item_model_in_world = item_model_in_world;
        this.item_model_in_inventory = item_model_in_inventory;

        this.equipment_type = equipment_type;
        this.weight = weight;
        this.durability_max = durability_max;
        this.durability_current = durability_current;
        this.item_model_in_equip = item_model_in_equip;

        this.draw_power = draw_power;
        this.draw_speed = draw_speed;
        this.aim_accuracy = aim_accuracy;
    }
}