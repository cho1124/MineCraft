using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : Monster, IDamageable 
    {

    public GameObject slimePrefab; // 원래 크기의 슬라임 프리팹
    public float deathAnimationDuration = 2f; // 죽는 애니메이션의 지속 시간
    private int currentHealth = 20; // 슬라임의 현재 체력, 필요에 따라 초기화

    private Vector3 deathPosition; // 슬라임이 죽을 때의 위치

    protected override void Start() 
    {
        base.Start();
           currentHealth = (int)Health; // 부모 클래스의 Health 초기값을 currentHealth에 저장
    }

    private void OnCollisionEnter(Collision collision) //충돌시 다른 오브젝트에게 데미지를 줌 
        { 
        base.OnCollisionEnter(collision); // Monster 클래스의 OnCollisionEnter 메서드 호출

        // 충돌한 물체가 플레이어 또는 동물인지 확인
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Animals")) {
            IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();
            if (damageable != null) {
                damageable.TakeDamage(10); // 닿은 물체에게 10의 데미지를 입힘
            }
        }
    }

    public void TakeDamage(float damage) //슬라임이 데미지를 받을때 
        {
        currentHealth -= (int)damage;
        Debug.Log("공격받음!!");
        if (currentHealth <= 0) {
            deathPosition = transform.position;
            StartCoroutine(OnDie());
        }
    }

    protected override void Die() {
        Debug.Log("Slime Die 시작");
        StartCoroutine(OnDie());
    }

    protected override IEnumerator OnDie() {
        Debug.Log("Slime OnDie 시작");
        yield return base.OnDie();
        DieAndSplit();
    }

    private void DieAndSplit() {
        Debug.Log("슬라임 분열 시작");

        Vector3 spawnPosition1 = deathPosition + new Vector3(-0.5f, 0, 0);
        Vector3 spawnPosition2 = deathPosition + new Vector3(0.5f, 0, 0);

        GameObject newSlime1 = Instantiate(slimePrefab, spawnPosition1, Quaternion.identity);
        GameObject newSlime2 = Instantiate(slimePrefab, spawnPosition2, Quaternion.identity);

        newSlime1.transform.localScale = transform.localScale / 2;
        newSlime2.transform.localScale = transform.localScale / 2;

        newSlime1.GetComponent<Slime>().currentHealth = currentHealth / 2;
        newSlime2.GetComponent<Slime>().currentHealth = currentHealth / 2;
    }

    public override void OnAnimationEnd() {
        DieAndSplit();
        Destroy(gameObject);
    }
}
