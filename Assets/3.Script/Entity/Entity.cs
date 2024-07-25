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

    //�ð��� ���� ������ �����ϴ� �޼ҵ� �߰��� ��
}

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

    public void Set_Value_Default()
    {
        //�Ǽ����� �ʱ�ȭ
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
        //��ųʸ� �ʱ�ȭ
    }

    private void OnTriggerStay(Collider victim)
    {
        if (victim.CompareTag("Entity"))
        {
            victim.gameObject.GetComponent<Entity>().On_Hit(UnityEngine.Random.Range(damage_min, damage_max), collider);
            //������ ����� ��ųʸ��� �߰��ϰ� ��ųʸ��� �ִ� ��ƼƼ�� �ٽ� ó�� ����
        }
    }
}

public class Entity : MonoBehaviour
{
    // �������� ������ �� 0.5�� ���� ���������� ����
    // ������ ��ƼŬ�� ����Ʈ
    // awake���� entityeditor �� �ִ� ���� ���� =>������ ����� ü�� ������ ������
    // ������ ���� ���ݰ� ������ ó�� �ϴ� �κ� �ش� Ŭ������ ����

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
    private float guard_rate;

    private int moveset_number;

    private float movement_speed;
    private float jump_height;

    public float Health { get { return health_current; } }
    public float Posture { get { return posture_current; } }
    public float Defense { get { return defense_current; } }
    public float Weight { get { return weight_current; } }

    [SerializeField] private Animator entity_animator;
    [SerializeField] private GameObject L_Hand;
    [SerializeField] private GameObject R_Hand;

    private ItemComponent L_held_data, R_held_data, helmet_data, chestplate_data, leggings_data, boots_data;

    public Entity()
    {
        VIT = 1;
        END = 1;
        STR = 1;
        DEX = 1;
        AGI = 1;

        Update_Status();
    }
    public Entity(int VIT, int END, int STR, int DEX, int AGI)
    {
        this.VIT = VIT;
        this.END = END;
        this.STR = STR;
        this.DEX = DEX;
        this.AGI = AGI;

        Update_Status();
    }

    public void Update_Status()
    {

        //������ ������ ������ �б�

        health_max = 100f * (1f + VIT * 0.01f);

        weight_base = VIT * 1f + END * 0.5f;
        weight_max = END * 1f + STR * 0.5f;
        weight_current = weight_base + helmet_data.weight + chestplate_data.weight + leggings_data.weight + boots_data.weight + L_held_data.weight + R_held_data.weight;

        weight_rate = 1f - weight_current / weight_max;

        posture_max = weight_current;

        defense_base = END * 5f;
        defense_current = defense_base + helmet_data.armorDefense + chestplate_data.armorDefense + leggings_data.armorDefense + boots_data.armorDefense;

        attack_damage_base = STR * 0.5f;
        attack_damage_max_rate = 1f + STR * 0.01f;
        attack_damage_min_rate = 0.5f + DEX * 0.005f;
        attack_speed_rate = 1f + AGI * 0.005f;

        movement_speed = 1f + AGI * 0.0025f - weight_rate;
        jump_height = 1f * AGI * 0.0025f - weight_rate;



        //���⼭ ���� ���տ� ���� ����� ����
        if (moveset_number == 1 || moveset_number == -1 || moveset_number == 2 || moveset_number == -2)
        {
            L_Hand.GetComponent<Hand>().Set_Value_Melee(L_held_data.meleeDamage, attack_damage_max_rate, attack_damage_min_rate, L_held_data.meleeSpeed, attack_speed_rate, L_held_data.guardRate, L_held_data.toolTier);
            R_Hand.GetComponent<Hand>().Set_Value_Melee(R_held_data.meleeDamage, attack_damage_max_rate, attack_damage_min_rate, R_held_data.meleeSpeed, attack_speed_rate, R_held_data.guardRate, R_held_data.toolTier);
            L_Hand.GetComponent<Hand>().Set_Collider(L_Hand.GetComponentInChildren<Collider>());
            R_Hand.GetComponent<Hand>().Set_Collider(R_Hand.GetComponentInChildren<Collider>());

            switch (moveset_number)
            {
                case 1:
                    guard_rate = L_Hand.GetComponent<Hand>().guard_rate;
                    break;
                case -1:
                    guard_rate = R_Hand.GetComponent<Hand>().guard_rate;
                    break;
                case 2: case -2:
                    guard_rate = (L_Hand.GetComponent<Hand>().guard_rate + R_Hand.GetComponent<Hand>().guard_rate) / 2f;
                    break;
            }
        }
        else if (moveset_number == 3)
        {
            L_Hand.GetComponent<Hand>().Set_Value_Default();
            R_Hand.GetComponent<Hand>().Set_Value_Melee(R_held_data.meleeDamage, attack_damage_max_rate, attack_damage_min_rate, R_held_data.meleeSpeed, attack_speed_rate, R_held_data.guardRate, R_held_data.toolTier);
            L_Hand.GetComponent<Hand>().Set_Collider_Default();
            R_Hand.GetComponent<Hand>().Set_Collider(R_Hand.GetComponentInChildren<Collider>());

            guard_rate = R_Hand.GetComponent<Hand>().guard_rate;
        }
        else if (moveset_number == -3)
        {
            L_Hand.GetComponent<Hand>().Set_Value_Melee(L_held_data.meleeDamage, attack_damage_max_rate, attack_damage_min_rate, L_held_data.meleeSpeed, attack_speed_rate, L_held_data.guardRate, L_held_data.toolTier);
            L_Hand.GetComponent<Hand>().Set_Value_Default();
            L_Hand.GetComponent<Hand>().Set_Collider(L_Hand.GetComponentInChildren<Collider>());
            R_Hand.GetComponent<Hand>().Set_Collider_Default();

            guard_rate = L_Hand.GetComponent<Hand>().guard_rate;
        }
        else if (moveset_number == 4)
        {
            L_Hand.GetComponent<Hand>().Set_Value_Melee(L_held_data.meleeDamage, attack_damage_max_rate, attack_damage_min_rate, L_held_data.meleeSpeed, attack_speed_rate, L_held_data.guardRate, L_held_data.toolTier);
            R_Hand.GetComponent<Hand>().Set_Value_Range(R_held_data.drawPower, attack_damage_max_rate, R_held_data.drawSpeed, attack_speed_rate, R_held_data.aimAccuracy, attack_damage_min_rate);
            L_Hand.GetComponent<Hand>().Set_Collider(L_Hand.GetComponentInChildren<Collider>());
            R_Hand.GetComponent<Hand>().Set_Collider(R_Hand.GetComponentInChildren<Collider>());

            guard_rate = 0f;
        }
        else if (moveset_number == -4)
        {
            L_Hand.GetComponent<Hand>().Set_Value_Range(L_held_data.drawPower, attack_damage_max_rate, L_held_data.drawSpeed, attack_speed_rate, L_held_data.aimAccuracy, attack_damage_min_rate);
            R_Hand.GetComponent<Hand>().Set_Value_Melee(R_held_data.meleeDamage, attack_damage_max_rate, attack_damage_min_rate, R_held_data.meleeSpeed, attack_speed_rate, R_held_data.guardRate, R_held_data.toolTier);
            L_Hand.GetComponent<Hand>().Set_Collider(L_Hand.GetComponentInChildren<Collider>());
            R_Hand.GetComponent<Hand>().Set_Collider(R_Hand.GetComponentInChildren<Collider>());

            guard_rate = 0f;
        }

        entity_animator.SetFloat("L_Attack_Speed", L_Hand.GetComponent<Hand>().attack_speed);
        entity_animator.SetFloat("R_Attack_Speed", R_Hand.GetComponent<Hand>().attack_speed);
        entity_animator.SetFloat("Movement_Speed", movement_speed);
    }

    public void On_Hit(float damage, Collider attacker)
    {
        float damage_health = damage;
        float damage_posture = damage;

        float damage_health_result = damage_health * (damage_health / (damage_health + defense_current));
        float damage_posture_result = damage_posture * (damage_posture / (damage_posture + weight_current));


        if (entity_animator.GetBool("Is_Guarding"))
        {
            Vector3 attack_direction = (attacker.transform.position - transform.position).normalized;
            Vector3 victim_direction = transform.forward;
            attack_direction.y = 0f;
            victim_direction.y = 0f;

            float angle = Vector3.Angle(victim_direction, attack_direction);

            if (-45f <= angle && angle <= 45f)
            {
                if (posture_current > 0)
                {
                    // �ǰ����� ���� ������ posture�� ������ŭ �˹�
                    damage_posture_result *= 1f - guard_rate;
                    posture_current -= damage_posture_result;
                    if (posture_current <= 0) Debug.Log("�׷α�!");
                }
            }
        }
        else
        {
            if (health_current > 0)
            {
                health_current -= damage_health_result;
                if (health_current <= 0) OnDie();
            }

            if (posture_current > 0)
            {
                // �ǰ����� ���� ������ posture�� ������ŭ �˹�
                posture_current -= damage_posture_result;
                if (posture_current <= 0) Debug.Log("�׷α�!");
            }
        }
    }




















    public GameObject deathEffectPrefab;
    
    protected Animator animator;
    protected Rigidbody rb;
    private Renderer[] entityRenderer;
    private Color[] originalColor;

    protected int damage_base = 0;
    
    public event Action OnDeath; // ���� �̺�Ʈ ����
    
    protected virtual void Start()
    {
        health_current = health_max;
        posture_current = posture_max;
        animator = GetComponent<Animator>();
        entityRenderer = GetComponentsInChildren<Renderer>();
        originalColor = new Color[entityRenderer.Length];
        for (int i = 0; i < entityRenderer.Length; i++) {
            originalColor[i] = entityRenderer[i].material.color;
        }
    }
    
    protected virtual void Die()
    {
        Debug.Log($"{name} �׾�����̤�");
        OnDeath?.Invoke(); // ���� �̺�Ʈ ȣ��
    }
    
    private IEnumerator BlinkRed()
    {
        float elapsedTime = 0f;
        bool isRed = false;
    
        while (elapsedTime < 2f)
        {
            for (int i = 0; i < entityRenderer.Length; i++)
            {
                // ObstacleDetector ������Ʈ�� ���� ������Ʈ�� ����
                if (entityRenderer[i].GetComponent<ObstacleDetector>() != null)
                {
                    continue;
                }
    
                entityRenderer[i].material.color = isRed ? originalColor[i] : Color.red;
            }
            isRed = !isRed;
            elapsedTime += 0.3f; // �����̴� �ӵ�
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
    
    public virtual IEnumerator OnDie() // virtual Ű���� �߰�
    {
        animator.SetTrigger("Die");
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        yield return new WaitForSeconds(stateInfo.length);
        Debug.Log("�ִϸ��̼� ��� �Ϸ�");
    
    
        Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
    
    }
    
    public virtual void TakeDamage(float damage)
    {
        Debug.Log($"{name}��(��) {damage}��ŭ�� �������� �Ծ����ϴ�. ���� ü��: {health_current}");
    }
}

public interface IDamageable{
    public void TakeDamage(int damage);
}