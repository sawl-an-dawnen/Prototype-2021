using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatTutorialPanel : MonoBehaviour
{
	public GameObject panel;

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.X))
		{
			if (panel != null)
			{
				panel.SetActive(false);
			}
		}
	}
}
