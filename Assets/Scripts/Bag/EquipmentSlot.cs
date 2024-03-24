using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EquipmentSlot : MonoBehaviour, IPointerClickHandler
{
    private SpellsManager spellsManager;

    // Equipment Data
    public string equipName;
    public Sprite equipSprite;
    public bool isFull;
    public Sprite emptySprite;
    public EquipmentType equipType;

    // Equipment Slot
    [SerializeField]
    public Image equipImage;

    public GameObject slotShader;
    public bool equipSelected;

    // Equipped Slot
    [SerializeField]
    private EquippedSlot headSlot, bodySlot, shirtSlot, legsSlot, mainHandSlot, offHandSlot, relicSlot, feetSlot;

    private void Start()
    {
        spellsManager = GameObject.Find("SpellsManager").GetComponent<SpellsManager>();
    }

    public void AddEquipment(InventoryItem inventory)
    {
        this.equipType = inventory.equipType;
        this.equipName = inventory.equipName;
        this.equipSprite = inventory.equipIcon;
        isFull = true;
        equipImage.sprite = equipSprite;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            OnLeftClick();
        }
        //if (eventData.button == PointerEventData.InputButton.Right)
        //{
        //    OnRightClick();
        //}
    }

    public void OnLeftClick()
    {
        if (isFull && equipSelected)
        {
            EquipGear();
        }
        else if (isFull)
        {
            spellsManager.DeselectAllSlots();
            slotShader.SetActive(true);
            equipSelected = true;
        }
    }

    private void EquipGear()
    {
        if (equipType == EquipmentType.Head)
            headSlot.EquipGear(equipSprite, equipName);
        if (equipType == EquipmentType.Body)
            bodySlot.EquipGear(equipSprite, equipName);
        if (equipType == EquipmentType.Shirt)
            shirtSlot.EquipGear(equipSprite, equipName);
        if (equipType == EquipmentType.Legs)
            legsSlot.EquipGear(equipSprite, equipName);
        if (equipType == EquipmentType.MainHand)
            mainHandSlot.EquipGear(equipSprite, equipName);
        if (equipType == EquipmentType.OffHand)
            offHandSlot.EquipGear(equipSprite, equipName);
        if (equipType == EquipmentType.Relic)
            relicSlot.EquipGear(equipSprite, equipName);
        if (equipType == EquipmentType.Feet)
            feetSlot.EquipGear(equipSprite, equipName);

        EmptySlot();
    }

    public void EmptySlot()
    {
        spellsManager.DeselectAllSlots();
        equipImage.sprite = emptySprite;
        isFull = false;
    }

    //public void OnRightClick()
    //{

    //}
}
