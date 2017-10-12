using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityStandardAssets._2D{
	public class Passagem : MonoBehaviour {
		
		[SerializeField] private GameObject to;
		[SerializeField] private bool baixo, cima, esquerda, direita;

		void OnTriggerEnter2D(Collider2D col) {
			//testa se o objeto que está sendo verificado é a raiz
			if (col.GetComponent<Rigidbody2D> () == null)
				return;
			
			if (col.tag == "Player") {
				Rigidbody2D rigid = col.GetComponent<Rigidbody2D> ();
				if (cima && rigid.velocity.y > 0) {//Cima
					col.transform.position = to.transform.position;
				} else if (baixo && rigid.velocity.y < 0) {//Baixo
					col.transform.position = to.transform.position;
				} else if (esquerda && rigid.velocity.x < 0) {//Esquerda
					col.transform.position = to.transform.position;
				} else if (direita && rigid.velocity.x > 0) {//Direita
					col.transform.position = to.transform.position;
				}
			} else if (col.tag == "Stone" || col.tag == "Respawn") {
				if (col.GetComponent<Stone> () == null)
					return;
				float rotation = col.transform.rotation.eulerAngles.z;
				if (cima && 45>rotation && rotation<=135) {//Cima
					col.transform.position = to.transform.position;
				} else if (baixo && 180>rotation && rotation<=225) {//Baixo
					col.transform.position = to.transform.position;
				} else if (esquerda && 135>rotation && rotation<=180) {//Esquerda
					col.transform.position = to.transform.position;
				} else if (direita && (0>rotation && rotation<=45) || (225>rotation && rotation<=315)) {//Direit
					col.transform.position = to.transform.position;
				}
			}
		}
	}
}
