using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using UnityEngine.SceneManagement;

public class SpellsManager : MonoBehaviour
{
	public GameObject bag;
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

	private static SpellsManager instance;
	void Awake()
	{
		if (instance == null)
		{
			instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else
		{
			Destroy(gameObject);
		}
	}

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
		string currentSceneName = SceneManager.GetActiveScene().name;
		if (currentSceneName == "MainMenu" || currentSceneName == "Credits" || currentSceneName == "IntroStory" || currentSceneName == "TrailerViewer")
        {
			bag.SetActive(false);
        }
        else
        {
			bag.SetActive(true);
			if (Input.GetKeyDown(KeyCode.I))
			{
				if (GameIsPaused)
				{
					Resume();
					ClearPanel();
				}
				else
				{
					//PlayerStatus playerStatus = GameObject.Find("SpellsManager").GetComponent<PlayerStatus>();
					Pause();
					//playerStatus.UpdateEquipmentStatus();
					PopulatePanel();
				}
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
		}
	}

	public void ContinuePanel()
	{
		GameManager gameManager = GameManager.Instance;
		List<string> items = gameManager.GetItmes();
		foreach (var item in items)
		{
			InventoryItem inventory = itemMapper.GetInventory(item);
			if (inventory.isEquip)
            {
				if (inventory.equipped)
                {
					for (int i = 0; i < equippedSlot.Length; i++)
					{
						equippedSlot[i].slotImage.sprite = inventory.itemIcon;
						equippedSlot[i].slotInUse = true;
						equippedSlot[i].slotName.enabled = false;
					}
				}
                else
                {
					for (int i = 0; i < equipSlot.Length; i++)
					{
						equipSlot[i].equipImage.sprite = inventory.itemIcon;
						equipSlot[i].isFull = true;
					}
				}
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
	}

	public void ClearEquip()
    {
		for (int i = 0; i < equipSlot.Length; i++)
        {
            equipSlot[i].equipImage.sprite = equipSlot[i].emptySprite;
            equipSlot[i].isFull = false;
        }
		for (int i = 0; i < equippedSlot.Length; i++)
		{
			if (equippedSlot[i].slotInUse)
            {
				equippedSlot[i].equipment.equipped = false;
			}
			equippedSlot[i].slotImage.sprite = equippedSlot[i].emptySprite;
			equippedSlot[i].slotInUse = false;
			equippedSlot[i].slotName.enabled = true;
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
