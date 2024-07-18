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

    public void Status_Calculator()
    {
        //health_max = 100 * (1 + VIT * 0.01f);
        //posture_max = 100 * (1 + weight_current * 0.01f);
        //...
        //올려둔 공식 참조
    }

    public void Hit(float damage)
    {
        //올려둔 공식 참조
    }
    public void Attack(GameObject victim)
    {
        victim.GetComponent<Entity>().Hit(1);
    }
//나머지는 커스텀 해서 구현 할 것
}

public class Hand
{
    public float attack_damage_min { get; private set; }
    public float attack_damage_max { get; private set; }
    public float attack_speed { get; private set; }
    public float guard_rate { get; private set; }

    public void Set_Hand(GameObject entity, GameObject weapon)
    {
        
    }
}

public class Entity_Humanoid_Data : Entity
{
    private Hand L_hand;
    private Hand R_hand;

    private GameObject[] equipments; // 투구 갑옷 바지 신발 왼손 오른손
    private GameObject[] inventory;
}

public class Entity_NonHumanoid_Data : Entity
{
    
}