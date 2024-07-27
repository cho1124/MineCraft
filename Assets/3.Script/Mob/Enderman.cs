using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enderman : Monster, IDamageable
{
    /*
    ���� ������ ü���� 0���Ϸ� �پ�� �ڵ鵥�� �޼��� �̺�Ʈ��
    ȣ����� �ʰ�����, ü�� -���� ��� ��
    
    => �̺�Ʈ ���� Entity Ŭ������ TakeDamage �޼��带 �������̵��Ͽ� ü���� 0 ������ �� ���� Die �޼��带 ȣ���ϵ���
    => �׷��� ������ ���̴� ����...
    */

    private Entity entity;

    protected override void Start()
    {
        base.Start();

        entity = GetComponent<Entity>();
        if (entity != null)
        {
            entity.OnDeath += HandleDeath; // ���� �̺�Ʈ ����
        }
        Debug.Log("Enderman Start ȣ���");

    }

 //   private void Update()
 //   {
 //       Damaged_ender();
 //   }


    private void HandleDeath()
    {
        Debug.Log("HandleDeath ȣ���");
        StartCoroutine(OnDie());
    }

    //public override IEnumerator OnDie()
    //{
    //    Debug.Log($"{name}�������.");
    //    // ��� �ִϸ��̼��� Ʈ�����մϴ�.
    //    animator.SetTrigger("Die");
    //
    //    // ��� �ִϸ��̼��� ���̸� ��� ����մϴ�.
    //    AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
    //    yield return new WaitForSeconds(stateInfo.length);
    //
    //    // ��� ȿ���� �����մϴ�.
    //    Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
    //
    //    // ������ ������Ʈ�� �ı��մϴ�.
    //    Destroy(gameObject);
    //}

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
        if (Health <= 0 ) {
            HandleDeath();
        }
    }

    // private void Damaged_ender()
    // {
    //     if(Input.GetKeyDown(KeyCode.K))
    //     {
    //         TakeDamage(50);
    //     }
    // }

    protected override void AttackTarget() {
        if (Vector3.Distance(transform.position, targetPosition) < attackRange) {
            Debug.Log($"�����ǽ�ũ��Ʈ {name} �� ������.!");
            SetRandomAttackParameters();
            animator.SetTrigger("Attack");
        }
        EndChaseAndWander();
    }

    private void SetRandomAttackParameters() {
        bool attackType1 = Random.value > 0.5f;
        bool attackType2 = Random.value > 0.5f;

        if (!attackType1 && !attackType2) {
            attackType1 = true; // �� �� false�� �� AttackType1�� true�� ����
        }

        Debug.Log($"AttackType1: {attackType1}, AttackType2: {attackType2}");

        animator.SetBool("AttackType1", attackType1);
        animator.SetBool("AttackType2", attackType2);
    }

}
