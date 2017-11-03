using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ExibirIP : NetworkBehaviour {
	string meuip;
	void Start() {
		meuip = Network.player.ipAddress;
	}
	void Update(){
		meuip = Network.player.ipAddress;
		if (meuip != "0.0.0.0") {
            //print (transform.GetChild (0));
                string converted = Convert.IPToHash(meuip);
                transform.GetChild(0).GetChild(0).gameObject.GetComponent<Text>().text = converted;
                print(Convert.HashToIP(converted));

                this.enabled = false;
		}
    }
}
