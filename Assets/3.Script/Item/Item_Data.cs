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
    SWORD_LIGHT,
    SWORD_HEAVY,    
    AXE_LIGHT,
    AXE_HEAVY,    
    BLUNT_LIGHT,
    BLUNT_HEAVY,
    BOW,
    PICKAXE,
    SHOVEL,
    HOE
}

public class Item_Data
{
    public string item_ID; //������ ���̵�
    public string item_name; //������ �̸�
    public GameObject item_model_in_world; //�ٴڿ� ���������� �� ������ �������� ����
    public GameObject item_model_in_inventory; //�κ��丮���� ������ �������� ����
}

public class Item_Stackable_Data : Item_Data
{
    public int stack_max; //��ĥ �� �ִ� �ִ� ����
    public int stack_current;
}
public class Item_Consumable_Data : Item_Stackable_Data
{
    public float hunger_amount; //�Ծ��� �� ȸ���Ǵ� ����� ��ġ
    public float thirst_amount; //�Ծ��� �� ȸ���Ǵ� �񸶸� ��ġ
    public float fatigue_amount; //�Ծ��� �� ȸ���Ǵ� �Ƿε� ��ġ

    public float freshness_max; //�ִ� �������
    public float freshness_current; //���� �������
}

public class Item_Placeable_Data : Item_Stackable_Data
{
    public Equipment_Type require_tool_type; //�ش� ���� �ı��ϱ� ���� �ʿ��� ������ Ÿ��
    public int require_tool_tier; //�ش� ���� �ı��ϱ� ���� �ʿ��� ������ Ƽ��

    public float durability_max; //�ش� ���� ü��
    public float durability_current;
    
    public GameObject item_model_in_place; //��ġ�Ǿ��� �� ������ �������� ���� (�ٵ� �̰� �ؽ��ķ� �س��� �ǳ�)

    //���� ����� �ִ� ���� ��� �̺�Ʈ�� �߰��ϴ� ������
}


public class Item_Equipment_Data : Item_Data
{
    public Equipment_Type equipment_type; //����� Ÿ��

    public float weight; //����� ����
    public float durability_max; //����� �ִ� ������
    public float durability_current; //����� ���� ������

    public GameObject item_model_in_equip; // �������� ���� �𵨸�
}

public class Item_Armor_Data : Item_Equipment_Data
{
    public float armor_defense; //���� ����
}

public class Item_Gear_Data : Item_Equipment_Data
{
    public float attack_damage; //���� �Ǵ� ������ ���ݷ�
    public float attack_speed; //���� �Ǵ� ������ ���ݼӵ�
    public float guard_rate; //���� �Ǵ� ������ ������

    public int tool_tier; //���� �Ǵ� ������ Ƽ�� (�� �ı� �ܿ��� ���� �����̼� ��� ����Ҽ���?)
}

public class item_Bow_Data : Item_Equipment_Data
{
    public float attack_damage;
    public float draw_speed;
    public float aim_accuracy;
}