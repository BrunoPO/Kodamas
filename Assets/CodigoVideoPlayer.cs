using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class CodigoVideoPlayer : MonoBehaviour {

    public RawImage image;
    public VideoClip videoToPlay;
    public GameObject playIcon;
    public VideoPlayer videoPlayer;

    private bool isPaused = false;
    private bool firstRun = true;

    IEnumerator playVideo(){
        playIcon.SetActive(false);
        firstRun = false;

        videoPlayer.source = VideoSource.VideoClip;
        videoPlayer.clip = videoToPlay;
        videoPlayer.Prepare();

        //Wait until video is prepared
        while (!videoPlayer.isPrepared){
            yield return null;
        }

        //Debug.Log("Done Preparing Video");

        //Assign the Texture from Video to RawImage to be displayed
        image.texture = videoPlayer.texture;

        //Play Video
        videoPlayer.Play();

        //Debug.Log("Playing Video");
        while (videoPlayer.isPlaying){
            //Debug.LogWarning("Video Time: " + Mathf.FloorToInt((float)videoPlayer.time));
            yield return null;
        }

        if (!videoPlayer.isPlaying) {
            playIcon.SetActive(true);
        }

        //Debug.Log("Done Playing Video");
    }

    public void PlayPause() {
        Debug.Log("Apertou!");
        if (!firstRun && !isPaused){
            videoPlayer.Pause();
            playIcon.SetActive(true);
            isPaused = true;
        }else if (!firstRun && isPaused){
            videoPlayer.Play();
            playIcon.SetActive(false);
            isPaused = false;
        }else {
            StartCoroutine(playVideo());
        }
    }
}
