using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Entity
{
    //플레이어는 단순히 엔티티의 속성을 받는 것 뿐만이 아닌 좀 더 다양한 속성을 구현해야 해요. -> 플레이어만이 가지는 포만도, 목마름, 경험치 등을 여기서 구현하면 되지 않을까요?
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