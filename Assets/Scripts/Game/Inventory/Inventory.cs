using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    private static List<Item> items = new List<Item>();

    public static void AddItem(Item item)
    {
        item.gameObject.GetComponent<Rigidbody>().isKinematic = true;
        items.Add(item);
    }

    public static void DeleteItem(Item item)
    {
        items.Remove(item);
        item.inHand = false;
        item.gameObject.GetComponent<Rigidbody>().isKinematic = false;
    }

    public static Item GetItem(int id)
    {
        if(id <= items.Count)
            return items[id-1];

        return null;
    }

    public static int Count()
    {
        return items.Count;
    }

    public static void Reset()
    {
        items.Clear();
    }
}
