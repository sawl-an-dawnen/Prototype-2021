using UnityEngine;
using UnityEngine.UI;

public class ItemMapper : MonoBehaviour
{
    public InventoryItem non;
    public InventoryItem news1;
    public InventoryItem news2;
    public InventoryItem news3;
    public InventoryItem helmet;
    public InventoryItem boots;
    public InventoryItem axe;
    public InventoryItem sword;

    public InventoryItem GetInventory(string itemName)
    {
        switch (itemName)
        {
            case "News1":
                return news1;
            case "News2":
                return news2;
            case "News3":
                return news3;
            case "Helmet":
                return helmet;
            case "Boots":
                return boots;
            case "Axe":
                return axe;
            case "Sword":
                return sword;
            default:
                Debug.LogError("Unrecognized item name: " + itemName);
                return non;
        }
    }
}
