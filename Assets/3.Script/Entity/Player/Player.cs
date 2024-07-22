using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Entity
{
    //�÷��̾�� �ܼ��� ��ƼƼ�� �Ӽ��� �޴� �� �Ӹ��� �ƴ� �� �� �پ��� �Ӽ��� �����ؾ� �ؿ�. -> �÷��̾�� ������ ������, �񸶸�, ����ġ ���� ���⼭ �����ϸ� ���� �������?
    private float hunger_max, hunger_current;
    private float thirst_max, thirst_current;
    private float fatigue_max, fatigue_current;

    private float health_max, health_current;
    private float posture_max, posture_current;

    private float defense_base, defense_current;
    
    private float weight_base, weight_max, wieght_current;

    private float speed_rate;
    private float speed_movement;
    private float jump_height;

    private Hand hand_L, hand_R;
}

public class Hand
{
    private float damage_max;
    private float damage_min;
    private float attack_speed;
    private float guard_rate;
}