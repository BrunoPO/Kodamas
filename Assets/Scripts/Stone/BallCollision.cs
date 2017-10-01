﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityStandardAssets._2D{
	public class BallCollision : MonoBehaviour {
		[SerializeField] private bool Commented = false;
		private Rigidbody2D rigid;
		private int groundTime=0;
		private GameObject pai;
		private bool end = false;
		public int paiHash=0;
		void Start(){
			pai = this.transform.parent.gameObject;
		}
		//Detecção de colisões entre a Bola e Pesonagem ou outra bolsa
		void OnTriggerEnter2D(Collider2D col) {
			if(col.tag == "Player")
				if(Commented) print ("Colidiu This:"+this.name+" tag:"+this.tag+"FatherID:"+paiHash+", Com:"+col.name+" Tag:"+col.tag+"Hash:"+col.GetComponent<Platformer2DUserControl> ().getHash());
			else if(col.tag == "Stone")
				if(Commented) print ("Colidiu This:"+this.name+" tag:"+this.tag+"FatherID:"+paiHash+", Com:"+col.name+" Tag:"+col.tag);

			if (end)
				return;
			if (col.tag == "Player") {
				if (pai.GetComponent<Stone> ().onTheGround) {//Balls++ Ball on the ground
					end = col.gameObject.GetComponent<Platformer2DUserControl> ().gainBall();
					if (end) {
						pai.GetComponent<Stone> ().DestroySelf ();
					}
				} else if (col.GetComponent<Platformer2DUserControl> ().getHash() != paiHash) {
					if(Commented) print ("Player Killed");
					col.GetComponent<Platformer2DUserControl> ().Killed();
					pai.GetComponent<Stone> ().DestroySelf ();
				} else {//Balls++ Ball on the air
					end = col.GetComponent<Platformer2DUserControl> ().gainBall();
					if (end) {
						pai.GetComponent<Stone> ().DestroySelf ();
					}
				}
			}else if(pai.tag != "Respawn" ){
				if (col.gameObject.tag == "Stone") {
					if (col == null || col.GetComponent<Stone>() == null)
						return;
					if (col.GetComponent<Stone> ().paiHash != paiHash) {//2 Stones adversárias se batem
						Fall ();
					}
				}else if(col.gameObject.tag == "Ground" || col.gameObject.tag == "Wall"){//Stone acerta o chão
					Fall ();
				}
			}
		}

		void Fall(){
			pai.GetComponent<Stone> ().onTheGround = true;
			pai.tag = "Respawn";
			pai.layer = 12;
			this.tag = "Respawn";
			this.gameObject.layer = 13;
			pai.transform.rotation = Quaternion.Euler (new Vector3 (0, 0, 0));
			pai.GetComponent<Rigidbody2D>().gravityScale = 5;
		}

		void OnTriggerStay2D(Collider2D col){
			//print(coll.tag+" "+groundTime+" "+pai.GetComponent<Stone> ().onTheGround);
			if (col.tag == "Ground" && groundTime > 10 && pai.GetComponent<Stone> ().onTheGround) {
				pai.GetComponent<Stone> ().flutuarNoChao();
			} else if(groundTime<11){
				groundTime++;
			}
		}
	}
}
