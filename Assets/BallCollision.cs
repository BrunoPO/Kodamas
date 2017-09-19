using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallCollision : MonoBehaviour {

	Transform pai;
	void Start(){
		pai = transform.parent;
		//print (pai);
	}

	void OnTriggerEnter2D(Collider2D col) {
		print (col);
		print (col.tag);
		if (col.tag == "Player")
			Destroy (pai);
		else if (col.tag == "Respawn") {
			pai.GetComponent<Rigidbody2D>().gravityScale = 2;
			pai.GetComponent<Fire>().fired = false;
		}
	}
}
