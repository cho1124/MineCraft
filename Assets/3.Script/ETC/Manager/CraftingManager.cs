using System.Collections.Generic;
using UnityEngine;

public class CraftingManager : MonoBehaviour
{
    public CraftingSlot[] craftingSlots; // 2x2 �迭�� ũ������ ���Ե�
    public InventoryItem resultItemPrefab; // ��� ������ ������
    public Transform resultSlotTransform; // ��� ���� Transform
    public List<CraftingRecipe> recipes; // ������ ����Ʈ

    private void Start()
    {
        // ũ������ ������ ����� ������ ������ üũ
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
