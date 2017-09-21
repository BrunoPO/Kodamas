using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityStandardAssets._2D{
public class BallCollision : MonoBehaviour {
		public bool Commented = false;
		private Rigidbody2D rigid;
		private int groundTime=0;
		private GameObject pai;
		private bool end = false;
		void Start(){
			pai = this.transform.parent.gameObject;
		}
		//Detecção de colisões entre a Bola e Pesonagem ou outra bolsa
		void OnTriggerEnter2D(Collider2D coll) {
			if(coll.tag != "Ground")
				print ("Colidiu This:"+this.name+" tag:"+this.tag+", Com:"+coll.name+" Tag:"+coll.tag);
			if (end)
				return;
			if (coll.tag == "Player") {
				if (pai.GetComponent<Fire> ().onTheGround) {
					end = coll.GetComponent<Platformer2DUserControl>().getBall ();
					if (end) {
						Destroy (pai.gameObject);
						//print ("Player.Balls++");
					}
				} else {
					print ("Player Kill");
					Destroy (pai.gameObject);
				}
			}else if(pai.tag != "Respawn" && (coll.gameObject.tag == "Ball" || coll.gameObject.tag == "Ground")){
				pai.GetComponent<Fire> ().onTheGround = true;
				pai.tag = "Respawn";
				pai.layer = 12;
				this.tag = "Respawn";
				this.gameObject.layer = 13;
				pai.transform.rotation = Quaternion.Euler (new Vector3 (0, 0, 0));
				pai.GetComponent<Rigidbody2D>().gravityScale = 5;
			}
		}


		void OnTriggerStay2D(Collider2D coll){
			//print(coll.tag+" "+groundTime+" "+pai.GetComponent<Fire> ().onTheGround);
			if (coll.tag == "Ground" && groundTime > 10 && pai.GetComponent<Fire> ().onTheGround) {
				pai.GetComponent<Fire> ().flutuarNoChao();
			} else if(groundTime<11){
				groundTime++;
			}
		}
	}
}
