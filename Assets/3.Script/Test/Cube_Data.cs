using UnityEngine;

[CreateAssetMenu(fileName = "Cube_Data", menuName = "ScriptableObjects/Cube_Data", order = 2)]
public class Cube_Data : ScriptableObject
{
    public string cubeName; // ť�� �̸�
    public Color cubeColor; // ť�� ����
    public GameObject[] cube; // ť������
    public GameObject[] items; // ť�갡 ����ϴ� ������ ����
    public Texture2D[] texture; // ť�� �ؽ�ó
    public int cubewaterGaze; //ť�� ���� ��
    public int cubewaterGaze_Max; //ť�� ���� �ִ� ��
    public int cubemineralGaze; //ť�� ����� ��
    public int cubemineralGaze_Max; //ť�� ����� �ִ� ��
    public int cube_hp_Count; //ť�� ���صǴ� Ÿ�� Ƚ��(hp���� ���� ��� ������ ���صǰ� ������ ����? �ϴ� int�� �����غ�)

}
// ��糪 ��ȣ�ۿ�� ����ũ����Ʈ�� cube �����̱� ������ �ʿ����� ������ �ؼ� �����ý��ϴ�.
// ���� ������ �𸣰ڴ�... 