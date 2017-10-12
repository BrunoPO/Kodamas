using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BallParticleDestroyNet : NetworkBehaviour {

	Animator ani;
	void Start () {
		ani = this.GetComponent<Animator> ();
	}

	[ServerCallback]
	void Update () {
		if (ani.GetBool ("Ended")) {
			NetworkServer.Destroy (this.gameObject);
		}
	}
}
