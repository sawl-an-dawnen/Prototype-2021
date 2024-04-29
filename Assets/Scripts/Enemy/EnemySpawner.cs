using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
	public GameObject[] enemyPrefabs; // Array of enemy prefabs to choose from
	[SerializeField] TMP_Text enemyNameText; // Text to display the enemy name

	void Start()
	{
        // Check if GameManager has an enemy to spawn
        string objectToSpawn = PlayerPrefs.GetString("ObjectToSpawn", "Skeleton NLA w_Lights");

        // Loop through the available enemy prefabs and find the one that matches the stored name
        GameObject enemyToSpawn = null;
		if (objectToSpawn != null)
		{
			foreach (GameObject enemyPrefab in enemyPrefabs)
			{
				if (enemyPrefab.name.ToLower().Contains(objectToSpawn.ToLower()))
				{
					enemyToSpawn = enemyPrefab;
					break;
				}
			}
		}

		if (enemyToSpawn != null)
		{
			var enemyObjTransform = GameObject.Find("Enemi").transform;
			Instantiate(enemyToSpawn, transform.position, transform.rotation);

			string enemyName = "";
			switch (enemyToSpawn.name)
			{
				case "Skeleton NLA w_Lights":
					enemyName = "Skeleton";
					break;
				case "MonsterEye":
					enemyName = "Soul Stare";
					break;
				case "horseBoss":
					enemyName = "Chess Guardian";
					break;
				case "EnemyGhost":
					enemyName = "Ghost";
					break;
				case "Mushroom en":
					enemyName = "Poisonous Mushroom";
					break;
				case "witch":
					enemyName = "Wicked Witch";
					break;
				default:
					enemyName = enemyToSpawn.name;
					break;
			}

			enemyNameText.text = enemyName;
		}
		else
		{
			// Handle the case where the enemy prefab with the stored name is not found
			Debug.LogError("Enemy prefab with name '" + objectToSpawn + "' not found!");
		}
	}
}
