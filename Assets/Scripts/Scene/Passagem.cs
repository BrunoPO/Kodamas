using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityStandardAssets._2D{
	public class Passagem : MonoBehaviour {
		[SerializeField] private GameObject to;
		[SerializeField] private bool baixo, cima, esquerda, direita;
		void OnTriggerEnter2D(Collider2D col) {
			if (col.GetComponent<Rigidbody2D> () == null)//testa se o objeto que está sendo verificado é a raiz
				return;
			//print (col);
			if (col.tag == "Player") {
				Rigidbody2D rigid = col.GetComponent<Rigidbody2D> ();
				//print ("Pass Colide Y:"+rigid.velocity.y+"X:"+rigid.velocity.x);
				if (cima && rigid.velocity.y > 0) {
					col.transform.position = to.transform.position;
				} else if (baixo && rigid.velocity.y < 0) {
					col.transform.position = to.transform.position;
				} else if (esquerda && rigid.velocity.x < 0) {
					col.transform.position = to.transform.position;
				} else if (direita && rigid.velocity.x > 0) {
					col.transform.position = to.transform.position;
				}
			} else if (col.tag == "Stone" || col.tag == "Respawn") {
				if (col.GetComponent<Stone> () == null)
					return;
				float rotation = col.transform.rotation.eulerAngles.z;
				print ("Pass Colide rotation z"+rotation);
				if (cima && 45>rotation && rotation<=135) {
					col.transform.position = to.transform.position;
				} else if (baixo && 180>rotation && rotation<=225) {
					col.transform.position = to.transform.position;
				} else if (esquerda && 135>rotation && rotation<=180) {
					col.transform.position = to.transform.position;
				} else if (direita && (0>rotation && rotation<=45) || (225>rotation && rotation<=315)) {
					col.transform.position = to.transform.position;
				}
			}
		}
	}
}
