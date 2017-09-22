using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace UnityStandardAssets._2D{
	public class RadarNet : NetworkBehaviour {
		
		public bool Commented = false;
		private GameObject pai;
		public int paiHash;

		void Start(){
			pai = this.transform.parent.gameObject;
		}

		//Controle do radar para rotacionar em direção ao alvo(Ball,Player)
		void OnTriggerStay2D(Collider2D col) {
			//só passar se for player ou bola
			if (!(col.tag == "Player" || col.tag == "Ball" ))
				return;
			if (!pai.GetComponent<StoneNet> ().fired  || pai.GetComponent<StoneNet> ().onTheGround)
				return;

			if(col.gameObject.GetComponent<StoneNet> () != null){
				if (col.gameObject.GetComponent<NetworkIdentity>().netId.GetHashCode () == paiHash)
					return;
			}

			pai.GetComponent<StoneNet> ().rotacionar(col);
		}
	}
}
