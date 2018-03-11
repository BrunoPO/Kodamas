using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControleVars : MonoBehaviour, Joystick {
	private bool atk;
	private bool pulo;
	private int buttonAtkDelay=10;
	private int buttonPuloDelay=5;

	public void alterAtk(bool b){
		atk = b;
	}
	public void alterPulo(bool b){
        pulo = b;
	}

	public bool getAtk(){
		return atk;
	}
	public bool getPulo(){
		return pulo;
	}

	[SerializeField] private GameObject control;

	public float getHorizontal(){
		return control.GetComponent<ControleVirtual>().Horizontal ();
	}
	public float getVertical(){
		return control.GetComponent<ControleVirtual>().Vertical ();
	}

}
