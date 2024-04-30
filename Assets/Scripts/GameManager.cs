using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Platformer.Mechanics;
using Platformer.Core;
using System.Threading.Tasks;
using System.IO;
using System.Linq;
using UnityEngine.Rendering;
using UnityEngine.UI;
using Ink.Runtime;

public class GameManager : MonoBehaviour
{
	public static GameManager Instance; // Singleton instance
	public SpellManager Spellmanager;
	public InputManager inputManager;
	public InventoryManager inventoryManager;

	public GameObject enemyToSpawn; // Store the collided enemy to spawn in the combat scene
	[SerializeField] private int playerHealth;
	[SerializeField] private int playerAttack;
	[SerializeField] private int playerDefense;
	[SerializeField] private float coins;
	[SerializeField] private bool CanOpen;
	[SerializeField] public List<string> items;
	[SerializeField] public int[,] shopItems = new int[5, 5];
	[SerializeField] public string[] itemNames = new string[5];
	[SerializeField] private Color playerColor;
	[SerializeField] public bool clockPuzzleSolved;
	[SerializeField] public bool counterPuzzleSolved;
	[SerializeField] public bool slidingPuzzleSolved;

	public Checkpoint.SpawnsDict Spawns = new();
	public Checkpoint.PlayDoorSoundDict PlayDoorSound = new();
	public Checkpoint.PlayerPosDict PlayerPos = new();
	public Checkpoint.AvailableSpellsDict AvailableSpells = new();
	public string SceneName => SceneManager.GetActiveScene().name;
	private string prevScene;
	private string enemyUID;
	public Checkpoint Checkpoint = null;
	public SceneChangeInvokable sceneChange;
	public string SaveFilePath;
	private bool tryingLoadingCheckpoint = false;
	private const int CAN_SPAWN = -1;
	private const int CANT_SPAWN = 0;
	public string[] DefaultSpells;

	public bool hasSpokeBigDragon = false;
    public bool hasSpokeSmallDragon = false;

    [Serializable]
	public class Spell
	{
		public string name;
		public Button prefabButton;
		public int[] cost;
		public string description;
		public string strongAgainst;
		public bool weaknessIsFound;
		
		public Action eventFunc;
	}

	public List<Spell> spells = new List<Spell>();

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);
			SaveFilePath = Application.dataPath + "/save_file.txt";
			Task.Run(AsyncGetCheckpoint);
		}
		else
		{
			Destroy(gameObject);
		}
	}

	private float gametime = 0;

	void Update()
	{
		if(SceneName != "MainMenu" && SceneName != "IntroStory" && SceneName != "EndingStory")
		{
			gametime += Time.deltaTime;
		}
	}

	private Dictionary<int, int> playerInventory = new Dictionary<int, int>();

	public void AddToInventory(int itemID, int quantity)
	{
		if (playerInventory.ContainsKey(itemID))
		{
			playerInventory[itemID] += quantity;
		}
		else
		{
			playerInventory[itemID] = quantity;
		}
	}

	public int GetInventoryQuantity(int itemID)
	{
		if (playerInventory.ContainsKey(itemID))
		{
			return playerInventory[itemID];
		}
		return 0;
	}

	public void AddSpell(string spellToAdd)
    {
		AvailableSpells[spellToAdd] = true;
		foreach (Spell spell in spells)
		{
			if (spell.name == spellToAdd)
			{
				Sprite img = spell.prefabButton.image.sprite;
				GameObject.Find("PopUpManager").GetComponent<PopUpManager>().CreatePopUp("You Found " + spellToAdd + ", press 'I' to check your inventory", img);
			}
		}
		SaveCheckpoint();
	}
	public float GetGameTime()
	{
		return gametime;
	}

	public void SetPlayerHealth(int i)
	{
		playerHealth = i;
	}

	public int GetPlayerHealth()
	{
		return playerHealth;
	}

	public void SetPlayerAttack(int i)
	{
		playerAttack = i;
	}

	public int GetPlayerAttack()
	{
		return playerAttack;
	}

	public void SetPlayerDefense(int i)
	{
		playerDefense = i;
	}

	public int GetPlayerDefense()
	{
		return playerDefense;
	}

	public void SetCoins(float i)
	{
		coins = i;
	}

	public float GetCoins()
	{
		return coins;
	}

	public Color GetPlayerColor()
	{
		return playerColor;
	}

	public Color SetPlayerColor(Color color)
	{
		playerColor = color;
		return playerColor;
	}

	public bool getBigDragonStatus()
	{
		return hasSpokeBigDragon;
	}
    public void setBigDragonStatus()
    {
        hasSpokeBigDragon = true;
    }
    public bool getSmallDragonStatus()
    {
		return hasSpokeSmallDragon;
    }
    public void setSmallDragonStatus()
    {
        hasSpokeSmallDragon = true;
    }


    //public void AddCoins(float coin)
    //{
    //	//coins += coin;
    //	SetCoins(GetCoins() + coin);
    //}

    public void foundWeakness(string spellName){
		spells.Find(x=> x.name == spellName).weaknessIsFound = true;
	}

	public void OpenDoor()
    {
		Debug.Log("Call OpenDoor");
		CanOpen = true;
	}

	public bool GetOpen()
    {
		return CanOpen;
	}

	public List<string> GetItmes()
    {
		return items;
    }

	public void AddItem(InventoryItem item)
	{
		items.Add(item.name);
	}

	public void RemoveItem(InventoryItem item)
    {
		items.Remove(item.name);
	}

	public void RegisterRoomSpawner(RoomSpawner res)
	{
		Spawns.TryAdd(SceneName, new());
		Spawns[SceneName].TryAdd(res.uID, CAN_SPAWN);
	}

	public void PrepareForCombatSceneEnter(Vector3 playerPos, string enemyUID)
	{
		if (Checkpoint.SceneName == "") SaveCheckpoint();
		PlayerPos[SceneName] = playerPos;
		this.enemyUID = enemyUID;
		prevScene = SceneName;
	}

	public string PrepareForReturnFromCombat()
	{
		Spawns[prevScene][enemyUID] = CANT_SPAWN;
		return prevScene;
	}

	public bool CanSpawn(string uID)
	{
		return Spawns[SceneName][uID] == CAN_SPAWN;
	}

	public int SavedDialogueIdx(string uID)
	{
		return Spawns[SceneName][uID];
	}

	public void SaveDialogue(string uID, int idx, Vector3 pos)
	{
		PlayerPos[SceneName] = pos;
		Spawns[SceneName][uID] = idx;
		SaveCheckpoint();
	}

	public void SaveCheckpoint()
	{
		Checkpoint = new Checkpoint(
			playerHealth,
			playerAttack,
			playerDefense,
			Spawns,
			PlayDoorSound,
			PlayerPos,
			AvailableSpells,
			SceneName,
			coins,
			CanOpen,
			items,
			playerColor,
			hasSpokeBigDragon,
			hasSpokeSmallDragon,
			clockPuzzleSolved,
			counterPuzzleSolved,
			slidingPuzzleSolved
		);
		SaveFileManager.WriteToSaveFile(SaveFilePath, Checkpoint);
	}

	public void LoadCheckpoint()
	{
		if (Checkpoint.SceneName == "")
		{
			return;
		}
		playerHealth = Checkpoint.playerHealth;
		playerAttack = Checkpoint.playerAttack;
		playerDefense = Checkpoint.playerDefense;
		Spawns = Checkpoint.spawns;
		PlayDoorSound = Checkpoint.playDoorSound;
		PlayerPos = Checkpoint.playerPos;
		AvailableSpells = Checkpoint.spells;
		sceneChange.sceneName = Checkpoint.SceneName;
		coins = Checkpoint.coins;
		sceneChange.Invoke();
		CanOpen = Checkpoint.CanOpen;
		items = Checkpoint.items;
		playerColor = Checkpoint.playerColor;
		clockPuzzleSolved = Checkpoint.clockPuzzleSolved;
		counterPuzzleSolved = Checkpoint.counterPuzzleSolved;
		slidingPuzzleSolved = Checkpoint.slidingPuzzleSolved;

		//SpellsManager spellsManager = GameObject.Find("SpellsManager").GetComponent<SpellsManager>();
		//spellsManager.ContinuePanel();
	}

	public void NewGame()
	{
		const string scene = "IntroStory";
		items.Clear();
		Checkpoint = new(100, 5, 3, new(), new(), new(), CreateDefaultAvailableSpells(), scene, 0, false, new(), new Color(1f, 1f, 1f, 0f), false, false, false, false, false);
		SpellsManager spellsManager = GameObject.Find("SpellsManager").GetComponent<SpellsManager>();
		spellsManager.ClearEquip();

		LoadCheckpoint();
	}

	public void Continue()
	{
		if (!File.Exists(SaveFilePath))
		{
			Debug.LogError("Tried to load nonexistent checkpoint file");
			return;
		}
		else if (tryingLoadingCheckpoint)
		{
			return;
		}
		StartCoroutine(TryLoadCheckpoint());
	}

	public async void AsyncGetCheckpoint()
	{
		Checkpoint = await SaveFileManager.ReadFromSaveFile(SaveFilePath);
		Resources.UnloadUnusedAssets();
	}

	IEnumerator TryLoadCheckpoint()
	{
		tryingLoadingCheckpoint = true;
		for (int i = 0; i < 30; i++)
		{
			if (Checkpoint.SceneName != "")
			{
				LoadCheckpoint();
				break;
			}
			yield return new WaitForSeconds(1f);
		}
		tryingLoadingCheckpoint = false;
	}
	
	public void RegisterSpellEventFunc(string spellName, Action eventFunc)
	{
		var spell = spells.Find(s => s.name.Equals(spellName));
		if (spell != null)
		{
			spell.eventFunc = eventFunc;
		}
	}
	
	public Checkpoint.AvailableSpellsDict CreateDefaultAvailableSpells()
	{
		Checkpoint.AvailableSpellsDict availSpells = new();
		// Set default spells
		foreach (var spellName in DefaultSpells)
		{
			availSpells[spellName] = true;
		}
		return availSpells;
	}
	
	public void RegisterInventoryManager(InventoryManager im)
	{
		inventoryManager = im;
	}

}
