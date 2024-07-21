using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : Monster, IDamageable 
    {

    public GameObject slimePrefab; // ���� ũ���� ������ ������
    public float deathAnimationDuration = 2f; // �״� �ִϸ��̼��� ���� �ð�
    private int currentHealth = 20; // �������� ���� ü��, �ʿ信 ���� �ʱ�ȭ

    private Vector3 deathPosition; // �������� ���� ���� ��ġ

    protected override void Start() 
    {
        base.Start();
           currentHealth = (int)Health; // �θ� Ŭ������ Health �ʱⰪ�� currentHealth�� ����
    }

    private void OnCollisionEnter(Collision collision) //�浹�� �ٸ� ������Ʈ���� �������� �� 
        { 
        base.OnCollisionEnter(collision); // Monster Ŭ������ OnCollisionEnter �޼��� ȣ��

        // �浹�� ��ü�� �÷��̾� �Ǵ� �������� Ȯ��
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Animals")) {
            IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();
            if (damageable != null) {
                damageable.TakeDamage(10); // ���� ��ü���� 10�� �������� ����
            }
        }
    }

    public void TakeDamage(float damage) //�������� �������� ������ 
        {
        currentHealth -= (int)damage;
        Debug.Log("���ݹ���!!");
        if (currentHealth <= 0) {
            deathPosition = transform.position;
            StartCoroutine(OnDie());
        }
    }

    protected override void Die() {
        Debug.Log("Slime Die ����");
        StartCoroutine(OnDie());
    }

    protected override IEnumerator OnDie() {
        Debug.Log("Slime OnDie ����");
        yield return base.OnDie();
        DieAndSplit();
    }

    private void DieAndSplit() {
        Debug.Log("������ �п� ����");

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
