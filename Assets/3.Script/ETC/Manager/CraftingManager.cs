using System.Collections.Generic;
using UnityEngine;

public class CraftingManager : MonoBehaviour
{
    public CraftingSlot[] craftingSlots; // 2x2 배열의 크래프팅 슬롯들
    public InventoryItem resultItemPrefab; // 결과 아이템 프리팹
    public Transform resultSlotTransform; // 결과 슬롯 Transform
    public List<CraftingRecipe> recipes; // 레시피 리스트

    private void Start()
    {
        // 크래프팅 슬롯이 변경될 때마다 레시피 체크
        foreach (var slot in craftingSlots)
        {
            slot.OnItemChanged += CheckRecipe;
        }
    }

    private void CheckRecipe()
    {
        foreach (var recipe in recipes)
        {
            if (recipe.Matches(craftingSlots))
            {
                SetResultItem(recipe.resultItem);
                return;
            }
        }
        ClearResultItem();
    }

    private void SetResultItem(InventoryItem item)
    {
        var resultItem = Instantiate(resultItemPrefab, resultSlotTransform);
        resultItem.Initialize(item.itemComponent, null);
    }

    private void ClearResultItem()
    {
        foreach (Transform child in resultSlotTransform)
        {
            Destroy(child.gameObject);
        }
    }

    public void Craft()
    {
        if (resultSlotTransform.childCount > 0)
        {
            foreach (var slot in craftingSlots)
            {
                slot.ClearSlot();
            }
            ClearResultItem();
        }
    }
}
