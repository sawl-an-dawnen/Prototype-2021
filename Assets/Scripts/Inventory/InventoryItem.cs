using System;
using UnityEngine;
using UnityEngine.UI;

public enum EquipmentType
{
    Head,
    Body,
    Shirt,
    Legs,
    MainHand,
    OffHand,
    Relic,
    Feet,
    Unknown
}

[CreateAssetMenu(menuName = "Inventory/Item")]
public class InventoryItem : ScriptableObject
{
    public bool isEquip;

    [Header("Item")]
    public string itemName;
    public Sprite itemIcon;
    public Button itemButtonPrefab;
    public string itemInfo;

    [Header("Equipment")]
    public bool equipped;
    public EquipmentType equipType;
    public string equipName;
    public Sprite equipIcon;
    public int equipAttack;
    public int equipDefense;
    public bool equipSpecial;
    public string specialInfo;

    public void Equip()
    {
        PlayerStatus playerStatus = GameObject.Find("StatPanel").GetComponent<PlayerStatus>();
        GameManager.Instance.SetPlayerAttack(GameManager.Instance.GetPlayerAttack() + equipAttack);
        GameManager.Instance.SetPlayerDefense(GameManager.Instance.GetPlayerDefense() + equipDefense);

        playerStatus.UpdateEquipmentStatus();
    }

    public void UnEquip()
    {
        PlayerStatus playerStatus = GameObject.Find("StatPanel").GetComponent<PlayerStatus>();
        GameManager.Instance.SetPlayerAttack(GameManager.Instance.GetPlayerAttack() - equipAttack);
        GameManager.Instance.SetPlayerDefense(GameManager.Instance.GetPlayerDefense() - equipDefense);

        playerStatus.UpdateEquipmentStatus();
    }
}
