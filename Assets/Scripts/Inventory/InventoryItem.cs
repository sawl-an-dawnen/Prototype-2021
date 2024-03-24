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
    public EquipmentType equipType;
    public string equipName;
    public Sprite equipIcon;
    public Button equipButtonPrefab;
    public int equipAttack;
    public int equipDefense;
    public bool equipSpecial;
    public string specialInfo;
}
