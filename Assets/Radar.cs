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
			if (!(col.tag == "Player" || col.tag == "Ball" ) || col.gameObject.GetHashCode()!=paiHash)
				return;
			if (!pai.GetComponent<Stone> ().fired  || pai.GetComponent<Stone> ().onTheGround)
				return;
			pai.GetComponent<Stone> ().rotacionar(col);
		}
	}
}
