using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using System;

public class Survival_Status
{
    private float hunger_max, hunger_current;
    private float thirst_max, thirst_current;
    private float fatigue_max, fatigue_current;

    //시간이 지날 때마다 감소하는 메소드 추가할 것
}

public class Hand
{
    private float damage_max;
    private float damage_min;
    private float attack_speed;
    private float guard_rate;

    private float draw_power;
    private float draw_speed;
    private float aim_accuracy;

    private int tool_tier;

    private Collider collider;

    public void Set_Value_Default()
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
        Collider_Off();
    }



    public void Collider_On()
    {
        collider.enabled = true;
    }
    public void Collider_Off()
    {
        collider.enabled = false;
    }
}

public class Entity : MonoBehaviour
{
    // 데미지를 입으면 3초간 빨간색으로 깜박거리는 코드 (필요시 삭제수정해주세요)
    // entity가 공격받을때 빨갛게 변함
    // 죽을때 파티클로 이펙트
    // awake에서 entityeditor 에 있는 정보 적용 =>프리팹 수기로 체력 데미지 조정함
    // 동물과 몬스터 공격과 데미지 처리 하는 부분 해당 클래스에 구현
    //TakeDamage 메서드가 실행될 때 체력이 0 이하가 되면 Die 메서드가 호출되며, 이는 OnDeath 이벤트를 트리거합니다.



    public string entity_type;
    public string entity_name;

    private float VIT, END, STR, DEX, AGI;

    private float health_max;
    private float health_current;

    private float posture_max;
    private float posture_current;

    private float weight_base;
    private float weight_max;
    private float weight_current;

    private float weight_rate;

    private float defense_base;
    private float defense_current;

    private float attack_damage_base;
    private float attack_damage_max_rate;
    private float attack_damage_min_rate;
    private float attack_speed_rate;

    private int moveset_number;

    private float speed_movement;
    private float jump_height;

    [SerializeField] private GameObject L_Hand;
    [SerializeField] private GameObject R_Hand;

    private EquipmentItem L_gear_data, R_gear_data;

    private EquipmentItem helmet_data, chestplate_data, leggings_data, boots_data;

    public void Update_Status()
    {

        //장착된 장비들의 데이터 읽기

        health_max = 100f * (1f + VIT * 0.01f);

        weight_base = VIT * 1f + END * 0.5f;
        weight_max = END * 1f + STR * 0.5f;
        weight_current = weight_base + helmet_data.weight + chestplate_data.weight + leggings_data.weight + boots_data.weight + L_gear_data.weight + R_gear_data.weight;

        weight_rate = 1f - weight_current / weight_max;

        posture_max = weight_current;

        defense_base = END * 5f;
        defense_current = defense_base + helmet_data.armor_defense + chestplate_data.armor_defense + leggings_data.armor_defense + boots_data.armor_defense;

        attack_damage_base = STR * 0.5f;
        attack_damage_max_rate = 1f + STR * 0.01f;
        attack_damage_min_rate = 0.5f + DEX * 0.005f;
        attack_speed_rate = 1f + AGI * 0.005f;

        speed_movement = 1f + AGI * 0.0025f - weight_rate;
        jump_height = 1f * AGI * 0.0025f - weight_rate;



        //여기서 무기 조합에 따른 무브셋 지정
        if (moveset_number == 1 || moveset_number == -1 || moveset_number == 2 || moveset_number == -2)
        {
            L_Hand.GetComponent<Hand>().Set_Value_Melee(L_gear_data.melee_damage, attack_damage_max_rate, attack_damage_min_rate, L_gear_data.melee_speed, attack_speed_rate, L_gear_data.guard_rate, L_gear_data.tool_tier);
            R_Hand.GetComponent<Hand>().Set_Value_Melee(R_gear_data.melee_damage, attack_damage_max_rate, attack_damage_min_rate, R_gear_data.melee_speed, attack_speed_rate, R_gear_data.guard_rate, R_gear_data.tool_tier);
            L_Hand.GetComponent<Hand>().Set_Collider(L_Hand.GetComponentInChildren<Collider>());
            R_Hand.GetComponent<Hand>().Set_Collider(R_Hand.GetComponentInChildren<Collider>());
        }
        else if (moveset_number == 3)
        {
            L_Hand.GetComponent<Hand>().Set_Value_Default();
            R_Hand.GetComponent<Hand>().Set_Value_Melee(R_gear_data.melee_damage, attack_damage_max_rate, attack_damage_min_rate, R_gear_data.melee_speed, attack_speed_rate, R_gear_data.guard_rate, R_gear_data.tool_tier);
            L_Hand.GetComponent<Hand>().Set_Collider_Default();
            R_Hand.GetComponent<Hand>().Set_Collider(R_Hand.GetComponentInChildren<Collider>());
        }
        else if (moveset_number == -3)
        {
            L_Hand.GetComponent<Hand>().Set_Value_Melee(L_gear_data.melee_damage, attack_damage_max_rate, attack_damage_min_rate, L_gear_data.melee_speed, attack_speed_rate, L_gear_data.guard_rate, L_gear_data.tool_tier);
            L_Hand.GetComponent<Hand>().Set_Value_Default();
            L_Hand.GetComponent<Hand>().Set_Collider(L_Hand.GetComponentInChildren<Collider>());
            R_Hand.GetComponent<Hand>().Set_Collider_Default();
        }
        else if (moveset_number == 4)
        {
            L_Hand.GetComponent<Hand>().Set_Value_Melee(L_gear_data.melee_damage, attack_damage_max_rate, attack_damage_min_rate, L_gear_data.melee_speed, attack_speed_rate, L_gear_data.guard_rate, L_gear_data.tool_tier);
            R_Hand.GetComponent<Hand>().Set_Value_Range(R_gear_data.draw_power, attack_damage_max_rate, R_gear_data.draw_speed, attack_speed_rate, R_gear_data.aim_accuracy, attack_damage_min_rate);
            L_Hand.GetComponent<Hand>().Set_Collider(L_Hand.GetComponentInChildren<Collider>());
            R_Hand.GetComponent<Hand>().Set_Collider(R_Hand.GetComponentInChildren<Collider>());
        }
        else if (moveset_number == -4)
        {
            L_Hand.GetComponent<Hand>().Set_Value_Range(L_gear_data.draw_power, attack_damage_max_rate, L_gear_data.draw_speed, attack_speed_rate, L_gear_data.aim_accuracy, attack_damage_min_rate);
            R_Hand.GetComponent<Hand>().Set_Value_Melee(R_gear_data.melee_damage, attack_damage_max_rate, attack_damage_min_rate, R_gear_data.melee_speed, attack_speed_rate, R_gear_data.guard_rate, R_gear_data.tool_tier);
            L_Hand.GetComponent<Hand>().Set_Collider(L_Hand.GetComponentInChildren<Collider>());
            R_Hand.GetComponent<Hand>().Set_Collider(R_Hand.GetComponentInChildren<Collider>());
        }
    }


































    public GameObject deathEffectPrefab;
    
    protected Animator animator;
    protected Rigidbody rb;
    private Renderer[] entityRenderer;
    private Color[] originalColor;

    protected int damage_base = 0;
    
    public event Action OnDeath; // 죽음 이벤트 선언
    
    protected virtual void Start()
    {    
        health_current = health_max;
        animator = GetComponent<Animator>();
        entityRenderer = GetComponentsInChildren<Renderer>();
        originalColor = new Color[entityRenderer.Length];
        for (int i = 0; i < entityRenderer.Length; i++) {
            originalColor[i] = entityRenderer[i].material.color;
        }
    }
    
    public float Health
    {
        get
        {
            return health_current;
        }
        set
        {
            if (health_current > value)
            {
                StartCoroutine(BlinkRed());
                health_current = value;
            }
            if (health_current <= 0) Die();
        }
    }
    
    protected virtual void Die()
    {
        Debug.Log($"{name} 죽어버림ㅜㅜ");
        OnDeath?.Invoke(); // 죽음 이벤트 호출
    }
    
    private IEnumerator BlinkRed()
    {
        float elapsedTime = 0f;
        bool isRed = false;
    
        while (elapsedTime < 2f)
        {
            for (int i = 0; i < entityRenderer.Length; i++)
            {
                // ObstacleDetector 컴포넌트를 가진 오브젝트는 제외
                if (entityRenderer[i].GetComponent<ObstacleDetector>() != null)
                {
                    continue;
                }
    
                entityRenderer[i].material.color = isRed ? originalColor[i] : Color.red;
            }
            isRed = !isRed;
            elapsedTime += 0.3f; // 깜박이는 속도
            yield return new WaitForSeconds(0.3f);
        }
        for (int i = 0; i < entityRenderer.Length; i++)
        {
            if (entityRenderer[i].GetComponent<ObstacleDetector>() != null)
            {
                continue;
            }
            entityRenderer[i].material.color = originalColor[i];
        }
    }
    
    public virtual IEnumerator OnDie() // virtual 키워드 추가
    {
        animator.SetTrigger("Die");
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        yield return new WaitForSeconds(stateInfo.length);
        Debug.Log("애니메이션 대기 완료");
    
    
        Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
    
    }
    
    public virtual void TakeDamage(float damage)
    {
        Debug.Log($"{name}이(가) {damage}만큼의 데미지를 입었습니다. 현재 체력: {health_current}");
        Health -= damage;
    }
}

public interface IDamageable{
    public void TakeDamage(int damage);
}