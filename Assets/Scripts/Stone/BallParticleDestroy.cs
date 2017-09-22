using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallParticleDestroy : MonoBehaviour {

	Animator ani;
	// Use this for initialization
	void Start () {
		ani = this.GetComponent<Animator> ();
	}

	void Update () {
		if (ani.GetBool ("Ended")) {
			Destroy (this.gameObject);
		}
	}
}
