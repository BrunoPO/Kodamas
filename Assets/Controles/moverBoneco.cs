using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moverBoneco : MonoBehaviour {

	public ControleVirtual controle;
	public float velocidade = 0.1F;
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		float translationX = controle.Horizontal () * velocidade; 
		float translationY = controle.Vertical() * velocidade;
		transform.Translate (translationX,translationY,0);
	}
}
