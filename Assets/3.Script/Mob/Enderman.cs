using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enderman : Monster, IDamageable
{
    /*

    �ִϸ����� �Ķ���� ������ �� �Ǿ��ִµ� 
    ������ ��ũ��Ʈ�� attacktarget �޼��忡 �������� ���ϰ� �ִ°� ����. 

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

    private void HandleDeath()
    {
        Debug.Log("HandleDeath ȣ���");
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
        // �÷��̾ ������ ���� ���� ���� �ִ��� Ȯ���մϴ�.
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackRange);
        // ���Ϳ� ��ǥ ��ġ ������ �Ÿ��� ���� ���� ���� �ִ��� Ȯ���մϴ�.
        foreach (var hitCollider in hitColliders) {
            if (hitCollider.CompareTag("Player") || hitCollider.CompareTag("Animals")) {
                SetRandomAttackParameters();
                Debug.Log($"{this.name} ������ ���� �ִϸ��̼�.!");
                return; // ������ ���������� �Լ��� �����մϴ�.
            }
        }
        EndChaseAndWander();
    }

    private void SetRandomAttackParameters() {
        int attackType = Random.Range(1, 4); // 1, 2, 3 �� �ϳ��� �������� ����
        // ���õ� ���� �ִϸ��̼� Ʈ���Ÿ� ����
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
