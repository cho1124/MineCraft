using UnityEngine;

[CreateAssetMenu(fileName = "BiomeData", menuName = "ScriptableObjects/Biome", order = 1)]
public class Biome : ScriptableObject
{
    public string biomeName; // �������� �̸�
    public Color biomeColor; // �������� ����
    public GameObject[] creatures; // �������迡 ���� ����ü��
    public Texture2D texture; // ���������� �ؽ�ó
    // �ٸ� �������� ���� �Ӽ��� ���⿡ �߰�
}

//
