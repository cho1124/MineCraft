using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    // Start is called before the first frame update
    private float maxHealth;
    private float health;
    private float posture;
    private float defence;
    private float weight;
    private float speed;

    public float Health
    {
        get
        {
            return health;
        }
        set
        {
            health = value;

            if(health <= 0)
            {
                //Ondie �޼��� �����
            }
        }
    }

    public void OnDamage(float damage)
    {
        Health -= damage;
    }


//�������� Ŀ���� �ؼ� ���� �� ��



}
