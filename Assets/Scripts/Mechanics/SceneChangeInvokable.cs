﻿using System.Collections;
using System.Collections.Generic;
using Platformer.Core;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class SceneChangeInvokable : MonoBehaviour, Invokable
{
	[SerializeField] public Animator transitionAnim;
	public string sceneName;
	public bool IsDoor = true;
	public bool CanEnter;

	//Raw Image to Show Video Images [Assign from the Editor]
	public RawImage image;
	//Video To Play [Assign from the Editor]
	public VideoClip videoToPlay;

	private VideoPlayer videoPlayer;
	private VideoSource videoSource;

	IEnumerator ChangeScene()
	{
		//Add VideoPlayer to the GameObject
		videoPlayer = gameObject.AddComponent<VideoPlayer>();
		//Disable Play on Awake for Video
		videoPlayer.playOnAwake = false;
		videoPlayer.skipOnDrop = false;
		//We want to play from video clip not from url
		videoPlayer.source = VideoSource.VideoClip;
		videoPlayer.clip = videoToPlay;
		videoPlayer.Prepare();
		//Wait until video is prepared
		while (!videoPlayer.isPrepared)
		{
			yield return null;
		}
		//Assign the Texture from Video to RawImage to be displayed
		image.texture = videoPlayer.texture;
		videoPlayer.Play();
		while (videoPlayer.isPlaying)
		{
			yield return null;
		}

		Debug.Log("Changing Scene");

		var gm = GameManager.Instance;
		gm.PlayDoorSound[sceneName] = IsDoor;

		transitionAnim.SetTrigger("Start");
		yield return new WaitForSeconds(1);
		SceneManager.LoadScene(sceneName);
		Resources.UnloadUnusedAssets();
        transitionAnim.SetTrigger("End");
	}
	public void Exit()
	{
		Application.Quit();
	}

	public void Invoke()
	{
		Application.runInBackground = true;
		string currentScene = SceneManager.GetActiveScene().name;
		if (currentScene == "Main Scene 1" || currentScene == "Main Scene 2")
        {
			if (CanEnter)
            {
				StartCoroutine(ChangeScene());
			}
		}
        else
        {
			StartCoroutine(ChangeScene());
		}
	}
}