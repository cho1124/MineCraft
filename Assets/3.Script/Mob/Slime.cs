using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : Monster, IDamageable 
    {

    public GameObject slimePrefab; // ���� ũ���� ������ ������
    public float deathAnimationDuration = 2f; // �״� �ִϸ��̼��� ���� �ð�
    private int currentHealth = 20; // �������� ���� ü��, �ʿ信 ���� �ʱ�ȭ
    public RuntimeAnimatorController slimeAnimatorController; // ���� ������ �ִϸ����� ��Ʈ�ѷ�
    public RuntimeAnimatorController splitSlimeAnimatorController; // �п��� ������ �ִϸ����� ��Ʈ�ѷ�


    private Vector3 deathPosition; // �������� ���� ���� ��ġ

    private Entity entity;

    protected override void Start() 
    {
        entity = GetComponent<Entity>();
        if (entity != null)
        {
            entity.OnDeath += HandleDeath; // ���� �̺�Ʈ ����
        }

        base.Start();
           currentHealth = (int)entity.Health; // �θ� Ŭ������ Health �ʱⰪ�� currentHealth�� ����
                                               // DieAndSplit();
    }


   // private void Update()
   // {
   //     if(Input.GetKeyDown(KeyCode.Q))
   //     {
   //         DieAndSplit();
   //     }
   // }


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

    private void HandleDeath() 
    {
        deathPosition = transform.position; // ���� ��ġ ����
        Debug.Log("������ �п� ����");

            Vector3 spawnPosition1 = deathPosition + new Vector3(-0.5f, 0, 0);
        Vector3 spawnPosition2 = deathPosition + new Vector3(0.5f, 0, 0);

        GameObject newSlime1 = Instantiate(slimePrefab, spawnPosition1, Quaternion.identity);
        GameObject newSlime2 = Instantiate(slimePrefab, spawnPosition2, Quaternion.identity);

            // ������ �������� �Ӽ��� �ʱ�ȭ
            InitializeNewSlime(newSlime1);
            InitializeNewSlime(newSlime2);

        // OnDie �ڷ�ƾ ȣ��
        StartCoroutine(entity.OnDie());
    }

    private void InitializeNewSlime(GameObject newSlime) //������ �ִϸ��̼� Ŭ������ scale �����ϰ� �־ �� �۾����°ſ��� ���� �ִϸ��̼���Ʈ�ѷ� ���� 
                                                         // ������ �������� ��Ʈ�ѷ��� scale �۾��� ��Ʈ�ѷ�(slime_s)�� �ٲ�
    {
        Debug.Log("������ �ʱ�ȭ!");

        // �������� ũ�⸦ ������ ����
        newSlime.transform.localScale = transform.localScale * 0.5f;

        // �������� Animator Controller�� slime���� slime 1�� ����
        Animator animator = newSlime.GetComponent<Animator>();
        if (animator != null)
        {
            animator.runtimeAnimatorController = splitSlimeAnimatorController;
        }

        // �������� Health ����
        Slime slimeComponent = newSlime.GetComponent<Slime>();
        if (slimeComponent != null)
        {
            slimeComponent.currentHealth = currentHealth / 2;
            slimeComponent.Health = Health / 2;
        }

        // �������� material�� �������� material�� �����ϰ� ����
        Renderer[] renderers = newSlime.GetComponentsInChildren<Renderer>();
        Renderer prefabRenderer = slimePrefab.GetComponent<Renderer>();
        if (prefabRenderer != null)
        {
            foreach (var renderer in renderers)
            {
                renderer.material = prefabRenderer.material;
            }
        }
    }

}
