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
            case "Helmet":
                return GameManager.Instance.inventoryManager.helmet.isEquip;
            default:
                Debug.LogError("Unrecognized item name: " + itemName);
                return false;
        }
    }

    public Sprite GetIcon(string itemName)
    {
        switch (itemName)
        {
            case "News1":
                return GameManager.Instance.inventoryManager.news1.itemIcon;
            case "News2":
                return GameManager.Instance.inventoryManager.news2.itemIcon;
            case "News3":
                return GameManager.Instance.inventoryManager.news3.itemIcon;
            case "Helmet":
                return GameManager.Instance.inventoryManager.helmet.equipIcon;
            default:
                Debug.LogError("Unrecognized item name: " + itemName);
                return null;
        }
    }

    public Button GetButtonPrefab(string itemName)
    {
        switch (itemName)
        {
            case "News1":
                return GameManager.Instance.inventoryManager.news1.itemButtonPrefab;
            case "News2":
                return GameManager.Instance.inventoryManager.news2.itemButtonPrefab;
            case "News3":
                return GameManager.Instance.inventoryManager.news3.itemButtonPrefab;
            case "Helmet":
                return GameManager.Instance.inventoryManager.helmet.equipButtonPrefab;
            default:
                Debug.LogError("Unrecognized item name: " + itemName);
                return null;
        }
    }


    // only for items
    public string GetInfo(string itemName)
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


    // only for Equipment
    public EquipmentType GetEquipType(string itemName)
    {
        switch (itemName)
        {
            case "Helmet":
                return GameManager.Instance.inventoryManager.helmet.equipType;
            default:
                Debug.LogError("Unrecognized item name: " + itemName);
                return EquipmentType.Unknown;
        }
    }

    public int GetEquipAttack(string itemName)
    {
        switch (itemName)
        {
            case "Helmet":
                return GameManager.Instance.inventoryManager.helmet.equipAttack;
            default:
                Debug.LogError("Unrecognized item name: " + itemName);
                return 0;
        }
    }

    public int GetEquipDefense(string itemName)
    {
        switch (itemName)
        {
            case "Helmet":
                return GameManager.Instance.inventoryManager.helmet.equipDefense;
            default:
                Debug.LogError("Unrecognized item name: " + itemName);
                return 0;
        }
    }

}
