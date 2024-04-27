using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using UnityEngine.UI;

public class SceneSwitcher : MonoBehaviour
{
	private void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.CompareTag("Enemy"))
		{
			StartCoroutine(SwitchSceneWithDelay(collision.gameObject));
		}
	}

	//Raw Image to Show Video Images [Assign from the Editor]
	public RawImage image;
	//Video To Play [Assign from the Editor]
	public VideoClip videoToPlay;
	private VideoPlayer videoPlayer;
	private VideoSource videoSource;

	private IEnumerator SwitchSceneWithDelay(GameObject enemyToSpawn)
	{
		// Wait for the GameManager to be initialized
		yield return new WaitForEndOfFrame();

		// Store the enemy to spawn and player health in the GameManager script
		GameManager.Instance.enemyToSpawn = enemyToSpawn;

		// You can't directly store a GameObject in PlayerPrefs, so we'll store its name or a unique identifier
		PlayerPrefs.SetString("ObjectToSpawn", enemyToSpawn.name);

		// Get the player's health from the PlayerHealthBar component
		PlayerHealthBar playerHealthBar = GetComponent<PlayerHealthBar>();
		if (playerHealthBar != null)
		{
			GameManager.Instance.SetPlayerHealth(playerHealthBar.currentHealth);
		}
		else
		{
			// Handle the case where the PlayerHealthBar component is not found
			Debug.LogError("PlayerHealthBar component not found on the player!");
		}

		// Debug.Log("EnemyToSpawn = " + GameManager.Instance.enemyToSpawn);
		// Debug.Log("PlayerHealth = " + GameManager.Instance.playerHealth);

		var newScene = "Combat Arena";
		// We need to save the scene we're using before switching so we know what to return to
		GameManager.Instance.PrepareForCombatSceneEnter(transform.position, enemyToSpawn.GetComponent<Enemy>().uID);
		// Load the combat scene. Make sure you have this scene created in your Unity project.


		Debug.Log("enemy!!!");

		//videoPlayer = GameObject.Find("GameManager").AddComponent<VideoPlayer>();
		//videoPlayer.playOnAwake = false;
		//videoPlayer.skipOnDrop = false;
		//videoPlayer.source = VideoSource.VideoClip;
		//videoPlayer.clip = videoToPlay;

		videoPlayer = GameObject.Find("GameManager").GetComponent<VideoPlayer>();
		videoPlayer.source = VideoSource.VideoClip;
		videoPlayer.clip = videoToPlay;
		videoPlayer.Prepare();
		while (!videoPlayer.isPrepared)
		{
			yield return null;
		}
		image.texture = videoPlayer.texture;
		videoPlayer.Play();

		DontDestroyOnLoad(videoPlayer);
		DontDestroyOnLoad(image);
			
		yield return new WaitForSeconds(2);
		AsyncOperation op = SceneManager.LoadSceneAsync(newScene);
		//Destroy(videoPlayer, 6);

	}

    public void SwitchCombatSceneWithDelay(GameObject enemyToSpawn)
	{
		// Store the enemy to spawn and player health in the GameManager script
		GameManager.Instance.enemyToSpawn = enemyToSpawn;

		// You can't directly store a GameObject in PlayerPrefs, so we'll store its name or a unique identifier
		PlayerPrefs.SetString("ObjectToSpawn", enemyToSpawn.name);

		// Get the player's health from the PlayerHealthBar component
		PlayerHealthBar playerHealthBar = GetComponent<PlayerHealthBar>();
		if (playerHealthBar != null)
		{
			GameManager.Instance.SetPlayerHealth(playerHealthBar.currentHealth);
		}
		else
		{
			// Handle the case where the PlayerHealthBar component is not found
			Debug.LogError("PlayerHealthBar component not found on the player!");
		}

		// Debug.Log("EnemyToSpawn = " + GameManager.Instance.enemyToSpawn);
		// Debug.Log("PlayerHealth = " + GameManager.Instance.playerHealth);

		var newScene = "Combat Arena";
		// We need to save the scene we're using before switching so we know what to return to
		GameManager.Instance.PrepareForCombatSceneEnter(transform.position, enemyToSpawn.GetComponent<Enemy>().uID);
		// Load the combat scene. Make sure you have this scene created in your Unity project.
		SceneManager.LoadScene(newScene);

	}
}

