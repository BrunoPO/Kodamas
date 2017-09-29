﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityStandardAssets._2D{
	public class BallCollision : MonoBehaviour {
		public bool Commented = false;
		private Rigidbody2D rigid;
		private int groundTime=0;
		private GameObject pai;
		private bool end = false;
		public int paiHash=0;
		void Start(){
			pai = this.transform.parent.gameObject;
		}
		//Detecção de colisões entre a Bola e Pesonagem ou outra bolsa
		void OnTriggerEnter2D(Collider2D coll) {
			if(coll.tag == "Player")
				print ("Colidiu This:"+this.name+" tag:"+this.tag+"FatherID:"+paiHash+", Com:"+coll.name+" Tag:"+coll.tag+"Hash:"+coll.GetComponent<Platformer2DUserControl> ().getHash());
			else
				print ("Colidiu This:"+this.name+" tag:"+this.tag+"FatherID:"+paiHash+", Com:"+coll.name+" Tag:"+coll.tag);

			if (end)
				return;
			if (coll.tag == "Player") {
				if (pai.GetComponent<Stone> ().onTheGround) {//Balls++ Ball on the ground
					end = coll.gameObject.GetComponent<Platformer2DUserControl> ().gainBall();
					if (end) {
						pai.GetComponent<Stone> ().DestroySelf ();
					}
				} else if (coll.GetComponent<Platformer2DUserControl> ().getHash() != paiHash) {
					print ("Player Kill");
					coll.GetComponent<Platformer2DUserControl> ().Killed();
					pai.GetComponent<Stone> ().DestroySelf ();
				} else {//Balls++ Ball on the air
					end = coll.GetComponent<Platformer2DUserControl> ().gainBall();
					if (end) {
						pai.GetComponent<Stone> ().DestroySelf ();
					}
				}
			}else if(pai.tag != "Respawn" ){
				if (coll.gameObject.tag == "Ball") {
					/*if (coll.GetComponent<StoneNet> () == null) {//Código para debug de colisão se bola inimiga não tiver script
						Fall ();
					}else */
					if (coll == null || coll.GetComponent<Stone>() == null)
						return;
					if (coll.GetComponent<Stone> ().paiHash != paiHash) {//2 Stones adversárias se batem
						Fall ();
					}
				}else if(coll.gameObject.tag == "Ground" || coll.gameObject.tag == "Wall"){//Stone acerta o chão
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

		void OnTriggerStay2D(Collider2D coll){
			//print(coll.tag+" "+groundTime+" "+pai.GetComponent<Stone> ().onTheGround);
			if (coll.tag == "Ground" && groundTime > 10 && pai.GetComponent<Stone> ().onTheGround) {
				pai.GetComponent<Stone> ().flutuarNoChao();
			} else if(groundTime<11){
				groundTime++;
			}
		}
	}
}
