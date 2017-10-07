﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ExibirIP : NetworkBehaviour {
	void Start () {
		this.GetComponent<Text>().text = Network.player.ipAddress;
	}
	void Update(){
		if(this.GetComponent<Text>().text == "")
			this.GetComponent<Text>().text = Network.player.ipAddress;
	}
}