/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealthBar : MonoBehaviour
{
	public int maxHealth = 100;
	public int currentHealth;
	public HealthBar healthBar;
	private RectTransform canvasRectTransform;

    private bool isPlayer; // Indicate if it's the player

    private void Start()
    {
        isPlayer = CompareTag("Player"); // Check if this is the player

        // Check if the initial health value is already set in PlayerPrefs
        if (isPlayer && PlayerPrefs.HasKey("InitialHealth"))
        {
            currentHealth = PlayerPrefs.GetInt("InitialHealth");
        }
        else
        {
            // If not set or not the player, initialize health to the maximum
            currentHealth = maxHealth;

            if (isPlayer)
            {
                // Store the initial health value only for the player
                PlayerPrefs.SetInt("InitialHealth", currentHealth);
            }
        }

        healthBar.SetHealth(currentHealth);
    }

    // Rest of the script remains the same...

    public int TakeDamage(int damage, bool isPlayer)
    {
        Debug.Log("Taking damage: " + damage);
        currentHealth = damage >= currentHealth ? 0 : (currentHealth - damage);

        // Store the updated health in PlayerPrefs only for the player
        if (isPlayer)
        {
            PlayerPrefs.SetInt("InitialHealth", currentHealth);
            PlayerPrefs.Save();
        }

        // Update the health bar
        healthBar.SetHealth(currentHealth);

        return currentHealth;
    }

    public void HPManager(int damage)
	{
		TakeDamage(damage, true);
	}

}
*/

using UnityEngine;


public class PlayerHealthBar : MonoBehaviour
{
	public int maxHealth = 100;
	public HealthBar healthBar;
	public int currentHealth;
	private bool isPlayer;

	private void Start()
	{
		isPlayer = CompareTag("Player");
		if (isPlayer)
        {
			currentHealth = GameManager.Instance.GetPlayerHealth();
			healthBar.SetHealth(currentHealth);
		}
        else
        {
			healthBar.SetHealth(maxHealth);
		}

	}

	public int TakeDamage(int damage, bool isPlayer = true)
	{
		Debug.Log("Taking damage: " + damage);
		int oldHealth = currentHealth;
		currentHealth = Mathf.Min(Mathf.Max(0, currentHealth - damage), 500);

		if (isPlayer)
		{
			GameManager.Instance.SetPlayerHealth(currentHealth);
		}
		healthBar.SetHealth(currentHealth);

		return currentHealth;
	}

	public void HPManager(int damage)
	{
		TakeDamage(damage, true);
	}

	/*
	 * This should be moved somewhere else but I'm too tired now to create a new script
	*/
	public void AddCoins(float coin)
	{
		//coins += coin;
		GameManager.Instance.SetCoins(GameManager.Instance.GetCoins() + coin);
	}
}
