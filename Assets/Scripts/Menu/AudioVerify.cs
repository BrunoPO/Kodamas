using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prototype.NetworkLobby;

public class AudioVerify : MonoBehaviour {

	public GameObject TopPanel;
	LobbyTopPanel myPanel;
	AudioSource mAudio;
	// Use this for initialization
	void Start () {
		TopPanel = GameObject.Find ("TopPanel");
		mAudio = this.GetComponent<AudioSource> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (TopPanel != null) {
			if(myPanel == null)
				myPanel = TopPanel.GetComponent<LobbyTopPanel> ();
			if(myPanel != null){
				if (myPanel.isInGame && mAudio.isPlaying)
					mAudio.Stop ();
				else if (!myPanel.isInGame && !mAudio.isPlaying)
					mAudio.Play ();
					
			}
		}
		
	}
}
