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

	public GameObject enemyToSpawn; // Store the collided enemy to spawn in the combat scene
	[SerializeField] private int playerHealth;
	[SerializeField] private float coins;
	[SerializeField] public int[,] shopItems = new int[5, 5];
	[SerializeField] public string[] itemNames = new string[5];
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

	[Serializable]
	public class Spell
	{
		public string name;
		public Button prefabButton;
		public int[] cost;
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

	public void SetPlayerHealth(int i)
	{
		playerHealth = i;
	}

	public int GetPlayerHealth()
	{
		return playerHealth;
	}

	public void SetCoins(float i)
	{
		coins = i;
	}

	public float GetCoins()
	{
		return coins;
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
			Spawns,
			PlayDoorSound,
			PlayerPos,
			AvailableSpells,
			SceneName,
			coins
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
		Spawns = Checkpoint.spawns;
		PlayDoorSound = Checkpoint.playDoorSound;
		PlayerPos = Checkpoint.playerPos;
		AvailableSpells = Checkpoint.spells;
		sceneChange.sceneName = Checkpoint.SceneName;
		coins = Checkpoint.coins;
		sceneChange.Invoke();
	}

	public void NewGame()
	{
		const string scene = "Main Scene 1";
		Checkpoint = new(100, new(), new(), new(), CreateDefaultAvailableSpells(), scene, 40);
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

}
