using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entity_Data;

public class Entity_Humanoid : Entity
{
    [SerializeField] private Part L_Hand;
    [SerializeField] private Part R_Hand;

    [SerializeField] private Inventory inventory = null;
    [SerializeField] private ItemComponent L_held_data, R_held_data, helmet_data, chestplate_data, leggings_data, boots_data;

    public Transform[] anchorLeft; //0 : BareHand, 1 : One_hand_Sword, 2 : One_hand_Axe, 3 : Bow
    public Transform[] anchorRight; //아래도 마찬가지



    private void Awake()
    {
        TryGetComponent(out inventory);
        Awake_Default();
        L_Hand = gameObject.transform.Find("SimplePlayer.arma/center/Body/Chest/Arm:Left:Upper/Arm:Left:Lower/Arm:Left:Hand").gameObject.GetComponent<Part>();
        R_Hand = gameObject.transform.Find("SimplePlayer.arma/center/Body/Chest/Arm:Right:Upper/Arm:Right:Lower/Arm:Right:Hand").gameObject.GetComponent<Part>();
        Update_Status_Humanoid();
    }

    private void Start()
    {
        inventory.OnChangedInv += Update_Status_Humanoid;
        
    }

    public void Update_Status_Humanoid()
    {
        if(inventory == null)
        {
            Debug.Log("Null 널 어쩌면 좋니");
        }
        //Debug.Log("event!!");

        //0 : head, 1 : chest, 2 : legs, 3 : feet, 4 : weapon1, 5 : weapon2
        if (inventory != null)
        {
            if (inventory.Equipment_Slot[0] != null) //head
            {
                helmet_data = inventory.Equipment_Slot[0];
                weight_current = weight_current + helmet_data.weight;
                defense_current = defense_current + helmet_data.armorDefense;
            }
            else helmet_data = null;
            if (inventory.Equipment_Slot[1] != null) //chest
            {
                chestplate_data = inventory.Equipment_Slot[1];
                weight_current = weight_current + chestplate_data.weight;
                defense_current = defense_current + chestplate_data.armorDefense;
            }
            else chestplate_data = null;
            if (inventory.Equipment_Slot[2] != null) // legs
            {
                leggings_data = inventory.Equipment_Slot[2];
                weight_current = weight_current + leggings_data.weight;
                defense_current = defense_current + leggings_data.armorDefense;
            }
            else leggings_data = null;
            if (inventory.Equipment_Slot[3] != null) // feet
            {
                boots_data = inventory.Equipment_Slot[3];
                weight_current = weight_current + boots_data.weight;
                defense_current = defense_current + boots_data.armorDefense;
            }
            else boots_data = null;

            if (inventory.Equipment_Slot[4] != null) //weapon1
            {
                L_held_data = inventory.Equipment_Slot[4];
                Debug.Log(L_held_data.meleeDamage + "asdsad");
               // Debug.Log(inventory.Equipment_Slot[4].equipmentType);
                L_Hand.Set_Value_Melee(attack_damage_base + L_held_data.meleeDamage, attack_damage_max_rate, attack_damage_min_rate, L_held_data.meleeSpeed, attack_speed_rate, L_held_data.toolTier);
                
                anchorLeft[0].gameObject.SetActive(false);




                EquipWeaponL(inventory.Equipment_Slot[4].equipmentType, 4);


            }
            else
            {
                L_held_data = null;
                L_Hand.Set_Value_Melee(attack_damage_base, attack_damage_max_rate, attack_damage_min_rate, 1f, attack_speed_rate, 1);
                anchorLeft[0].gameObject.SetActive(true);
                UnEquipWeaponL();


                //L_Hand.gameObject.transform.Find(inventory.Equipment_Slot[4].equipmentType).gameObject.SetActive(false);
            }

            if (inventory.Equipment_Slot[5] != null) //weapon2
            {
                R_held_data = inventory.Equipment_Slot[5];
                R_Hand.Set_Value_Melee(attack_damage_base + R_held_data.meleeDamage, attack_damage_max_rate, attack_damage_min_rate, R_held_data.meleeSpeed, attack_speed_rate, R_held_data.toolTier);
                anchorRight[0].gameObject.SetActive(false);


                EquipWeaponR(inventory.Equipment_Slot[5].equipmentType, 5);
            }
            else
            {
                R_held_data = null;
                R_Hand.Set_Value_Melee(attack_damage_base, attack_damage_max_rate, attack_damage_min_rate, 1f, attack_speed_rate, 1);
                anchorRight[0].gameObject.SetActive(true);
                UnEquipWeaponR();
            }



            if (L_held_data != null && R_held_data != null)
            {
                if (L_held_data.equipmentType == "SHIELD")
                {
                    animator.SetInteger("Moveset_Number", 1);
                    animator.SetFloat("LR_Attack_Speed", R_Hand.attack_speed);
                }
                else if (R_held_data.equipmentType == "SHIELD")
                {
                    animator.SetInteger("Moveset_Number", -1);
                    animator.SetFloat("LR_Attack_Speed", L_Hand.attack_speed);
                }
                else if (L_held_data.equipmentType.Contains("ONE_HANDED") && R_held_data.equipmentType.Contains("ONE_HANDED"))
                {
                    animator.SetInteger("Moveset_Number", 2);
                    animator.SetFloat("LR_Attack_Speed", (L_Hand.attack_speed + R_Hand.attack_speed) / 2f);
                }
            }
            else if (L_held_data != null && R_held_data == null)
            {
                if (L_held_data.equipmentType.Contains("ONE_HANDED"))
                {
                    animator.SetInteger("Moveset_Number", -1);
                    animator.SetFloat("LR_Attack_Speed", L_Hand.attack_speed);
                }
                else if (L_held_data.equipmentType.Contains("TWO_HANDED"))
                {
                    animator.SetInteger("Moveset_Number", -3);
                    animator.SetFloat("LR_Attack_Speed", L_Hand.attack_speed);
                }
                else if (L_held_data.equipmentType == "BOW")
                {
                    animator.SetInteger("Moveset_Number", -4);
                    animator.SetFloat("Draw_Speed", L_Hand.draw_speed);
                }
            }
            else if (L_held_data == null && R_held_data != null)
            {
                if (R_held_data.equipmentType.Contains("ONE_HANDED"))
                {
                    animator.SetInteger("Moveset_Number", 1);
                    animator.SetFloat("LR_Attack_Speed", R_Hand.attack_speed);
                }
                else if (R_held_data.equipmentType.Contains("TWO_HANDED"))
                {
                    animator.SetInteger("Moveset_Number", 3);
                    animator.SetFloat("LR_Attack_Speed", R_Hand.attack_speed);
                }
                else if (R_held_data.equipmentType == "BOW")
                {
                    animator.SetInteger("Moveset_Number", 4);
                    animator.SetFloat("Draw_Speed", R_Hand.draw_speed);
                }
            }
            else
            {
                animator.SetInteger("Moveset_Number", 10);
                animator.SetFloat("LR_Attack_Speed", (L_Hand.attack_speed + R_Hand.attack_speed) / 2f);
            }

            switch (animator.GetInteger("Moveset_Number"))
            {
                case 10:
                case -10:
                    guard_rate = 0.5f;
                    break;
                case 1:
                    guard_rate = R_held_data.guardRate;
                    break;
                case -1:
                    guard_rate = L_held_data.guardRate;
                    break;
                case 2:
                case -2:
                    guard_rate = (L_held_data.guardRate + R_held_data.guardRate) / 2f;
                    break;
                case 3:
                    guard_rate = R_held_data.guardRate;
                    break;
                case -3:
                    guard_rate = L_held_data.guardRate;
                    break;
                case 4:
                case -4:
                    guard_rate = 0f;
                    break;
            }
        }
        else
        {
            animator.SetInteger("Moveset_Number", 10);
            L_Hand.Set_Value_Melee(attack_damage_base, attack_damage_max_rate, attack_damage_min_rate, 1f, attack_speed_rate, 1);
            R_Hand.Set_Value_Melee(attack_damage_base, attack_damage_max_rate, attack_damage_min_rate, 1f, attack_speed_rate, 1);
            animator.SetFloat("LR_Attack_Speed", (L_Hand.attack_speed + R_Hand.attack_speed) / 2f);
            guard_rate = 0.5f;
        }
        L_Hand.Set_Collider();
        R_Hand.Set_Collider();

        movement_speed = Mathf.Max(0.1f, movement_speed - weight_rate);

        animator.SetFloat("L_Attack_Speed", L_Hand.attack_speed);
        animator.SetFloat("R_Attack_Speed", R_Hand.attack_speed);
        animator.SetFloat("Movement_Speed", movement_speed);
    }


    private void EquipWeaponL(string item, int index)
    {
        Transform temp;

        switch(item)
        {
            case "ONE_HANDED_SWORD":
                temp = anchorLeft[1];
                anchorLeft[1].gameObject.SetActive(true);
                break;
            case "ONE_HANDED_AXE":
                temp = anchorLeft[2];
                anchorLeft[2].gameObject.SetActive(true);
                break;
            case "TWO_HANDED_SWORD":
                temp = anchorLeft[3];
                anchorLeft[3].gameObject.SetActive(true);
                break;
            case "BOW":
                temp = anchorLeft[4];
                anchorLeft[4].gameObject.SetActive(true);
                break;
            default:
                return;
                

        }

        inventory.Equipment_Slot[index].transform.SetParent(temp);
        inventory.Equipment_Slot[index].transform.localPosition = Vector3.zero;
        inventory.Equipment_Slot[index].transform.localRotation = Quaternion.identity;
        inventory.Equipment_Slot[index].transform.localScale = new Vector3(1.5f, 1.5f, 1f);
        inventory.Equipment_Slot[index].gameObject.layer = 0;
        
        inventory.Equipment_Slot[index].gameObject.SetActive(true);
        
    }

    private void EquipWeaponR(string item, int index)
    {
        Transform temp;

        switch (item)
        {
            case "ONE_HANDED_SWORD":
                temp = anchorRight[1];
                anchorRight[1].gameObject.SetActive(true);
                break;
            case "ONE_HANDED_AXE":
                temp = anchorRight[2];
                anchorRight[2].gameObject.SetActive(true);
                break;
            case "TWO_HANDED_SWORD":
                temp = anchorLeft[3];
                anchorLeft[3].gameObject.SetActive(true);
                break;
            case "BOW":
                temp = anchorRight[4];
                anchorRight[4].gameObject.SetActive(true);
                break;
            default:
                return;


        }

        inventory.Equipment_Slot[index].transform.SetParent(temp);
        inventory.Equipment_Slot[index].transform.localPosition = Vector3.zero;
        inventory.Equipment_Slot[index].transform.localRotation = Quaternion.identity;
        inventory.Equipment_Slot[index].transform.localScale = new Vector3(1.5f, 1.5f, 1f);
        inventory.Equipment_Slot[index].gameObject.layer = 0;
        inventory.Equipment_Slot[index].gameObject.SetActive(true);

    }

    private void UnEquipWeaponL()
    {
       Transform temp;

       for(int i = 0; i < anchorLeft.Length; i++)
        {
            if(anchorLeft[i].childCount != 0)
            {
                temp = anchorLeft[i].GetChild(0);
                temp.transform.SetParent(inventory.Inventory_obj);
                temp.localScale = Vector3.one;
                temp.gameObject.SetActive(false);
                anchorLeft[i].gameObject.SetActive(false);
                return;
            }

        }
        
        
    }
    private void UnEquipWeaponR()
    {
        Transform temp;

        for (int i = 0; i < anchorRight.Length; i++)
        {
            if (anchorRight[i].childCount != 0)
            {
                temp = anchorRight[i].GetChild(0);
                temp.transform.SetParent(inventory.Inventory_obj);
                temp.localScale = Vector3.one;
                temp.gameObject.SetActive(false);
                anchorRight[i].gameObject.SetActive(false);
                return;
            }

        }


    }

    



    public void On_L_Hand_Collider()
    {
        if (animator.GetInteger("Moveset_Number") > 0)
        {
            if (L_Hand.Is_Collider_On_Off()) L_Hand.Collider_On_Off(false);
            else L_Hand.Collider_On_Off(true);
        }
        else if (animator.GetInteger("Moveset_Number") < 0)
        {
            if (R_Hand.Is_Collider_On_Off()) R_Hand.Collider_On_Off(false);
            else R_Hand.Collider_On_Off(true);
        }
    }
    public void On_R_Hand_Collider()
    {
        if (animator.GetInteger("Moveset_Number") > 0)
        {
            if (R_Hand.Is_Collider_On_Off()) R_Hand.Collider_On_Off(false);
            else R_Hand.Collider_On_Off(true);
        }
        else if (animator.GetInteger("Moveset_Number") < 0)
        {
            if (L_Hand.Is_Collider_On_Off()) L_Hand.Collider_On_Off(false);
            else L_Hand.Collider_On_Off(true);
        }
    }
    public void Reset_Hand_Collider()
    {
        L_Hand.Collider_On_Off(false);
        R_Hand.Collider_On_Off(false);
    }
}