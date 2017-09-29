using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityStandardAssets._2D{
	public class Radar : MonoBehaviour {

		public bool Commented = false;
		private GameObject pai;
		public int paiHash;

		void Start(){
			pai = this.transform.parent.gameObject;
		}

		//Controle do radar para rotacionar em direção ao alvo(Ball,Player)
		void OnTriggerStay2D(Collider2D col) {
			//só passar se for player ou bola
			/*if (!(col.tag == "Player" || col.tag == "Ball" ))
				return;*/

			if (!pai.GetComponent<Stone> ().wasFired()  || pai.GetComponent<Stone> ().onTheGround)
				return;

			if (col.tag == "Ball") {
				
				if (col.gameObject.GetComponent<Stone> () == null || col.gameObject.GetComponent<Stone> ().paiHash == paiHash)
					return;
			} else if (col.tag == "Player") {
				if (col.gameObject.GetComponent<Platformer2DUserControl> ().getHash() == paiHash)
					return;
			} else {
				return;
			}

			pai.GetComponent<Stone> ().rotacionar(col);
		}
	}
}
