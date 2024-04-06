using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseIfActive : MonoBehaviour
{
	private bool gameIsPaused = false;
	public GameObject targetObject;

	void Update()
	{
		if (targetObject != null && targetObject.activeSelf)
		{
			Pause();
		}
	}

	public void Resume()
	{
		if (gameIsPaused)
		{
			Time.timeScale = 1f;
			gameIsPaused = false;
		}
	}

	void Pause()
	{
		if (!gameIsPaused)
		{
			Time.timeScale = 0f;
			gameIsPaused = true;
		}
	}
}
