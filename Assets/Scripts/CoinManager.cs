using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CoinManager : MonoBehaviour
{
	public TextMeshProUGUI coinText;

	void Update()
	{
		UpdateCoinText();
	}

	public void UpdateCoinText()
	{
		coinText.text = GameManager.Instance.GetCoins().ToString();
	}
}
