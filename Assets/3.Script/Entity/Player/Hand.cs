using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
    public float damage_max { get; private set; }
    public float damage_min { get; private set; }
    public float attack_speed { get; private set; }
    public float guard_rate { get; private set; }

    public float draw_power { get; private set; }
    public float draw_speed { get; private set; }
    public float aim_accuracy { get; private set; }

    private int tool_tier;

    private Collider collider;


    public void Set_Value_Test()
    {
        damage_max = 10f;
        damage_min = 5f;
        attack_speed = 2f;
        guard_rate = 0.6f;
        tool_tier = 1;
    }
    public void Set_Value_Default() //기본 데미지만 받아오면 될듯?
    {
        //맨손으로 초기화
    }
    public void Set_Value_Melee(float melee_damage, float attack_damage_max_rate, float attack_damage_min_rate, float melee_speed, float attack_speed_rate, float guard_rate, int tool_tier)
    {
        damage_max = melee_damage * attack_damage_max_rate;
        damage_min = damage_max * attack_damage_min_rate;
        attack_speed = melee_speed * attack_speed_rate;
        this.guard_rate = guard_rate;
        this.tool_tier = tool_tier;
    }
    public void Set_Value_Range(float draw_power, float draw_power_rate, float draw_speed, float draw_speed_rate, float aim_accuracy, float aim_accuracy_rate)
    {
        this.draw_power = draw_power * draw_power_rate;
        this.draw_speed = draw_speed * draw_speed_rate;
        this.aim_accuracy = aim_accuracy * aim_accuracy_rate;
    }



    public void Set_Collider_Default()
    {
        this.collider = null;
    }
    public void Set_Collider(Collider collider)
    {
        this.collider = collider;
        //Collider_Off();
    }



    public void Collider_On()
    {
        collider.enabled = true;
    }
    public void Collider_Off()
    {
        collider.enabled = false;
        //딕셔너리 초기화
    }

    private void OnTriggerStay(Collider victim)
    {
        if (victim.CompareTag("Entity"))
        {
            victim.gameObject.GetComponent<Entity>().On_Hit(UnityEngine.Random.Range(damage_min, damage_max), collider);
            //공격한 대상은 딕셔너리에 추가하고 딕셔너리에 있는 엔티티는 다시 처리 안함
        }
    }
}