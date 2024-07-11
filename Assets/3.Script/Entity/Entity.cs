using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    // Start is called before the first frame update
    private float hunger_rate;
    private float thirst_rate;
    private float fatigue_rate;

    private float VIT;
    private float END;
    private float STR;
    private float DEX;
    private float AGI;

    private float health_max;
    private float health_current;
    private float posture_max;
    private float posture_current;
    
    private float defense_base;
    private float defense_current;
    
    private float weight_max;
    private float weight_base;
    private float weight_current;

    private float speed_rate;
    private float speed_walk;
    private float speed_sprint;
    private float jump_height;

    private Hand L_hand;
    private Hand R_hand;

    protected GameObject[] equipments; // ���� ���� ���� �Ź� �޼� ������
    private GameObject[] inventory;

    public void uptate_status()
    {
        health_max = 100 * (1 + VIT * 0.01f);
        posture_max = 100 * (1 + weight_current * 0.01f);

        //�÷��� ���� ����
    }

    public void OnDamage(float damage)
    {
    }


//�������� Ŀ���� �ؼ� ���� �� ��
}

public class Hand
{
    private float damage_min;
    private float damage_max;
    private float attack_speed;
    private float guard_rate;
    private string[] actions;
}

public class Equipment
{

}