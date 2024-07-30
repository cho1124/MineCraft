using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecipeManager : MonoBehaviour
{
    public TextAsset recipeJsonFile;
    public List<CraftingRecipe> recipes;

    private void Awake()
    {
        if (recipeJsonFile != null)
        {
            string jsonString = recipeJsonFile.text;
            CraftingRecipes loadedRecipes = JsonUtility.FromJson<CraftingRecipes>(jsonString);
            recipes = loadedRecipes.recipes;
        }
        else
        {
            Debug.LogError("Recipe JSON file not found.");
        }
    }

    public CraftingRecipe FindMatchingRecipe(List<ItemComponent> ingredients)
    {
        foreach (var recipe in recipes)
        {
            bool match = true;
            foreach (var ingredient in recipe.ingredients)
            {
                bool found = false;
                foreach (var item in ingredients)
                {
                    if (item.ItemID == ingredient.item_id && item.StackCurrent >= ingredient.item_quantity)
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    match = false;
                    break;
                }
            }
            if (match)
            {
                return recipe;
            }
        }
        return null;
    }
}

[System.Serializable]
public class CraftingIngredient
{
    public int itemID;
    public int quantity;
}

[System.Serializable]
public class CraftingRecipe
{
    public InventoryItem resultItem; // 결과 아이템
    public InventoryItem[] ingredients; // 재료 아이템 배열

    public bool Matches(CraftingSlot[] slots)
    {
        for (int i = 0; i < ingredients.Length; i++)
        {
            if (slots[i].myItem == null || slots[i].myItem.itemComponent.ItemID != ingredients[i].itemComponent.ItemID)
            {
                return false;
            }
        }
        return true;
    }
}

[System.Serializable]
public class CraftingRecipes
{
    public List<CraftingRecipe> recipes;
}

