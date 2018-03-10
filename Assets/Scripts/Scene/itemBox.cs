using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityStandardAssets._2D;

public class itemBox : NetworkBehaviour {
	//[HideInInspector] 
	public int itemType=0;
	[SerializeField] private bool Commented = false;
	private bool end = false;

	//Detecção de colisões entre a Bola e Pesonagem ou outra bolsa
	void OnTriggerEnter2D(Collider2D col) {
			
		if (end == true || col.tag != "Player" || col.gameObject.GetComponent<PlatformerCharacter2D> () == null)
			return;
		end = true;

		col.gameObject.GetComponent<PlatformerCharacter2D> ().improvePlayer(itemType);
		NetworkServer.Destroy (gameObject);
	}
}
