using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityStandardAssets._2D{
	public class Stone : MonoBehaviour {
		private Rigidbody2D rigid;
		public bool Commented=true,onTheGround = false;
		private float horin,vert,indexSeno=0.5f,peso=0;
		public GameObject effect;
		public int paiHash=0;
		private StoneAttributesNet m_attributesNet;
		private StoneAttributes m_attributes;
		private bool isNet ;

		private void Start(){
			rigid = GetComponent<Rigidbody2D> ();
		}

		public bool wasFired(){
			if (isNet) {
				return m_attributesNet.fired;
			} else {
				return m_attributes.fired;
			}
		}

		private void setFired(bool b){
			if (isNet) {
				m_attributesNet.SetFired(b);
			} else {
				m_attributes.SetFired(b);
			}
		}
		private void setKill(bool k){
			if (isNet) {
				m_attributesNet.kill = k;
			} else {
				m_attributes.kill = k;
			}
		}

		public void trageto(){
			float velo = Mathf.Sin(Mathf.PI/indexSeno);
			if (peso != 0 && !onTheGround) {
				transform.position += transform.right * velo * peso / 10;
			}
			if(indexSeno<10)
				indexSeno += 0.5f;
		}

		//Controle da bola depois dela já estar no chão
		public void flutuarNoChao(){
			//print("Tentando Flutuar"+transform.up+peso);
			if (rigid.gravityScale != 0.2f)
				rigid.gravityScale = 0.2f;
			transform.rotation = Quaternion.Euler (new Vector3 (0, 0, 0));
			rigid.AddForce (transform.up * peso * 10);
			//transform.position += transform.up * peso /2;
		}

		public void rotacionar(Collider2D col){
			//Debug para identificar quais inimigos foram encontrados;
			Debug.DrawRay(transform.position, col.transform.position, Color.white);
			Vector3 dif;
			Quaternion r = transform.rotation;
			if (indexSeno < 10)
				indexSeno = 5;
			r.SetFromToRotation (transform.position, col.transform.position-transform.position);
			dif = ( transform.rotation.eulerAngles-r.eulerAngles)/500;
			dif += transform.rotation.eulerAngles;
			transform.rotation = Quaternion.Euler(dif);
			if(Commented) print (col.transform.position);
		}

		public void Fire(float peso,int Hash){

			print ("----------Attributes----------");
			print (GetComponent<StoneAttributesNet> ());
			print (GetComponent<StoneAttributes> ());
			print ("-----------------------");

			print ("Fired" + m_attributesNet);

			if (GetComponent<StoneAttributesNet> () != null) {
				if(Commented) print("Online");
				m_attributesNet = GetComponent<StoneAttributesNet> ();
				isNet = true;
			}else if(GetComponent<StoneAttributes> () != null){
				if(Commented) print("Offline");
				m_attributes = GetComponent<StoneAttributes> ();
				isNet = false;
			}

			print ("Fired2" + m_attributesNet);
			//print(" "+paiHash+" "+Hash);
			this.paiHash = Hash;
			setFired(true);
			//Configurando Ball para interagir com o ambiente
			this.gameObject.layer = 9;
			this.tag = "Stone";

			GameObject child = this.transform.GetChild (0).gameObject;
			child.GetComponent<Radar>().paiHash = Hash;
			child.GetComponent<Radar> ().enabled = true;
			child.layer = 9;
			child.tag = "Stone";

			child = this.transform.GetChild (1).gameObject;
			child.GetComponent<BallCollision>().paiHash = Hash;
			child.GetComponent<BallCollision> ().enabled = true;
			child.layer = 9;
			child.tag = "Stone";


			GetComponent<Animator>().SetBool("Fired",true);//Solved Bug
			//Bug Prefab isn't enable at begin so Start wouln't run and because of that ani wouln't inicialize
			this.peso = peso;

			if (isNet) {
				m_attributesNet = GetComponent<StoneAttributesNet> ();
			} else {
				m_attributes = GetComponent<StoneAttributes> ();
			}
		}


		public void DestroySelf(){
			setKill (true);
		}
	}
}
