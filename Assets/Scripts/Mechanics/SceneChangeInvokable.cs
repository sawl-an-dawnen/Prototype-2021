using System.Collections;
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
		Time.timeScale = 0;
		if (SceneManager.GetActiveScene().name != "Combat Arena")
		{
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
		}

		Debug.Log("Changing Scene");

		var gm = GameManager.Instance;
		gm.PlayDoorSound[sceneName] = IsDoor;

		if (SceneManager.GetActiveScene().name != "Combat Arena")
		{
			DontDestroyOnLoad(videoPlayer);
		}

		yield return new WaitForSecondsRealtime(1.5f);
		Resources.UnloadUnusedAssets();
		AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
		Time.timeScale = 1;
		/*
		if (SceneManager.GetActiveScene().name != "Combat Arena")
		{
			Destroy(videoPlayer, 6);
		}
		*/
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