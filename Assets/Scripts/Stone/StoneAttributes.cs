using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityStandardAssets._2D{
	public class StoneAttributes : MonoBehaviour {
		private Rigidbody2D rigid;
		public bool kill=false;
		public bool fired = false;

		void Start(){
			rigid = GetComponent<Rigidbody2D>();
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
			Vector2 velo = rigid.velocity;
			//if (velo.y!=0) print (velo.y);
			if(velo.y<-16.0f){
				velo.y = -15.0f;
				rigid.velocity = velo;
			}
			//Controle sobre o limite do cenário para que as bolas vão até o infinito;
			if (transform.position.x > 45 || transform.position.x < -45 || transform.position.y < -35 || transform.position.y > 35)
				Destroy (gameObject);
		}
	}
}
