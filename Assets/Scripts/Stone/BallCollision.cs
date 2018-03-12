using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityStandardAssets._2D{
	public class BallCollision : MonoBehaviour {
		[HideInInspector] public int paiHash=0;
		[HideInInspector] public int hashTeam=0;

		[SerializeField] private bool Commented = false;
		private Rigidbody2D rigid;
		private int groundTime=0;
		private GameObject pai;
		private bool end = false;
		private bool isTeamParty = false;
		private float timeToCanCollide = 1f;
		private float counter_CanCollide = 0;

		void Start(){
			pai = this.transform.parent.gameObject;
			isTeamParty = pai.GetComponent<Stone>().isTeamParty();
		}

		void Update()
		{
			counter_CanCollide += Time.deltaTime; 
		}


		//Detecção de colisões entre a Bola e Pesonagem ou outra bolsa
		void OnTriggerEnter2D(Collider2D col) {
			if (pai == null)
				return;
			if(col.tag == "Player")
				if(Commented) print ("Colidiu This:"+this.name+" tag:"+this.tag+"FatherID:"+paiHash+", Com:"+col.name+" Tag:"+col.tag+"Hash:"+col.GetComponent<PlatformerCharacter2D> ().getHash());
			else if(col.tag == "Stone")
				if(Commented) print ("Colidiu This:"+this.name+" tag:"+this.tag+"FatherID:"+paiHash+", Com:"+col.name+" Tag:"+col.tag);

			if (end)
				return;
			if (col.tag == "Player") {
				if (col.gameObject.GetComponent<Platformer2DUserControl> () == null)//se não possuir script ativo então volte
					return;
				
				if(Commented) print (col.name);
				
				bool itsAEnemy = false;

				if(isTeamParty){
					itsAEnemy = (col.GetComponent<PlatformerCharacter2D> ().getTeam() != hashTeam);
				}else{
					itsAEnemy = (col.GetComponent<PlatformerCharacter2D> ().getHash() != paiHash);
				}
				

				if (pai.GetComponent<Stone> ().onTheGround) {//Balls++ Ball on the ground
					end = col.gameObject.GetComponent<Platformer2DUserControl> ().gainBall();
					if (end) {
						pai.GetComponent<Stone> ().DestroySelf ();
					}
				} else if (itsAEnemy) {
					if(Commented) print ("Player Killed");
					col.GetComponent<PlatformerCharacter2D> ().Killed();
					pai.GetComponent<Stone> ().Killed (col.GetComponent<PlatformerCharacter2D> ().getHash ());
                    pai.GetComponent<Stone>().collisionDetected();//Mostra animação de colisão
                    pai.GetComponent<Stone> ().DestroySelf ();
					//Fall ();
				} else if(timeToCanCollide < counter_CanCollide){//Balls++ Ball on the air
					end = col.GetComponent<Platformer2DUserControl> ().gainBall();
					if (end) {
						pai.GetComponent<Stone> ().DestroySelf ();
					}
				}

			}else if(pai.tag != "Respawn" ){//Se for igual a stone
				if (col.gameObject.tag == "Stone") {
					if (col == null || col.GetComponent<Stone>() == null)
						return;
					if (col.GetComponent<Stone> ().paiHash != paiHash) {//2 Stones adversárias se batem então as duas começam a cair
						col.GetComponent<Stone> ().GetComponentInChildren<BallCollision>().Fall();
						Fall ();
					}
				}else if(col.gameObject.tag == "Ground" || col.gameObject.tag == "Wall"){//Stone acerta o chão então cai
					Fall ();
				}
			}
		}

		public void Fall(){
            pai.GetComponent<Stone>().collisionDetected();
            pai.GetComponent<Stone> ().onTheGround = true;
			pai.tag = "Respawn";
			pai.layer = 12;
			this.tag = "Respawn";
			this.gameObject.layer = 13;
			pai.transform.rotation = Quaternion.Euler (new Vector3 (0, 0, 0));
			pai.GetComponent<Rigidbody2D>().gravityScale = 5;
		}

		void OnTriggerStay2D(Collider2D col){
			if (!this.enabled)
				return;
			if (col.tag == "Ground" && groundTime > 10 && pai.GetComponent<Stone> ().onTheGround) {
				pai.GetComponent<Stone> ().flutuarNoChao();
			} else if(groundTime<11){
				groundTime++;
			}
		}
	}
}
