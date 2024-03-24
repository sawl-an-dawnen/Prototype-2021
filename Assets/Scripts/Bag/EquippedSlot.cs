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
    private Image slotImage;

    [SerializeField]
    private TMP_Text slotName;

    // Slot Data
    [SerializeField]
    private EquipmentType equipType = new EquipmentType();

    private Sprite equipSprite;
    private string equipName;
    private string equipDescription;

    // Other Variable
    private bool slotInUse;
    [SerializeField]
    public GameObject slotShader;

    [SerializeField]
    public bool equipSelected;

    [SerializeField]
    private Sprite emptySprite;

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
            UnEquipGear();
        }
        else if (slotInUse)
        {
            spellsManager.DeselectAllSlots();
            slotShader.SetActive(true);
            equipSelected = true;
        }
    }

    public void EquipGear(Sprite equipSprite, string equipName)
    {
        // If something is already equipped, send it back brfore re-writing the data for this slot
        if (slotInUse)
            UnEquipGear();

        // Update Image
        this.equipSprite = equipSprite;
        slotImage.sprite = this.equipSprite;
        slotName.enabled = false;

        // Update Status
        this.equipName = equipName;
        slotInUse = true;
    }

    public void UnEquipGear()
    {
        spellsManager.DeselectAllSlots();
        spellsManager.AddEquipment(equipName, equipSprite, equipType);

        // Update Slot Image
        this.equipSprite = emptySprite;
        slotImage.sprite = this.emptySprite;
        slotName.enabled = true;

        slotInUse = false;
    }
}