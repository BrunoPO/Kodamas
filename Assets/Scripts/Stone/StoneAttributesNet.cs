using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace UnityStandardAssets._2D{
	public class StoneAttributesNet : NetworkBehaviour {
		
		private Rigidbody2D rigid;
		[SyncVar]
		public bool kill=false;
		[SyncVar]
		public bool fired = false;

		void Start(){
			rigid = GetComponent<Rigidbody2D>();
		}


		public void SetFired(bool b){
			if(!isServer)
				fired = b;
			CmdSetFired (b);
		}

		[Command]
		public void CmdSetFired(bool b){
			fired = b;
		}
		// Update is called once per frame
		[ServerCallback]
		void Update () {
			if (isServer && kill) {
				NetworkServer.Destroy (gameObject);
			}
			if(fired){
				GetComponent<Stone> ().trageto ();
			}
			Vector2 velo = rigid.velocity;
			//if (velo.y!=0) print (velo.y);
			if(velo.y<-19.0f){
				velo.y = -15.0f;
				rigid.velocity = velo;
			}
			//Controle sobre o limite do cenário para que as bolas vão até o infinito;
			if (transform.position.x > 45 || transform.position.x < -45 || transform.position.y < -35 || transform.position.y > 35)
				NetworkServer.Destroy (gameObject);
		}
	}
}
