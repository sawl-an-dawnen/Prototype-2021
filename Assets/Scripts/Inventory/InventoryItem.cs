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

public enum Rarity
{
    N,
    R,
    SR,
    UR
}

[CreateAssetMenu(menuName = "Inventory/Item")]
public class InventoryItem : ScriptableObject
{
    public bool isEquip;

    public string itemName;
    public Sprite itemIcon;

    [Header("Item")]
    public Button itemButtonPrefab;
    public string itemInfo;

    [Header("Equipment")]
    public bool equipped;
    public EquipmentType equipType;
    public Rarity equipRarity;
    public int equipAttack;
    public int equipDefense;
    public string specialInfo;

    public void PreviewEquipment()
    {
        PlayerStatus playerStatus = GameObject.Find("StatPanel").GetComponent<PlayerStatus>();
        playerStatus.PreviewEquipmentStatus(itemIcon, equipAttack, equipDefense, specialInfo);
    }

    public void Equip()
    {
        equipped = true;
        PlayerStatus playerStatus = GameObject.Find("StatPanel").GetComponent<PlayerStatus>();
        GameManager.Instance.SetPlayerAttack(GameManager.Instance.GetPlayerAttack() + equipAttack);
        GameManager.Instance.SetPlayerDefense(GameManager.Instance.GetPlayerDefense() + equipDefense);
        GameManager.Instance.SaveCheckpoint();
        playerStatus.UpdateEquipmentStatus();
    }

    public void UnEquip()
    {
        equipped = false;
        PlayerStatus playerStatus = GameObject.Find("StatPanel").GetComponent<PlayerStatus>();
        GameManager.Instance.SetPlayerAttack(GameManager.Instance.GetPlayerAttack() - equipAttack);
        GameManager.Instance.SetPlayerDefense(GameManager.Instance.GetPlayerDefense() - equipDefense);
        GameManager.Instance.SaveCheckpoint();
        playerStatus.UpdateEquipmentStatus();
    }
}
