using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using System;

namespace Entity_Data
{
    public class Entity : MonoBehaviour
    {
        public string entity_type;
        public string entity_name;

        [SerializeField] protected float VIT, END, STR, DEX, AGI;

        [SerializeField] protected float health_max;
        [SerializeField] protected float health_current;

        [SerializeField] protected float posture_max;
        [SerializeField] protected float posture_current;

        [SerializeField] protected float weight_base;
        [SerializeField] protected float weight_max;
        [SerializeField] protected float weight_current;

        [SerializeField] protected float weight_rate;

        [SerializeField] protected float defense_base;
        [SerializeField] protected float defense_current;

        [SerializeField] protected float attack_damage_base;
        [SerializeField] protected float attack_damage_max_rate;
        [SerializeField] protected float attack_damage_min_rate;
        [SerializeField] protected float attack_speed_rate;
        [SerializeField] protected float guard_rate = 0f;

        public float movement_speed { get; private set; }
        public float jump_height { get; private set; }



        protected Animator animator;
        protected Renderer[] entityRenderer;
        protected Color[] originalColor;

        protected void Awake_Default()
        {
            animator = GetComponent<Animator>();

            Update_Status();
            health_current = health_max;
            posture_current = posture_max;

            entityRenderer = GetComponentsInChildren<Renderer>();
            originalColor = new Color[entityRenderer.Length];
            for (int i = 0; i < entityRenderer.Length; i++) originalColor[i] = entityRenderer[i].material.color;
        }

        public void Update_Status()
        {
            health_max = VIT * 2f;

            weight_base = VIT * 1f + END * 0.5f;
            weight_max = END * 0.75f + STR * 1.5f;
            weight_current = weight_base;

            weight_rate = weight_current / weight_max;

            posture_max = weight_current;

            defense_base = END * 5f;
            defense_current = defense_base;

            attack_damage_base = STR * 0.5f;
            attack_damage_max_rate = 1f + STR * 0.01f;
            attack_damage_min_rate = 0.5f + DEX * 0.005f;
            attack_speed_rate = 1f + AGI * 0.005f;

            movement_speed = 1f + AGI * 0.0025f - weight_rate;
            jump_height = 1f * AGI * 0.0025f - weight_rate;
        }

        public void On_Hit(float damage, Collider attacker)
        {
            Debug.Log($"{entity_name}가 {attacker.gameObject.GetComponentInParent<Entity>().entity_name}에게 맞았음");
            gameObject.GetComponent<Target_Handler>().target = attacker.gameObject.GetComponentInParent<Entity>().gameObject;

            float damage_health = damage;
            float damage_posture = damage;

            float damage_health_result = damage_health * (damage_health / (damage_health + defense_current));
            float damage_posture_result = damage_posture * (damage_posture / (damage_posture + weight_current));


            if (animator.GetBool("Is_Guarding"))
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
                        damage_posture_result *= 1f - guard_rate;
                        posture_current -= damage_posture_result;
                        //float knockback_rate = damage_posture_result / posture_max; //넉백 이벤트 추가하기
                        if (posture_current <= 0) Debug.Log("그로기!");
                    }
                }
            }
            else
            {
                if (health_current > 0)
                {
                    StopCoroutine(BlinkRed());
                    StartCoroutine(BlinkRed());
                    health_current -= damage_health_result;
                    if (health_current <= 0)
                    {
                        Debug.Log("사망!");
                        Destroy(gameObject);
                    }
                }

                if (posture_current > 0)
                {
                    posture_current -= damage_posture_result;
                    //float knockback_rate = damage_posture_result / posture_max; //넉백 이벤트 추가하기
                    if (posture_current <= 0) Debug.Log("그로기!");
                }
            }
        }

        protected IEnumerator BlinkRed()
        {
            for (int i = 0; i < entityRenderer.Length; i++)
            {
                entityRenderer[i].material.color = Color.red;
            }
            yield return new WaitForSeconds(0.3f);

            for (int i = 0; i < entityRenderer.Length; i++)
            {
                entityRenderer[i].material.color = originalColor[i];
            }
        }
    }
}