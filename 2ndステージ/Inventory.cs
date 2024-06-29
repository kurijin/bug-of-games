using System.Collections.Generic;
using UnityEngine;

//Playerのインベントリー
public class Inventory : MonoBehaviour
{
    //アイテムを格納しておくもの
    public List<Item> items = new List<Item>();

    //Playerのスクリプトから呼び出されるものアイテムをlistに格納する
    public void AddItem(Item item)
    {
        items.Add(item);
    }

    //インベントリ内にアイテムがあるかどうかを確認するもの
    public bool HasItem(string itemName)
    {
        foreach (Item item in items)
        {
            if (item.itemName == itemName)
            {
                return true;
            }
        }
        return false;
    }
}
