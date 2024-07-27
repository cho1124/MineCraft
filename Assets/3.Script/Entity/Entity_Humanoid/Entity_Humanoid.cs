using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entity_Data;

public class Entity_Humanoid : Entity
{
    [SerializeField] private Part L_Hand;
    [SerializeField] private Part R_Hand;

    [SerializeField] private ItemComponent L_held_data, R_held_data, helmet_data, chestplate_data, leggings_data, boots_data;

    private void Awake()
    {
        Awake_Default();
        Update_Status_Humanoid();
    }

    public void Update_Status_Humanoid()
    {
        //인벤토리의 장비칸을 읽어오고 각 능력치에 추가
        //weight_current = weight_base + helmet_data.weight + chestplate_data.weight + leggings_data.weight + boots_data.weight + L_held_data.weight + R_held_data.weight;
        //defense_current = defense_base + helmet_data.armorDefense + chestplate_data.armorDefense + leggings_data.armorDefense + boots_data.armorDefense;

        //무기 조합에 따른 무브셋 넘버 지정 및 Set_Value
        //L_Hand.Set_Value_Melee(attack_damage_base + L_held_data.meleeDamage, attack_damage_max_rate, attack_damage_min_rate, L_held_data.meleeSpeed, attack_speed_rate, L_held_data.toolTier);
        //R_Hand.Set_Value_Melee(attack_damage_base + R_held_data.meleeDamage, attack_damage_max_rate, attack_damage_min_rate, R_held_data.meleeSpeed, attack_speed_rate, R_held_data.toolTier);
        L_Hand.Set_Value_Melee(attack_damage_base, attack_damage_max_rate, attack_damage_min_rate, 1f, attack_speed_rate, 1); //이건 맨손 기준
        R_Hand.Set_Value_Melee(attack_damage_base, attack_damage_max_rate, attack_damage_min_rate, 1f, attack_speed_rate, 1); //이건 맨손 기준
        L_Hand.Set_Collider();
        R_Hand.Set_Collider();

        //animator.SetInteger("Moveset_Number", moveset_number);
        animator.SetFloat("LR_Attack_Speed", (L_Hand.attack_speed + R_Hand.attack_speed) / 2f);
        animator.SetFloat("L_Attack_Speed", L_Hand.attack_speed);
        animator.SetFloat("R_Attack_Speed", R_Hand.attack_speed);
        animator.SetFloat("Movement_Speed", movement_speed);
        //guard_rate 설정
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