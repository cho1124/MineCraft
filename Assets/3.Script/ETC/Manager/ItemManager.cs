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
        //LoadItemsFromJson(itemJsonFile.text);

        //testDic = Item_Dictionary.item_dictionary;
        //testDic.ContainsKey(5);
        //Debug.Log(Item_Dictionary.item_dictionary.ContainsKey(5));



    }

    /*void LoadItemsFromJson(string json)
    {
        // ������ȭ
        Item_List itemList = JsonUtility.FromJson<Item_List>(json);

        



        Debug.Log(itemList.item_Consumable_Datas.Count + ", " + itemList.item_Equipment_Datas.Count);

        // ���� ���� ������ �ε�
        foreach (var item in itemList.item_Stackable_Datas)
        {

            Debug.Log("112");

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
        foreach (var item in itemList.item_Consumable_Datas)
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
        foreach (var item in itemList.item_Placeable_Datas)
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
        foreach (var item in itemList.item_Equipment_Datas)
        {
            switch (item.equipment_type)
            {
                case Equipment_Type.HELMET:
                    //Item_Dictionary.Add(item.item_ID, new Item_Armor_Data())
                    break;
                case Equipment_Type.CHESTPLATE:
                    break;
                case Equipment_Type.LEGGINGS:
                    break;
                case Equipment_Type.BOOTS:
                    break;
                case Equipment_Type.SHIELD:
                    break;
                case Equipment_Type.ONE_HANDED_SWORD:
                    break;
                case Equipment_Type.ONE_HANDED_AXE:
                    break;
                case Equipment_Type.ONE_HANDED_HAMMER:
                    break;
                case Equipment_Type.TWO_HANDED_SWORD:
                    break;
                case Equipment_Type.TWO_HANDED_AXE:
                    break;
                case Equipment_Type.TWO_HANDED_HAMMER:
                    break;
                case Equipment_Type.BOW:
                    break;
                case Equipment_Type.PICKAXE:
                    break;
                case Equipment_Type.SHOVEL:
                    break;
                case Equipment_Type.HOE:
                    break;
            }


            //Item_Dictionary.Add(item.item_ID, new Item_Gear_Data(
            //    item.item_ID,
            //    item.item_name,
            //    item.item_model_in_world,
            //    item.item_model_in_inventory,
            //    item.equipment_type,
            //    item.weight,
            //    item.durability_max,
            //    item.durability_current,
            //    item.item_model_in_equip,
            //    item.melee_damage,
            //    item.melee_speed,
            //    item.guard_rate,
            //    item.tool_tier
            //));
        }
    }
    */
}


