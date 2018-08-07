using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Prototype.NetworkLobby;

public class ExibirIP : MonoBehaviour {
	string meuip;
	void Start() {
		AlterIp();
	}
	private void AlterIp(){
		
		GameObject Lobby = GameObject.Find("LobbyManager");
		if(Lobby == null){
			return;
		}
		meuip = Lobby.GetComponent<LobbyManager>().getIP();
		if (meuip != "0.0.0.0" && meuip != "") {
			transform.GetChild (2).gameObject.SetActive(true);
			string converted = Convert.IPToHash(meuip);
			transform.GetChild(0).GetChild(0).gameObject.GetComponent<Text>().text = converted;
			print(Convert.HashToIP(converted));
			transform.GetChild (2).gameObject.GetComponent<QRCodeGnrtTexture> ().generateQR (converted);
			this.enabled = false;
		}
	}
	void Update(){
		AlterIp();
    }
}
