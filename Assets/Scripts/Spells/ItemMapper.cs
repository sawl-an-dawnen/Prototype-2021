using UnityEngine;
using UnityEngine.UI;

public class ItemMapper : MonoBehaviour
{
    public bool InventoryType(string itemName)
    {
        switch (itemName)
        {
            case "News1":
                return GameManager.Instance.inventoryManager.news1.isEquip;
            case "News2":
                return GameManager.Instance.inventoryManager.news2.isEquip;
            case "News3":
                return GameManager.Instance.inventoryManager.news3.isEquip;
            //case "Helmet":
            //    return GameManager.Instance.inventoryManager.helmet.isEquip;
            default:
                Debug.LogError("Unrecognized item name: " + itemName);
                return false;
        }
    }

    public Sprite GetItemIcon(string itemName)
    {
        switch (itemName)
        {
            case "News1":
                return GameManager.Instance.inventoryManager.news1.itemIcon;
            case "News2":
                return GameManager.Instance.inventoryManager.news2.itemIcon;
            case "News3":
                return GameManager.Instance.inventoryManager.news3.itemIcon;
            default:
                Debug.LogError("Unrecognized item name: " + itemName);
                return null;
        }
    }

    public Button GetItemButtonPrefab(string itemName)
    {
        switch (itemName)
        {
            case "News1":
                return GameManager.Instance.inventoryManager.news1.itemButtonPrefab;
            case "News2":
                return GameManager.Instance.inventoryManager.news2.itemButtonPrefab;
            case "News3":
                return GameManager.Instance.inventoryManager.news3.itemButtonPrefab;
            default:
                Debug.LogError("Unrecognized item name: " + itemName);
                return null;
        }
    }

    public string GetItemInfo(string itemName)
    {
        switch (itemName)
        {
            case "News1":
                return GameManager.Instance.inventoryManager.news1.itemInfo;
            case "News2":
                return GameManager.Instance.inventoryManager.news2.itemInfo;
            case "News3":
                return GameManager.Instance.inventoryManager.news3.itemInfo;
            default:
                Debug.LogError("Unrecognized item name: " + itemName);
                return null;
        }
    }
}
