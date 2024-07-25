using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enderman : Monster
{
    /*
    현재 엔더맨 체력이 0이하로 줄어들어도 핸들데스 메서드 이벤트에
    호출되지 않고있음, 체력 -까지 계속 뜸
    
    => 이벤트 말고 Entity 클래스의 TakeDamage 메서드를 오버라이드하여 체력이 0 이하일 때 직접 Die 메서드를 호출하도록
    => 그러나 여전히 차이는 없음...
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

    private void Update()
    {
        Damaged_ender();
    }


    private void HandleDeath()
    {
        Debug.Log("HandleDeath 호출됨");
        StartCoroutine(OnDie());
    }

    //public override IEnumerator OnDie()
    //{
    //    Debug.Log($"{name}사망시작.");
    //    // 사망 애니메이션을 트리거합니다.
    //    animator.SetTrigger("Die");
    //
    //    // 사망 애니메이션의 길이를 얻어 대기합니다.
    //    AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
    //    yield return new WaitForSeconds(stateInfo.length);
    //
    //    // 사망 효과를 생성합니다.
    //    Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
    //
    //    // 엔더맨 오브젝트를 파괴합니다.
    //    Destroy(gameObject);
    //}

    //public override void TakeDamage(int damage)
    //{
    //    base.TakeDamage(damage);
    //    if (Health <= 0 ) {
    //        HandleDeath();
    //    }
    //}

    private void Damaged_ender()
    {
        if(Input.GetKeyDown(KeyCode.K))
        {
            TakeDamage(50);
        }
    }

}
