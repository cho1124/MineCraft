using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ItemManager : MonoBehaviour
{
    public TextAsset itemJsonFile; // �ߺ�


    //�� �κ� ��ųʸ��� ���� ����

    //TODO
    //�̺κ��� ����� json ��ųʸ��� �о �Ŵ����� ������ ��
    //�̷��� �������� �����ϰ� �ִٰ� �ʿ��Ҷ����� �������� ����
    //���⼭ ���� �������� -> �������� instantiate �Ǵ� ���̱� ������ ���صǴ� �κ��� ��������? -> �������� �������̴��� ��ϵ��� ���� �������ΰ�
    //�� �ڿ� �κ��丮���� ������ Ÿ�Կ� ���� ī��Ʈ�� �ö� ������ �� ���� ó��, ���� ��� ĭ �κ� �迭���� �巡�� �� ��ӵǴ� ������ �����Ͱ� equipment �����ϰ� �� ��ġ�� �´� �ش��ϴ� �������� �˻��ϰ� �´ٸ� ����
    //������ ������ �÷��̾��� �������ͽ��� ���� -> �� �κ��� DI Ȥ�� event system �ƴϸ� �ִ��� �κ��丮 �ȿ� �ִ� ������ �����Ϳ��� �̹� ������ ����Ǿ� ���� ���̱� ������ ������Ʈ�� �޾ƿͼ� ó���ص� �ɵ�
    //��� �׳� ��ũ���ͺ��̰� �������̰� �׳� �ᵵ �Ǳ� �ϴµ�, Ȯ�强���� �� ȿ�����̱� ������ �׳� �Ẹ�°�
    //�׷��� ������ ���� �����Ϳ� ���� �� �׳� ��ũ���ͺ� �ᵵ �ɵ�
    //���ζ��� ���̳ʸ� ���� �κп� �÷��̾� �����͸� �����Ѵ�. >>> �翬�� �ΰ��� �ð��� ���� �ǰ���?
    //�÷��̾� ������ >>> �÷��̾� �������ͽ�, �κ��丮
    //�κ��丮�� �����ϱ� ���ؼ��� �� ���� ����ȭ�� �Ͽ� json�̴� ��� �ؾߵɵ�
    //���� �ִ� ������ �ε��� ���� ���� ���� Ȥ�� �� �ƹ�ư ����� ���Ͽ��� ���� ȣ��
    //�÷��̾�� �ٸ� ��ƼƼ���� ��ȣ�ۿ��� �Ͻ��ϴ�.
    //�� ���� ������ ó���ϴ� �κ��� �����ϱ��� ������ ���������� �� �ݿ� ���� �� ��
    //�� ������ ������ ������ ���� ó���� �������� �ҵ�
    //���� ó���� ���� �κ��� �̹� ������Ʈ������ �ٷ�� �����, �⺻���� ���常 ó���ϰ�, ���������� �κе��� ���� ������Ʈ�� �ϴ°ɷ�
    //��ƼƼ ������ ���� �κ� -> ���� �Ŵ��� ���� �׳� ��ũ���ͺ� �� �����ڰ� ������
    //�÷��̾� ������ ���� ó�� >> ���ն찡 ���� ���� �޼��带 Ȱ���ؼ� �κ��丮 ���� ����Ʈ�� ���� �� ������ ���� �ݺ� -> �ƴϸ� �̰� ���� ���� �޼��带 ���� ó���ص� �ɵ�
    //��Ȯ���� ��� �޼���� ���°� ���� �� ���׿�
    //��� ���� �߿��� ���� ����ȭ !!!!!!!!!!!!!!!!!!!!!!!!
    //�񵿱� �ε��� ���� ���ΰ�? -> ������ �ε�Ǵ� ���º��� �Ǵ�
    //���� ����
    //������ ���� ���� ���� : ���� 2��, �Ź� : ������, ������ : �ϴ� ������
    //Damage�޴� �κ� �������̽��� ó���ص� �Ǳ� �ϴµ� ������ entity �ڽ� Ŭ�������̸� �׳� ��� �޾Ƶ� �ɵ�



    void Start()
    {
        LoadItemsFromJson(itemJsonFile.text);
        
        

    }

    void LoadItemsFromJson(string json)
    {
        // ������ȭ
        ItemDataList itemDataList = JsonUtility.FromJson<ItemDataList>(json);

        // ���� ���� ������ �ε�
        foreach (var item in itemDataList.stackableItems)
        {
            Item_Dictionary.Add(item.item_ID, new Item_Stackable_Data(
                item.item_ID,
                item.item_name,
                item.item_model_in_world,
                item.item_model_in_inventory,
                item.stack_max,
                item.stack_current
            ));
        }

        // �Һ� ������ �ε�
        foreach (var item in itemDataList.consumableItems)
        {
            Item_Dictionary.Add(item.item_ID, new Item_Consumable_Data(
                item.item_ID,
                item.item_name,
                item.item_model_in_world,
                item.item_model_in_inventory,
                item.stack_max,
                item.stack_current,
                item.hunger_amount,
                item.thirst_amount,
                item.fatigue_amount,
                item.freshment_max,
                item.freshment_current
            ));
        }

        // ��ġ ���� ������ �ε�
        foreach (var item in itemDataList.placeableItems)
        {
            Item_Dictionary.Add(item.item_ID, new Item_Placeable_Data(
                item.item_ID,
                item.item_name,
                item.item_model_in_world,
                item.item_model_in_inventory,
                item.stack_max,
                item.stack_current,
                item.require_tool_type,
                item.require_tool_tier,
                item.durability_max,
                item.durability_current,
                item.item_model_in_place
            ));
        }

        // ��� ������ �ε�
        foreach (var item in itemDataList.equipmentItems)
        {
            Item_Dictionary.Add(item.item_ID, new Item_Gear_Data(
                item.item_ID,
                item.item_name,
                item.item_model_in_world,
                item.item_model_in_inventory,
                item.equipment_type,
                item.weight,
                item.durability_max,
                item.durability_current,
                item.item_model_in_equip,
                item.melee_damage,
                item.melee_speed,
                item.guard_rate,
                item.tool_tier
            ));
        }
    }
}

[System.Serializable]
public class ItemDataList
{
    public List<ItemJsonStackable> stackableItems;
    public List<ItemJsonConsumable> consumableItems;
    public List<ItemJsonPlaceable> placeableItems;
    public List<ItemJsonEquipment> equipmentItems;
}

[System.Serializable]
public class ItemJsonStackable
{
    public int item_ID;
    public string item_name;
    public string item_model_in_world;
    public string item_model_in_inventory;
    public int stack_max;
    public int stack_current;
}

[System.Serializable]
public class ItemJsonConsumable : ItemJsonStackable
{
    public float hunger_amount;
    public float thirst_amount;
    public float fatigue_amount;
    public float freshment_max;
    public float freshment_current;
}

[System.Serializable]
public class ItemJsonPlaceable : ItemJsonStackable
{
    public Equipment_Type require_tool_type;
    public int require_tool_tier;
    public float durability_max;
    public float durability_current;
    public string item_model_in_place;
}

[System.Serializable]
public class ItemJsonEquipment
{
    public int item_ID;
    public string item_name;
    public string item_model_in_world;
    public string item_model_in_inventory;
    public Equipment_Type equipment_type;
    public float weight;
    public float durability_max;
    public float durability_current;
    public string item_model_in_equip;
    public float melee_damage;
    public float melee_speed;
    public float guard_rate;
    public int tool_tier;
}

