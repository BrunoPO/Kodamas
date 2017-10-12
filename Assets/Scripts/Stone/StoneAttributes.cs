using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityStandardAssets._2D{
	public class StoneAttributes : MonoBehaviour {
		[HideInInspector] public bool kill=false;
		[HideInInspector] public bool fired = false;

		void Start(){
		}

		public void SetFired(bool b){
			CmdSetFired (b);
		}

		public void CmdSetFired(bool b){
			fired = b;
		}

		void Update () {
			if (kill) {
				Destroy (gameObject);
			}
			if(fired){
				GetComponent<Stone> ().trageto ();
			}
			//Controle sobre o limite do cenário para que as bolas vão até o infinito;
			if (transform.position.x > 45 || transform.position.x < -45 || transform.position.y < -35 || transform.position.y > 35)
				Destroy (gameObject);
		}
	}
}
