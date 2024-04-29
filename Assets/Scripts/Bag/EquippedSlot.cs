using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class EquippedSlot : MonoBehaviour, IPointerClickHandler
{
    // Slot Appearance
    [SerializeField]
    public Image slotImage;

    [SerializeField]
    public TMP_Text slotName;

    // Slot Data
    //[SerializeField]
    public EquipmentType equipType = new EquipmentType();

    public InventoryItem equipment;
    private Sprite equipSprite;
    private string equipName;
    private string equipDescription;

    // Other Variable
    public bool slotInUse;
    [SerializeField]
    public GameObject slotShader;

    [SerializeField]
    public bool equipSelected;

    [SerializeField]
    public Sprite emptySprite;

    private SpellsManager spellsManager;

    private void Start()
    {
        spellsManager = GameObject.Find("SpellsManager").GetComponent<SpellsManager>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            OnLeftClick();
        }
    }

    public void OnLeftClick()
    {
        if (slotInUse && equipSelected)
        {
            UnEquipGear(equipment);
        }
        else if (slotInUse)
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

    public void EquipGear(InventoryItem newEquipment)
    {
        newEquipment.Equip();
        // If something is already equipped, send it back brfore re-writing the data for this slot
        if (slotInUse)
            UnEquipGear(equipment);

        this.equipment = newEquipment;

        // Update Image
        this.equipSprite = equipment.itemIcon;
        slotImage.sprite = this.equipSprite;
        slotName.enabled = false;

        // Update Status
        this.equipName = equipment.itemName;
        slotInUse = true;
    }

    public void UnEquipGear(InventoryItem equipment)
    {
        equipment.UnEquip();
        spellsManager.DeselectAllSlots();
        spellsManager.AddEquipment(equipment);

        // Update Slot Image
        this.equipSprite = emptySprite;
        slotImage.sprite = this.emptySprite;
        slotName.enabled = true;

        slotInUse = false;

        GameObject.Find("StatPanel").GetComponent<PlayerStatus>().TurnOffPreviewStatus();
    }
}