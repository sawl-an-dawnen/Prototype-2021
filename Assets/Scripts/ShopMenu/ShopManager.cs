using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopManager : MonoBehaviour
{
	public int[,] shopItems = new int[8, 8];
	public string[] itemNames = new string[8];
	public float coins;
	public TextMeshProUGUI ConisTXT;
	public Confirmation confirmationWindow;
	public Button[] itemButtons;
	public static bool GameIsPaused = false; 
	[SerializeField] public AudioSource buySound;
	private GameManager gameManager;

    void OnEnable()
	{
		Pause();
	}

	void Start()
	{
		gameManager = GameManager.Instance;
		ConisTXT.text = "Coins: " + gameManager.GetCoins().ToString();
		//ID's
		shopItems[1, 1] = 1;
		shopItems[1, 2] = 2;
		shopItems[1, 3] = 3;
		shopItems[1, 4] = 4;
		shopItems[1, 5] = 5;
		shopItems[1, 6] = 6;
		shopItems[1, 7] = 7;

		//Price
		shopItems[2, 1] = 10;
		shopItems[2, 2] = 30;
		shopItems[2, 3] = 40;
		shopItems[2, 4] = 0;
		shopItems[2, 5] = 0;
		shopItems[2, 6] = 0;
		shopItems[2, 7] = 0;

		//Names
		itemNames[1] = "Knife Throw";
		itemNames[2] = "Heal";
		itemNames[3] = "Stun";
		itemNames[4] = "Fire Element";
		itemNames[5] = "Water Element";
		itemNames[6] = "Earth Element";
		itemNames[7] = "Elemental Influence";
		InitializeShop();
	}

	void Pause()
	{
		Time.timeScale = 0f;
		GameIsPaused = true;
	}

	public void Resume()
	{
		Time.timeScale = 1f;
		GameIsPaused = false;
	}

	public void InitializeShop()
	{
		for (int i = 1; i < itemButtons.Length + 1; i++)
		{
			if (GameManager.Instance.AvailableSpells.ContainsKey(itemNames[i]) && GameManager.Instance.AvailableSpells[itemNames[i]])
			{
				DisableItemButton(i);
			}
		}
	}


	public void Buy(int itemID)
	{
		int itemPrice = shopItems[2, itemID];

		if (gameManager.GetCoins() >= itemPrice)
		{
			confirmationWindow.OpenConfirmationWindow("Are you sure you want to buy " + itemNames[itemID] + " for $" + itemPrice + "?", itemID);
			
		}
		else
		{
			Debug.Log("You don't have any coin left.");
		}
	}

	/* not interactable version
    public void DisableItemButton(int itemID)
    {
        if (itemID < itemButtons.Length)
        {
            itemButtons[itemID - 1].interactable = false;
        }
    }
	*/
	public void DisableItemButton(int itemID)
	{
		Debug.Log(itemID);
		if (itemID < itemButtons.Length)
		{
			itemButtons[itemID - 1].gameObject.SetActive(false);
		}
	}
	
	


}