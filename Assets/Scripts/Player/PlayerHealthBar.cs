using UnityEngine;
using TMPro;

public class PlayerHealthBar : MonoBehaviour
{
	public int maxHealth = 100;
	public HealthBar healthBar;
	public int currentHealth;
	private bool isPlayer;
	public GameObject floatingDamage;

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
		GameObject damageInstance = Instantiate(floatingDamage, transform.position + new Vector3(0f, 500f, 0f), Quaternion.identity, transform);
		TextMeshProUGUI textMeshPro = damageInstance.GetComponentInChildren<TextMeshProUGUI>();
		textMeshPro.text = damage.ToString();
		if (damage > 0)
		{
			if (!isPlayer)
			{
				textMeshPro.rectTransform.localPosition = new Vector3(605f, 500f, 0f);
			}
			else
			{
				textMeshPro.rectTransform.localPosition = new Vector3(-500f, 500f, 0f);
			}
		}

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
	* This should be move to somewhere else later
	*/
	public void AddCoins(float coin)
	{
		GameManager.Instance.SetCoins(GameManager.Instance.GetCoins() + coin);
	}
}
