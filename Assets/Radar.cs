using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Radar : MonoBehaviour {
	Transform pai;
	void Start(){
		pai = transform.parent;
		//print (pai);
	}

	void OnTriggerStay2D(Collider2D col) {
		if (!(col.tag == "Player" || col.tag == "Respawn"))
			return;
		if (!pai.GetComponent<Fire>().fired)
			return;
		Debug.DrawRay(pai.transform.position, col.transform.position-pai.transform.position, Color.white);
		Vector3 dif;
		Quaternion r = pai.transform.rotation;
		r.SetFromToRotation (pai.transform.position, col.transform.position-pai.transform.position);
		dif = (r.eulerAngles - pai.transform.rotation.eulerAngles)/50;
		dif += pai.transform.rotation.eulerAngles;
		pai.transform.rotation = Quaternion.Euler(dif);
		//print (col.transform.position);
	}
}
