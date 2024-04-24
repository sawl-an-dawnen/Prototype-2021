using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dontDestroyTransition : MonoBehaviour
{
	private static dontDestroyTransition instance;
	void Awake()
	{
		if (instance == null)
		{
			instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else
		{
			Destroy(gameObject);
		}
	}
}
