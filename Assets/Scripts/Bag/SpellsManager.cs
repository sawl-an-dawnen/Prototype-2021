using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class SpellsManager : MonoBehaviour
{
	public GameObject panel;
	public GameObject spellsPanel;
	public GameObject spellDetailsPanel;
	public GameObject inventoryPanel;
	public GameObject inventoryDetailsPanel;
	public static bool GameIsPaused = false;

	public EquipmentSlot[] equipSlot;
	public EquippedSlot[] equippedSlot;

	private List<Button> spellButtons = new List<Button>();
	private List<Button> inventoryButtons = new List<Button>();

	private ItemMapper itemMapper; // Reference to the ItemMapper script

	void Start()
	{
		panel.SetActive(false);
		itemMapper = FindObjectOfType<ItemMapper>(); // Find ItemMapper in the scene
		if (itemMapper == null)
		{
			Debug.LogError("ItemMapper script not found in the scene.");
		}
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.I))
		{
			if (GameIsPaused)
			{
				Resume();
				ClearPanel();
			}
			else
			{
				Pause();
				PopulatePanel();
			}
		}
	}

	public void Resume()
	{
		panel.SetActive(false);
		Time.timeScale = 1f;
		GameIsPaused = false;
		spellDetailsPanel.gameObject.SetActive(false);
		inventoryDetailsPanel.gameObject.SetActive(false);

	}

	public void Pause()
	{
		panel.SetActive(true);
		Time.timeScale = 0f;
		GameIsPaused = true;
	}

	public void PopulatePanel()
	{
		GameManager gameManager = GameManager.Instance;

		foreach (GameManager.Spell spell in gameManager.spells)
		{
			if (gameManager.AvailableSpells.TryGetValue(spell.name, out bool available) && available)
			{
				Button spellButton = Instantiate(spell.prefabButton, spellsPanel.transform);
				spellButtons.Add(spellButton);

				spellButton.onClick.AddListener(() => ShowDetailsPanel(spell));
			}
		}

		List<string> items = gameManager.GetItmes();
		InventoryManager inventoryManager = gameManager.inventoryManager;

		foreach (var item in items)
		{
			InventoryItem inventory = itemMapper.GetInventory(item);

			// Not equipment:
			if (!inventory.isEquip)
            {
				Button inventoryButton = Instantiate(inventory.itemButtonPrefab.GetComponent<Button>(), inventoryPanel.transform);
				Image buttonImage = inventoryButton.GetComponent<Image>();
				if (buttonImage != null)
				{
					buttonImage.sprite = inventory.itemIcon;
				}
				else
				{
					Debug.LogError("Image component not found on the Button.");
				}

				inventoryButtons.Add(inventoryButton);
				inventoryButton.onClick.AddListener(() => ShowItemsDetailsPanel(inventory));
			}
			// Equipment:
			else
			{
				//bool equipped = itemMapper.GetEquipped(item);
				//Sprite equipIcon = itemMapper.GetIcon(item);
				//Button itemPrefab = itemMapper.GetButtonPrefab(item);
				//EquipmentType equipType = itemMapper.GetEquipType(item);
				//int equipAttack = itemMapper.GetEquipAttack(item);
				//int equipDefense = itemMapper.GetEquipDefense(item);

				if (inventory.equipType == EquipmentType.Unknown)
				{
					Debug.LogError("Error: Failed to retrieve equip details for: " + item);
					continue;
				}

				AddEquipment(inventory);
			}
        }
	}


	public void ClearPanel()
	{
		foreach (Button button in spellButtons)
		{
			Destroy(button.gameObject);
		}

		spellButtons.Clear();

		foreach (Button button in inventoryButtons)
		{
			Destroy(button.gameObject);
		}

		inventoryButtons.Clear();

		for (int i = 0; i < equipSlot.Length; i++)
        {
			equipSlot[i].equipImage.sprite = equipSlot[i].emptySprite;
			equipSlot[i].isFull = false;
		}
	}

	private void ShowDetailsPanel(GameManager.Spell spell)
	{
		SpellsDetailsPanel detailsPanel = spellDetailsPanel.GetComponent<SpellsDetailsPanel>();
		if (detailsPanel != null)
		{
			detailsPanel.gameObject.SetActive(true);
			detailsPanel.ShowDetails(spell);
		}
		else
		{
			Debug.LogError("SpellsDetailsPanel component not found on spellDetailsPanel GameObject.");
		}
	}

	private void ShowItemsDetailsPanel(InventoryItem inventory)
	{
		InventoryDetailsPanel detailsPanel = inventoryDetailsPanel.GetComponent<InventoryDetailsPanel>();
		if (detailsPanel != null)
		{
			detailsPanel.gameObject.SetActive(true);
			detailsPanel.ShowDetails(inventory.itemName, inventory.itemIcon, inventory.itemInfo);
        }
		else
		{
			Debug.LogError("SpellsDetailsPanel component not found on spellDetailsPanel GameObject.");
		}
	}

	// Add iquipment to Equipment Panel
	public void AddEquipment(InventoryItem inventory)
	{
		for (int i = 0; i < equipSlot.Length; i++)
        {
			if (equipSlot[i].isFull == false)
            {
				equipSlot[i].AddEquipment(inventory);
				return;
            }
        }
    }

	public void DeselectAllSlots()
    {
		for (int i = 0; i < equipSlot.Length; i++)
        {
			equipSlot[i].slotShader.SetActive(false);
			equipSlot[i].equipSelected = false;
		}
		for (int i = 0; i < equippedSlot.Length; i++)
		{
			equippedSlot[i].slotShader.SetActive(false);
			equippedSlot[i].equipSelected = false;
		}
	}

}
