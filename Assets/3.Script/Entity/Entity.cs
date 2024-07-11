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
                //Ondie 메서드 만들기
            }
        }
    }

    public void OnDamage(float damage)
    {
        Health -= damage;
    }


//나머지는 커스텀 해서 구현 할 것



}
