using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    public static GameManager instance = null;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        //ó���� ������ �� �ٸ� �Ŵ��� Ŭ�������� ��ȯ
    }

    public void StartGame()
    {
        
    }


    //������ ������ ��, ��� �Ŵ��� Ŭ������ ���� �̱����� ����ұ�?
    //����, ��, ���� �Ŵ����� �̱����� ���°� �°�����
    //������ �Ŵ����� ���� ���� ������ ������ ���� �θ���
    //������ �Ŵ����� ���ؼ� �����͸� �θ��� �� ������ ���̺�� ������ �ε��ϴ� ���̺� �ε� �Ŵ����� ȣ���ϴ� ���
    //�� ������ �������� ��ƼƼ���� �����ϴ� ��ƼƼ �Ŵ����� ȣ���ϴ� ��
    //

}
