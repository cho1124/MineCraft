using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Entity
{
    //�÷��̾�� �ܼ��� ��ƼƼ�� �Ӽ��� �޴� �� �Ӹ��� �ƴ� �� �� �پ��� �Ӽ��� �����ؾ� �ؿ�. -> �÷��̾�� ������ ������, �񸶸�, ����ġ ���� ���⼭ �����ϸ� ���� �������?
    private float Exp;




    
    public float GetEXP()
    {
        return Exp++;
    }

    public void CheckEquipment()
    {
       
    }



}
