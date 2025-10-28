using UnityEngine;
using System.Collections.Generic;

public class Inventory : MonoBehaviour
{
    private HashSet<string> items = new HashSet<string>();

    public void AddItem(string itemName)
    {
        items.Add(itemName);
    }

    public bool HasItem(string itemName)
    {
        return items.Contains(itemName);
    }
    public void RemoveItem(string itemName)
    {
        if (items.Contains(itemName))
            items.Remove(itemName);
    }

}
