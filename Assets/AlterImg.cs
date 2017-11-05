using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Prototype.NetworkLobby;

public class AlterImg : MonoBehaviour {	
	public Sprite[] imgs;
	private int imgAtual = 0;

	public void Alter(int i){
		if(i < imgs.Length){
			gameObject.GetComponent<Image> ().sprite = imgs[i];
		}
	}

	public void Avancar(){
		if (imgAtual < imgs.Length-1) {
			imgAtual++;
			Alter (imgAtual);
		}
	}

	public void Voltar(){
		if (imgAtual > 0) {
			imgAtual--;
			Alter (imgAtual);
		}
	}

	public void confirmarCena(){
		GameObject obj = GameObject.Find ("LobbyManager");
		obj.GetComponent<LobbyManager>().ChangeScene(imgAtual);
	}
}
