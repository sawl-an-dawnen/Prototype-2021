using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EquipmentSlot : MonoBehaviour, IPointerClickHandler
{
    private SpellsManager spellsManager;

    // Equipment Data
    private InventoryItem equipment;
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
        this.equipment = inventory;
        this.equipType = inventory.equipType;
        this.equipName = inventory.itemName;
        this.equipSprite = inventory.itemIcon;
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
            GameObject.Find("StatPanel").GetComponent<PlayerStatus>().TurnOffPreviewStatus();
        }
        else if (isFull)
        {
            spellsManager.DeselectAllSlots();
            slotShader.SetActive(true);
            equipSelected = true;
            equipment.PreviewEquipment();
        }
        else
        {
            spellsManager.DeselectAllSlots();
            GameObject.Find("StatPanel").GetComponent<PlayerStatus>().TurnOffPreviewStatus();
        }
    }

    private void EquipGear()
    {
        if (equipType == EquipmentType.Head)
            headSlot.EquipGear(equipment);
        if (equipType == EquipmentType.Body)
            bodySlot.EquipGear(equipment);
        if (equipType == EquipmentType.Belt)
            shirtSlot.EquipGear(equipment);
        if (equipType == EquipmentType.Relic)
            legsSlot.EquipGear(equipment);
        if (equipType == EquipmentType.MainHand)
            mainHandSlot.EquipGear(equipment);
        if (equipType == EquipmentType.OffHand)
            offHandSlot.EquipGear(equipment);
        if (equipType == EquipmentType.Neck)
            relicSlot.EquipGear(equipment);
        if (equipType == EquipmentType.Ring)
            feetSlot.EquipGear(equipment);

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
