using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;

public class Item_Create_Editor
{

}

public class Item_Create_Editor_Window : EditorWindow
{
    private TextAsset item_json;

    private string item_type;

    private int item_ID;
    private string item_name;
    private string item_model_in_world;
    private string item_model_in_inventory;

    //stackable
    private int stack_max;
    private int stack_current;

    //consumable
    private float hunger_amount;
    private float thirst_amount;
    private float fatigue_amount;

    private float freshment_max;
    private float freshment_current;

    //placeable
    private Equipment_Type require_tool_type;
    private int require_tool_tier;

    private float block_durability_max;
    private float block_durability_current;

    private string item_model_in_place;

    //equipment
    private Equipment_Type equipment_type;

    private float weight;
    private float durability_max;
    private float durability_current;

    private string item_model_in_equip;

    //armor
    private float armor_defense;

    //gear
    private float melee_damage;
    private float melee_speed;
    private float guard_rate;

    private int tool_tier;

    //bow
    private float draw_power;
    private float draw_speed;
    private float aim_accuracy;



    [MenuItem("Window/Item Create Editor")]
    public static void ShowWindow()
    {
        GetWindow<Item_Create_Editor_Window>("Item_Create_Editor");
    }

    private void OnGUI()
    {
        GUILayout.Label("JSON File", EditorStyles.boldLabel);
        item_json = (TextAsset)EditorGUILayout.ObjectField(item_json, typeof(TextAsset), false);



        GUILayout.Label("", EditorStyles.boldLabel);
        GUILayout.Label("", EditorStyles.boldLabel);
        GUILayout.Label("", EditorStyles.boldLabel);



        GUILayout.Label("Item Type", EditorStyles.boldLabel);
        item_type = (string)EditorGUILayout.TextField(item_type);



        GUILayout.Label("", EditorStyles.boldLabel);



        GUILayout.Label("Item_ID", EditorStyles.boldLabel);
        item_ID = (int)EditorGUILayout.IntField(item_ID);
        GUILayout.Label("Item_Name", EditorStyles.boldLabel);
        item_name = (string)EditorGUILayout.TextField(item_name);
        GUILayout.Label("Item_Model_In_World", EditorStyles.boldLabel);
        item_model_in_world = (string)EditorGUILayout.TextField(item_model_in_world);
        GUILayout.Label("Item_Model_In_Inventory", EditorStyles.boldLabel);
        item_model_in_inventory = (string)EditorGUILayout.TextField(item_model_in_inventory);

        GUILayout.Label("", EditorStyles.boldLabel);

        switch (item_type)
        {
            case "Item":
                if (GUILayout.Button("Create Item"))
                {
                    GameObject item = new GameObject(item_name);
                    Item_Data item_data = new Item_Data(item_ID, item_name, item_model_in_world, item_model_in_inventory);
                    //item_data = item.AddComponent<Item_Data>();
                    //
                    //Instantiate(item);

                    //이런 식으로 딕셔너리에 먼저 저장해두고 나중에 빈 오브젝트에 컴포넌트 형식으로 데이터를 넣고? 자식으로 모델링을 넣으면? 된다?
                    //Item_Dictionary.Add(item_data.item_ID, item_data);

                    //아이템 생성 부분은 다시 생각해보자.
                    //이 부분에서 직접 생성하는 것이 아닌 데이터만 불러와 놓고 있다가 필요할 때만 생성하는 방식, 따라서 에디터에서도 무기 만들기와 마찬가지로 json으로 데이터를 가지고 있다가 게임이 시작할 때 딕셔너리 형태로 값을 받아온 다음에 필요할 때마다 생성해서 쓰면 될듯 합니다.
                    //굳이 풀링까지도 갈 것 없을것 같아요.
                    
                }
                break;

            case "Item_Stackable":
                GUILayout.Label("Stack_Max", EditorStyles.boldLabel);
                stack_max = (int)EditorGUILayout.IntField(stack_max);
                GUILayout.Label("Stack_Current", EditorStyles.boldLabel);
                stack_current = (int)EditorGUILayout.IntField(stack_current);



                if (GUILayout.Button("Create Item_Stackable"))
                {
                    
                }
                break;

            case "Item_Consumable":
                GUILayout.Label("Stack_Max", EditorStyles.boldLabel);
                stack_max = (int)EditorGUILayout.IntField(stack_max);
                GUILayout.Label("Stack_Current", EditorStyles.boldLabel);
                stack_current = (int)EditorGUILayout.IntField(stack_current);



                GUILayout.Label("", EditorStyles.boldLabel);
                


                GUILayout.Label("Hunger_Amount", EditorStyles.boldLabel);
                hunger_amount = (float)EditorGUILayout.FloatField(hunger_amount);
                GUILayout.Label("Thirst_Amount", EditorStyles.boldLabel);
                thirst_amount = (float)EditorGUILayout.FloatField(thirst_amount);
                GUILayout.Label("Fatigue_Amount", EditorStyles.boldLabel);
                fatigue_amount = (float)EditorGUILayout.FloatField(fatigue_amount);
                GUILayout.Label("Freshment_Max", EditorStyles.boldLabel);
                freshment_max = (float)EditorGUILayout.FloatField(freshment_max);
                GUILayout.Label("Freshment_Current", EditorStyles.boldLabel);
                freshment_current = (float)EditorGUILayout.FloatField(freshment_current);



                if (GUILayout.Button("Create Item_Consumable"))
                {

                }
                break;

            case "Item_Placeable":
                GUILayout.Label("Stack_Max", EditorStyles.boldLabel);
                stack_max = (int)EditorGUILayout.IntField(stack_max);
                GUILayout.Label("Stack_Current", EditorStyles.boldLabel);
                stack_current = (int)EditorGUILayout.IntField(stack_current);
                


                GUILayout.Label("", EditorStyles.boldLabel);



                GUILayout.Label("Require_Tool_Type", EditorStyles.boldLabel);
                require_tool_type = (Equipment_Type)EditorGUILayout.IntField((int)require_tool_type);
                GUILayout.Label("Require_Tool_Tier", EditorStyles.boldLabel);
                require_tool_tier = (int)EditorGUILayout.IntField(require_tool_tier);

                GUILayout.Label("Block_Durability_Max", EditorStyles.boldLabel);
                block_durability_max = (float)EditorGUILayout.FloatField(block_durability_max);
                GUILayout.Label("Block_Durability_Current", EditorStyles.boldLabel);
                block_durability_current = (float)EditorGUILayout.FloatField(block_durability_current);

                GUILayout.Label("Item_Model_In_Place", EditorStyles.boldLabel);
                item_model_in_place = (string)EditorGUILayout.TextField(item_model_in_place);



                if (GUILayout.Button("Create Item_Placeable"))
                {

                }
                break;

            case "Item_Armor":
                GUILayout.Label("Equipment_Type", EditorStyles.boldLabel);
                equipment_type = (Equipment_Type)EditorGUILayout.IntField((int)equipment_type);

                GUILayout.Label("Weight", EditorStyles.boldLabel);
                weight = (float)EditorGUILayout.FloatField(weight);
                GUILayout.Label("Durability_Max", EditorStyles.boldLabel);
                durability_max = (float)EditorGUILayout.FloatField(durability_max);
                GUILayout.Label("Durability_Current", EditorStyles.boldLabel);
                durability_current = (float)EditorGUILayout.FloatField(durability_current);

                GUILayout.Label("Item_Model_In_Equip", EditorStyles.boldLabel);
                item_model_in_equip = (string)EditorGUILayout.TextField(item_model_in_equip);



                GUILayout.Label("", EditorStyles.boldLabel);



                GUILayout.Label("Armor_Defense", EditorStyles.boldLabel);
                armor_defense = (float)EditorGUILayout.FloatField(armor_defense);



                if (GUILayout.Button("Create Item_Armor"))
                {

                }
                break;

            case "Item_Gear":
                GUILayout.Label("Equipment_Type", EditorStyles.boldLabel);
                equipment_type = (Equipment_Type)EditorGUILayout.IntField((int)equipment_type);

                GUILayout.Label("Weight", EditorStyles.boldLabel);
                weight = (float)EditorGUILayout.FloatField(weight);
                GUILayout.Label("Durability_Max", EditorStyles.boldLabel);
                durability_max = (float)EditorGUILayout.FloatField(durability_max);
                GUILayout.Label("Durability_Current", EditorStyles.boldLabel);
                durability_current = (float)EditorGUILayout.FloatField(durability_current);

                GUILayout.Label("Item_Model_In_Equip", EditorStyles.boldLabel);
                item_model_in_equip = (string)EditorGUILayout.TextField(item_model_in_equip);



                GUILayout.Label("", EditorStyles.boldLabel);



                GUILayout.Label("Melee_Damage", EditorStyles.boldLabel);
                melee_damage = (float)EditorGUILayout.FloatField(melee_damage);
                GUILayout.Label("Melee_Speed", EditorStyles.boldLabel);
                melee_speed = (float)EditorGUILayout.FloatField(melee_speed);
                GUILayout.Label("Guard_Rate", EditorStyles.boldLabel);
                guard_rate = (float)EditorGUILayout.FloatField(guard_rate);

                GUILayout.Label("Tool_Tier", EditorStyles.boldLabel);
                tool_tier = (int)EditorGUILayout.IntField(tool_tier);



                if (GUILayout.Button("Create Item_Gear"))
                {

                }
                break;

            case "Item_Bow":
                GUILayout.Label("Equipment_Type", EditorStyles.boldLabel);
                equipment_type = (Equipment_Type)EditorGUILayout.IntField((int)equipment_type);

                GUILayout.Label("Weight", EditorStyles.boldLabel);
                weight = (float)EditorGUILayout.FloatField(weight);
                GUILayout.Label("Durability_Max", EditorStyles.boldLabel);
                durability_max = (float)EditorGUILayout.FloatField(durability_max);
                GUILayout.Label("Durability_Current", EditorStyles.boldLabel);
                durability_current = (float)EditorGUILayout.FloatField(durability_current);

                GUILayout.Label("Item_Model_In_Equip", EditorStyles.boldLabel);
                item_model_in_equip = (string)EditorGUILayout.TextField(item_model_in_equip);



                GUILayout.Label("", EditorStyles.boldLabel);



                GUILayout.Label("Draw_Power", EditorStyles.boldLabel);
                draw_power = (float)EditorGUILayout.FloatField(draw_power);
                GUILayout.Label("Draw_Speed", EditorStyles.boldLabel);
                draw_speed = (float)EditorGUILayout.FloatField(draw_speed);
                GUILayout.Label("Aim_Accuracy", EditorStyles.boldLabel);
                aim_accuracy = (float)EditorGUILayout.FloatField(aim_accuracy);



                if (GUILayout.Button("Create Item_Bow"))
                {

                }
                break;
        }
    }

    private void Parse_JSON_File(TextAsset item_json)
    {

    }
}
