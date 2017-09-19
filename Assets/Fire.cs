using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : MonoBehaviour {
	private Animator ani;
	private Rigidbody2D rigid;
	public bool fired = false,Commented=true,onTheGround=false;
	private float horin,vert,indexSeno,peso;
	private int groundTime;
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

	//Detecção de colisões entre a Bola e Pesonagem ou outra bolsa
	void OnCollisionEnter2D(Collision2D coll) {
		print ("Colidiu This:"+this.name+" tag:"+this.tag+", Com:"+coll.collider.name+" Tag:"+coll.collider.tag);
		if (coll.gameObject.tag == "Player") {
			if (onTheGround)
				print ("Player.Balls++");
			else
				print("Player Kill");
			Destroy (this.gameObject);
		}else if(coll.gameObject.tag == "Ball" || coll.gameObject.tag == "Ground"){
			onTheGround = true;
			this.tag = "Respawn";
			rigid.gravityScale = 10;
		}
	}

	//Controle da bola depois dela já estar no chão
	void OnCollisionStay2D(Collision2D coll){
		if (onTheGround && coll.collider.tag == "Ground" && groundTime > 10) {
			if (rigid.gravityScale != 0.2f)
				rigid.gravityScale = 0.2f;
			transform.rotation = Quaternion.Euler (new Vector3 (0, 0, 0));
			transform.position += transform.up * peso / 10;
		} else {
			groundTime++;
		}
	}

	//Controle do radar para rotacionar em direção ao alvo(Ball,Player)
	void OnTriggerStay2D(Collider2D col) {
		if (!(col.tag == "Player" || col.tag == "Ball"))
			return;
		if (!fired  || onTheGround)
			return;
		
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

	private void trageto(){
		//print ("i:"+i+"seno:"+Mathf.Sin(Mathf.PI/i));
		float velo = Mathf.Sin(Mathf.PI/indexSeno);
		if (peso != 0 && !onTheGround) {
			transform.position += transform.right * velo * peso / 10;
		} /*else if (onTheGround) {
			print("Not going any where");
		}*/
		if(indexSeno<10)
			indexSeno += 0.5f;
	}

	public void Ball(float peso){
		fired=true;
		ani.SetBool("Fired",true);
		this.peso = peso;
	}
}
