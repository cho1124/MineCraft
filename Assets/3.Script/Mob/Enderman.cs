using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enderman : Monster, IDamageable
{
    /*

    애니메이터 파라미터 설정은 잘 되어있는데 
    엔더맨 스크립트의 attacktarget 메서드에 진입하지 못하고 있는것 같다. 

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
