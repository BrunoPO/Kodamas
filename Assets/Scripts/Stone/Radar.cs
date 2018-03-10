using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityStandardAssets._2D{
	public class Radar : MonoBehaviour {
		private GameObject pai;
		[HideInInspector] public int paiHash;
		[HideInInspector] public int hashTeam=0;
		
		void Start(){
			pai = this.transform.parent.gameObject;
		}

		//Controle do radar para rotacionar em direção ao alvo(Ball,Player)
		void OnTriggerStay2D(Collider2D col) {
			if (!this.enabled)
				return;
			else if (!pai.GetComponent<Stone> ().wasFired()  || pai.GetComponent<Stone> ().onTheGround)
				return;
			else if (col.tag == "Stone") {
				if (col.gameObject.GetComponent<Stone> () == null || col.gameObject.GetComponent<Stone> ().paiHash == paiHash)
					return;
			} else if (col.tag == "Player") {
				if (col.gameObject.GetComponent<Platformer2DUserControl> () == null) {
					if (col.transform.parent.gameObject.GetComponent<Platformer2DUserControl> () == null)
						return;
				} else {
					if (col.gameObject.GetComponent<PlatformerCharacter2D> ().getHash () == paiHash)
						return;
				}
			} else {
				return;
			}

			pai.GetComponent<Stone> ().rotacionar(col);
		}
	}
}
