using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityStandardAssets._2D{
	public class Fire : MonoBehaviour {
		private Animator ani;
		private Rigidbody2D rigid;
		public bool fired = false,Commented=true,onTheGround=false;
		private float horin,vert,indexSeno=0.5f,peso=0;
		void Start(){
			ani = GetComponent<Animator>();
			rigid = GetComponent<Rigidbody2D>();
		}
		// Update is called once per frame
		void Update () {
			if(fired){
				trageto ();
			}

			//Controle sobre o limite do cenário para que as bolas vão até o infinito;
			if (transform.position.x > 45 || transform.position.x < -45 || transform.position.y < -35 || transform.position.y > 35)
				Destroy (this.gameObject);
		}

		private void trageto(){
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
			Debug.DrawRay(transform.position, col.transform.position-transform.position, Color.white);
			Vector3 dif;
			Quaternion r = transform.rotation;
			if (indexSeno < 10)
				indexSeno = 5;
			r.SetFromToRotation (transform.position, col.transform.position-transform.position);
			dif = (r.eulerAngles - transform.rotation.eulerAngles)/100;
			dif += transform.rotation.eulerAngles;
			transform.rotation = Quaternion.Euler(dif);
			if(Commented) print (col.transform.position);
		}

		public void Ball(float peso){
			print (peso);
			fired=true;
			this.gameObject.layer = 9;
			this.tag = "Ball";
			GameObject child = this.transform.GetChild (0).gameObject;
			child.layer = 9;
			child.tag = "Ball";
			child = this.transform.GetChild (1).gameObject;
			child.layer = 9;
			child.tag = "Ball";

			ani.SetBool("Fired",true);
			this.peso = peso;
		}
	}
}
