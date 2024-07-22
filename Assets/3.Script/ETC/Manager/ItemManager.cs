using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ItemManager : MonoBehaviour
{
    public TextAsset itemJsonFile; // 견본


    //이 부분 딕셔너리로 수정 예정

    //TODO
    //이부분을 저장된 json 딕셔너리를 읽어서 매니저에 저장할 것
    //이렇게 정보만을 저장하고 있다가 필요할때마다 동적으로 생성
    //여기서 생길 문제점은 -> 동적으로 instantiate 되는 것이기 때문에 손해되는 부분이 있을지도? -> 아이템은 프리팹이더라도 블록들은 복셀 데이터인가
    //이 뒤에 인벤토리에서 아이템 타입에 따라 카운트가 올라갈 것인지 등등에 대한 처리, 또한 장비 칸 부분 배열에는 드래그 앤 드롭되는 아이템 데이터가 equipment 가능하고 그 위치에 맞는 해당하는 부위인지 검사하고 맞다면 장착
    //장착될 때마다 플레이어의 스테이터스를 설정 -> 이 부분을 DI 혹은 event system 아니면 애당초 인벤토리 안에 있는 아이템 데이터에는 이미 정보가 저장되어 있을 것이기 때문에 컴포넌트만 받아와서 처리해도 될듯
    //사실 그냥 스크립터블이건 프리팹이건 그냥 써도 되긴 하는데, 확장성에서 더 효율적이기 때문에 그냥 써보는것
    //그렇기 때문에 몬스터 데이터에 관한 건 그냥 스크립터블 써도 될듯
    //정민띠의 바이너리 저장 부분에 플레이어 데이터를 저장한다. >>> 당연히 인게임 시간도 저장 되겠죠?
    //플레이어 데이터 >>> 플레이어 스테이터스, 인벤토리
    //인벤토리를 저장하기 위해서는 이 또한 직렬화를 하여 json이던 어떻든 해야될듯
    //원래 있던 씬에서 로드할 때는 같은 파일 혹은 뭐 아무튼 저장된 파일에서 같이 호출
    //플레이어와 다른 엔티티간의 상호작용은 믿습니다.
    //이 모든건 나눠서 처리하는 부분이 수요일까지 끝난다 가정했을때 목 금에 내가 할 것
    //그 전까진 보스와 데이터 세팅 처리를 끝내놔야 할듯
    //사운드 처리에 대한 부분은 이번 프로젝트에서는 다루기 힘들듯, 기본적인 사운드만 처리하고, 디테일적인 부분들은 다음 프로젝트때 하는걸로
    //엔티티 스폰에 대한 부분 -> 스폰 매니저 만들어서 그냥 스크립터블 다 때려박고 만들죠
    //플레이어 데스에 대한 처리 >> 동균띠가 만들 사출 메서드를 활용해서 인벤토리 내의 리스트가 전부 빌 때까지 사출 반복 -> 아니면 이것 또한 따로 메서드를 만들어서 처리해도 될듯
    //정확히는 드랍 메서드라 보는게 좋을 것 같네요
    //사실 가장 중요한 것은 최적화 !!!!!!!!!!!!!!!!!!!!!!!!
    //비동기 로딩씬 만들 것인가? -> 데이터 로드되는 상태보고 판단
    //에픽 보스
    //지렁이 같이 생긴 무언가 : 패턴 2개, 거미 : 진행중, 데스웜 : 일단 구상중
    //Damage받는 부분 인터페이스로 처리해도 되긴 하는데 어차피 entity 자식 클래스들이면 그냥 상속 받아도 될듯



    void Start()
    {
        LoadItemsFromJson(itemJsonFile.text);
        
        

    }

    void LoadItemsFromJson(string json)
    {
        // 역직렬화
        ItemDataList itemDataList = JsonUtility.FromJson<ItemDataList>(json);

        // 스택 가능 아이템 로드
        foreach (var item in itemDataList.stackableItems)
        {
            Item_Dictionary.Add(item.item_ID, new Item_Stackable_Data(
                item.item_ID,
                item.item_name,
                item.item_model_in_world,
                item.item_model_in_inventory,
                item.stack_max,
                item.stack_current
            ));
        }

        // 소비 아이템 로드
        foreach (var item in itemDataList.consumableItems)
        {
            Item_Dictionary.Add(item.item_ID, new Item_Consumable_Data(
                item.item_ID,
                item.item_name,
                item.item_model_in_world,
                item.item_model_in_inventory,
                item.stack_max,
                item.stack_current,
                item.hunger_amount,
                item.thirst_amount,
                item.fatigue_amount,
                item.freshment_max,
                item.freshment_current
            ));
        }

        // 설치 가능 아이템 로드
        foreach (var item in itemDataList.placeableItems)
        {
            Item_Dictionary.Add(item.item_ID, new Item_Placeable_Data(
                item.item_ID,
                item.item_name,
                item.item_model_in_world,
                item.item_model_in_inventory,
                item.stack_max,
                item.stack_current,
                item.require_tool_type,
                item.require_tool_tier,
                item.durability_max,
                item.durability_current,
                item.item_model_in_place
            ));
        }

        // 장비 아이템 로드
        foreach (var item in itemDataList.equipmentItems)
        {
            Item_Dictionary.Add(item.item_ID, new Item_Gear_Data(
                item.item_ID,
                item.item_name,
                item.item_model_in_world,
                item.item_model_in_inventory,
                item.equipment_type,
                item.weight,
                item.durability_max,
                item.durability_current,
                item.item_model_in_equip,
                item.melee_damage,
                item.melee_speed,
                item.guard_rate,
                item.tool_tier
            ));
        }
    }
}

[System.Serializable]
public class ItemDataList
{
    public List<ItemJsonStackable> stackableItems;
    public List<ItemJsonConsumable> consumableItems;
    public List<ItemJsonPlaceable> placeableItems;
    public List<ItemJsonEquipment> equipmentItems;
}

[System.Serializable]
public class ItemJsonStackable
{
    public int item_ID;
    public string item_name;
    public string item_model_in_world;
    public string item_model_in_inventory;
    public int stack_max;
    public int stack_current;
}

[System.Serializable]
public class ItemJsonConsumable : ItemJsonStackable
{
    public float hunger_amount;
    public float thirst_amount;
    public float fatigue_amount;
    public float freshment_max;
    public float freshment_current;
}

[System.Serializable]
public class ItemJsonPlaceable : ItemJsonStackable
{
    public Equipment_Type require_tool_type;
    public int require_tool_tier;
    public float durability_max;
    public float durability_current;
    public string item_model_in_place;
}

[System.Serializable]
public class ItemJsonEquipment
{
    public int item_ID;
    public string item_name;
    public string item_model_in_world;
    public string item_model_in_inventory;
    public Equipment_Type equipment_type;
    public float weight;
    public float durability_max;
    public float durability_current;
    public string item_model_in_equip;
    public float melee_damage;
    public float melee_speed;
    public float guard_rate;
    public int tool_tier;
}

