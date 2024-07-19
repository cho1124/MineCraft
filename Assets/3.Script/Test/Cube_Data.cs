using UnityEngine;

[CreateAssetMenu(fileName = "Cube_Data", menuName = "ScriptableObjects/Cube_Data", order = 2)]
public class Cube_Data : ScriptableObject
{
    public string cubeName; // 큐브 이름
    public Color cubeColor; // 큐브 색상
    public GameObject[] cube; // 큐브종류
    public GameObject[] items; // 큐브가 드롭하는 아이템 종류
    public Texture2D[] texture; // 큐브 텍스처
    public int cubewaterGaze; //큐브 수분 양
    public int cubewaterGaze_Max; //큐브 수분 최대 양
    public int cubemineralGaze; //큐브 영양분 양
    public int cubemineralGaze_Max; //큐브 영양분 최대 양
    public int cube_hp_Count; //큐브 분해되는 타격 횟수(hp같은 개념 몇번 때려야 분해되고 아이템 될지? 일단 int로 생각해봄)

}
// 농사나 상호작용시 마인크래프트는 cube 단위이기 때문에 필요하지 않을까 해서 만들어봤습니다.
// 쓸모가 있을지 모르겠는... 