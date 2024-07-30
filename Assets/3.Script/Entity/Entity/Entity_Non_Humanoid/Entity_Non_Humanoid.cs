using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entity_Data;

public class Entity_Non_Humanoid : Entity
{
    [SerializeField] private Part head;
    [SerializeField] private Part body;

    private void Awake()
    {
        Awake_Default();
        Update_Status_Non_Humanoid();
    }

    private void Update_Status_Non_Humanoid()
    {
        head.Set_Value_Melee(attack_damage_base, attack_damage_max_rate, attack_damage_min_rate, 1f, attack_speed_rate, 1);
        body.Set_Value_Melee(attack_damage_base, attack_damage_max_rate, attack_damage_min_rate, 1f, attack_speed_rate, 1);
        head.Set_Collider();
        body.Set_Collider();

        movement_speed = Mathf.Max(0.1f, movement_speed - weight_rate);

        animator.SetFloat("Head_Attack_Speed", head.attack_speed);
        animator.SetFloat("Body_Attack_Speed", body.attack_speed);
        animator.SetFloat("Movement_Speed", movement_speed);
    }

    public void On_Head_Collider()
    {
        if (head.Is_Collider_On_Off()) head.Collider_On_Off(false);
        else head.Collider_On_Off(true);

    }
    public void On_Body_Collider()
    {
        if (body.Is_Collider_On_Off()) body.Collider_On_Off(false);
        else body.Collider_On_Off(true);
    }
}