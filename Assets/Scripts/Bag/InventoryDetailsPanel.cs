using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;

public class InventoryDetailsPanel : MonoBehaviour
{
    public TextMeshProUGUI inventoryNameText;
    public Image inventoryImage;
    public TextMeshProUGUI inventoryDescriptionText;

    public void ShowDetails(string itemName, Sprite itemIcon, string itemInfo)
    {
        inventoryNameText.text = itemName;
        inventoryImage.sprite = itemIcon;
        inventoryDescriptionText.text = itemInfo;
    }
}
