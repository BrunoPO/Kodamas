using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityStandardAssets._2D{
	public class Stone : MonoBehaviour {
		private Rigidbody2D rigid;
		public bool Commented=false,onTheGround = false;
		private float horin,vert,indexSeno=0.5f,intecity=0;
		public GameObject effect;
		public int paiHash=0;
		private StoneAttributesNet m_attributesNet;
		private bool isNet ;
		public bool teamParty = false;
		private int hashTeam = 0;

		private void Start(){
			rigid = GetComponent<Rigidbody2D> ();
			teamParty = GameObject.Find("GM").GetComponent<GMNet>().isTeamParty();
		}

		public bool isTeamParty(){
			return teamParty;
		}

		private void Update(){
			Vector3 velo = rigid.velocity;
			if(velo.y<-15.0f){
				velo.y = -10.0f;
				rigid.velocity = velo;
			}
		}

		public bool wasFired(){
			return m_attributesNet.fired;
		}

		private void setFired(bool b){
			m_attributesNet.SetFired(b);
		}

		private void setKill(bool k){
			m_attributesNet.kill = k;
		}

		public void Killed(int enemyHash){
			m_attributesNet.Killed(paiHash, enemyHash);
		}

        public void collisionDetected()
        {
            if (isNet)
            {
                m_attributesNet.CmdAniCollision();
            }
            else
            {
                //m_attributes.CmdAniCollision();
            }
        }

        public void trageto(){
			float velo = Mathf.Sin(Mathf.PI/indexSeno);
			if (intecity != 0 && !onTheGround) {
				transform.position += transform.right * velo * intecity / 10;
			}
			if(indexSeno<10)
				indexSeno += 0.5f;
		}

		//Controle da bola depois dela já estar no chão
		public void flutuarNoChao(){
            if (rigid.gravityScale != 0.2f)
				rigid.gravityScale = 0.2f;
			transform.rotation = Quaternion.Euler (new Vector3 (0, 0, 0));
			rigid.AddForce (transform.up * intecity * 10);
		}

		public void rotacionar(Collider2D col){
			//Debug para identificar quais inimigos foram encontrados;
			Debug.DrawRay(transform.position, transform.position-col.transform.position, Color.white);
			Vector3 dif;
			Vector3 r = transform.rotation.eulerAngles;
			float alter = 0;
			if (r.z > 90 || r.z < 270) {
				if (col.transform.position.y > transform.position.y) {
					alter = -0.3f;
				} else if (col.transform.position.y < transform.position.y) {
					alter = 0.3f;
				}
			} else {
				if (col.transform.position.y > transform.position.y) {
					alter = 0.3f;
				} else if (col.transform.position.y < transform.position.y) {
					alter = -0.3f;
				}
			}
			r.z += alter;
			if(r.z < 0){
				r.z = 360-r.z;
			}
			transform.rotation = Quaternion.Euler (r);
		}

		void alterConfig(int stoneType){
			switch (stoneType){
				case 1://Mais rapido
					this.intecity *= 1.5f;
					break;
				case 2://Maior
					Vector3 scale = transform.localScale;
					scale.x *= 1.3f;
					scale.y *= 1.3f;
					transform.localScale = scale;
					this.intecity *= 0.8f;
					break;
				case 3://Aumenta o radar
					this.intecity *= 1.5f;
					break;
				case 4:
					this.intecity *= 1.5f;
					break;
			}
		}

		public void Fire(int stoneType,int hash,int hashTeam){

			if(Commented) print ("Fired");

			if(Commented) print("Online -- Fire");
			m_attributesNet = GetComponent<StoneAttributesNet> ();
			isNet = true;

			this.paiHash = hash;
			this.hashTeam = hashTeam;
			setFired(true);

			//Configurando Ball para interagir com o ambiente
			this.gameObject.layer = 9;
			this.tag = "Stone";

			GameObject child = this.transform.GetChild (0).gameObject;
			child.GetComponent<Radar>().paiHash = hashTeam;
			child.GetComponent<Radar>().paiHash = hash;
			child.GetComponent<Radar> ().enabled = true;
			child.layer = 9;
			child.tag = "Stone";

			child = this.transform.GetChild (1).gameObject;
			child.GetComponent<BallCollision>().paiHash = hashTeam;
			child.GetComponent<BallCollision>().paiHash = hash;
			child.GetComponent<BallCollision> ().enabled = true;
			child.layer = 9;
			child.tag = "Stone";


			GetComponent<Animator>().SetBool("Fired",true);//Solved Bug
			//Bug Prefab isn't enable at begin so Start wouln't run and because of that ani wouln't inicialize

			this.intecity = 5;
			m_attributesNet = GetComponent<StoneAttributesNet> ();
			alterConfig(stoneType);
		}


		public void DestroySelf(){
            setKill (true);
		}
	}
}
