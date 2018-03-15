using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets._2D;

public class itemRadar : MonoBehaviour {

	void OnTriggerEnter2D(Collider2D col) {
		if(col.tag != "Player")
			return;
		if(col.name == "CeilingCheck" || col.name == "GroundCheck" || col.name == "Head" || col.name == "Target" )
			col.transform.parent.transform.gameObject.GetComponent<EnemyAI>().targetSec = this.transform.parent.transform;
		else
			col.transform.gameObject.GetComponent<EnemyAI>().targetSec = this.transform.parent.transform;

	}
}
