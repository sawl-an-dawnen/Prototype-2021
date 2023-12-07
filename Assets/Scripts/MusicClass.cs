using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicClass : MonoBehaviour
{
	private AudioSource _audioSource;
    private static MusicClass _instance;

    private void Awake()
	{
       
        if (_instance == null)
        {
           
            _instance = this;
            DontDestroyOnLoad(gameObject);
            _audioSource = GetComponent<AudioSource>();
        }
        else
        {
            //for going back to scene and not have big music mess
            Destroy(gameObject);
        }

    }

    private void OnEnable()
	{
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	private void OnDisable()
	{
		SceneManager.sceneLoaded -= OnSceneLoaded;
	}

	private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		if (scene.name == "MainMenu" || scene.name == "SimpleCombat" || scene.name == "CombatScene" || scene.name=="Combat Arena")
		{
			StopMusic();
		}
		else
		{
			PlayMusic();
		}
	}

	public void PlayMusic()
	{
		if (!_audioSource.isPlaying)
		{
			_audioSource.Play();
		}
	}

	public void StopMusic()
	{
		_audioSource.Stop();
	}
}
