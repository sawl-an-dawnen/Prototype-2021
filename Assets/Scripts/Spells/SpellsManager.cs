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
			bool isEquip = itemMapper.InventoryType(item);
			if (!isEquip)
            {
				Sprite itemIcon = itemMapper.GetIcon(item);
				Button itemPrefab = itemMapper.GetButtonPrefab(item);
				string itemInfo = itemMapper.GetInfo(item);

				if (itemIcon == null || itemPrefab == null || itemInfo == null)
				{
					Debug.LogError("Error: Failed to retrieve item details for: " + item);
					continue;
				}

				Button inventoryButton = Instantiate(itemPrefab.GetComponent<Button>(), inventoryPanel.transform);
				Image buttonImage = inventoryButton.GetComponent<Image>();
				if (buttonImage != null)
				{
					buttonImage.sprite = itemIcon;
				}
				else
				{
					Debug.LogError("Image component not found on the Button.");
				}

				inventoryButtons.Add(inventoryButton);

				inventoryButton.onClick.AddListener(() => ShowItemsDetailsPanel(item, itemIcon, itemInfo));
			}
            else
            {
				Sprite equipIcon = itemMapper.GetIcon(item);
				Button itemPrefab = itemMapper.GetButtonPrefab(item);
				EquipmentType equipType = itemMapper.GetEquipType(item);
				int equipAttack = itemMapper.GetEquipAttack(item);
				int equipDefense = itemMapper.GetEquipDefense(item);

				if (equipIcon == null || itemPrefab == null || equipType == EquipmentType.Unknown)
				{
					Debug.LogError("Error: Failed to retrieve equip details for: " + item);
					Debug.LogError(equipIcon);
                    Debug.LogError(itemPrefab);
                    Debug.LogError(equipType);
					continue;
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

	private void ShowItemsDetailsPanel(string itemName, Sprite itemIcon, string itemInfo)
	{
		InventoryDetailsPanel detailsPanel = inventoryDetailsPanel.GetComponent<InventoryDetailsPanel>();
		if (detailsPanel != null)
		{
			detailsPanel.gameObject.SetActive(true);
			detailsPanel.ShowDetails(itemName, itemIcon, itemInfo);
        }
		else
		{
			Debug.LogError("SpellsDetailsPanel component not found on spellDetailsPanel GameObject.");
		}
	}

	//private void ShowEquipDetailsPanel(string equipName, Sprite equipIcon, string equipInfo)
	//{
	//	InventoryDetailsPanel detailsPanel = inventoryDetailsPanel.GetComponent<InventoryDetailsPanel>();
	//	if (detailsPanel != null)
	//	{
	//		detailsPanel.gameObject.SetActive(true);
	//		detailsPanel.ShowDetails(itemName, itemIcon, itemInfo);
	//	}
	//	else
	//	{
	//		Debug.LogError("SpellsDetailsPanel component not found on spellDetailsPanel GameObject.");
	//	}
	//}

}
