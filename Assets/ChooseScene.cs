using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ChooseScene : NetworkBehaviour {
	
	void Start(){
		if (!isServer)
			this.gameObject.SetActive (false);
	}
}
