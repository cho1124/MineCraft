using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData {
    public float health;
    public float[] position;
    public List<Item_Stackable_Data> inventory; // 변경: Item_Stackable_Data 사용
   
    public PlayerData(Player player) {
        health = player.health;
        position = new float[3];
        position[0] = player.transform.position.x;
        position[1] = player.transform.position.y;
        position[2] = player.transform.position.z;
   
        inventory = new List<Item_Stackable_Data>(); // 인벤토리 초기화
        foreach (InventoryItem item in player.inventory.items) {
            inventory.Add(new Item_Stackable_Data(
                item.itemComponent.ItemID,
                item.itemComponent.item_name,
                item.itemComponent.item_model_in_world,
                item.itemComponent.item_model_in_inventory,
                item.itemComponent.stack_max,
                item.itemComponent.stackCurrent
            ));
        }
    }
}

