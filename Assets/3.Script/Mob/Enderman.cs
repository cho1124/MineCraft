using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enderman : Monster, IDamageable
{
    /*

    공격 애니메이션 3개중에 1개 랜덤으로 재생하게 하려고 함 

    => 일단은 애니메이션중 기존것 1개만 재생되도록 하고있음...
       AttackTarget 을 monster 스크립트에서 override 받아 만들었으나 monster 스크립트의 AttackTarget을 진행중ㅜㅜ
       (아직 해결방법을 못찾음)

    */

    private Entity entity;

    protected override void Start()
    {
        base.Start();

        entity = GetComponent<Entity>();
        if (entity != null)
        {
            entity.OnDeath += HandleDeath; // 죽음 이벤트 구독
        }
        Debug.Log("Enderman Start 호출됨");

    }

    private void HandleDeath()
    {
        Debug.Log("HandleDeath 호출됨");
        AudioManager.instance.PlayRandomSFX("Enderman", "Die"); // 죽음 효과음 재생
        StartCoroutine(OnDie());
    }


    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
        if (Health <= 0 ) {
            HandleDeath();
        }
    }

    protected override void AttackTarget() {
        // 플레이어나 동물이 공격 범위 내에 있는지 확인합니다.
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackRange);
        // 몬스터와 목표 위치 사이의 거리가 공격 범위 내에 있는지 확인합니다.
        foreach (var hitCollider in hitColliders) {
            if (hitCollider.CompareTag("Player") || hitCollider.CompareTag("Animals")) {
                SetRandomAttackParameters();
                Debug.Log($"{this.name} 엔더맨 공격 애니메이션.!");
                return; // 공격을 실행했으면 함수를 종료합니다.
            }
        }
        EndChaseAndWander();
    }

    private void SetRandomAttackParameters() {
        int attackType = Random.Range(1, 4); // 1, 2, 3 중 하나를 무작위로 선택
        // 선택된 공격 애니메이션 트리거를 설정
        switch (attackType) {
            case 1:
                animator.SetTrigger("FightType1");
                break;
            case 2:
                animator.SetTrigger("FightType2");
                break;
            case 3:
                animator.SetTrigger("FightType3");
                break;
        }

        Debug.Log($"Selected Attack Type: {attackType}");
    }
}
