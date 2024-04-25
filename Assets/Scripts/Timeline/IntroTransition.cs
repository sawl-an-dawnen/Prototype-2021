using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.Video;
using UnityEngine.UI;

public class IntroTransition : MonoBehaviour
{
    //Raw Image to Show Video Images [Assign from the Editor]
    public RawImage image;
    //Video To Play [Assign from the Editor]
    public VideoClip videoToPlay;

    private VideoPlayer videoPlayer;

    private VideoSource videoSource;

    public string sceneToChange;
    // Method to be called when the animation is complete
    public void OnAnimationComplete()
    {
        // Load the next scene
        StartCoroutine(changeScene());
    }

    IEnumerator changeScene()
    {
        videoPlayer = gameObject.AddComponent<VideoPlayer>();
        videoPlayer.playOnAwake = false;
        videoPlayer.skipOnDrop = false;
        videoPlayer.source = VideoSource.VideoClip;
        videoPlayer.clip = videoToPlay;
        videoPlayer.Prepare();
        while (!videoPlayer.isPrepared)
        {
            yield return null;
        }
        image.texture = videoPlayer.texture;
        videoPlayer.Play();
        //Debug.Log("intro transition out");

        yield return new WaitForSeconds(1.35f);
        SceneManager.LoadScene(sceneToChange);
    }
}
