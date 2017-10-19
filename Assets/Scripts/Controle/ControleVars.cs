using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControleVars : MonoBehaviour {
	private bool atk;
	private bool pulo;
	private int buttonAtkDelay=10;
	private int buttonPuloDelay=5;

	public void Start(){
		//float width = this.transform.parent.GetComponent<RectTransform> ().sizeDelta.x;
		//float height = this.transform.parent.GetComponent<RectTransform> ().sizeDelta.y;
		//this.GetComponent<RectTransform> ().sizeDelta = new Vector2(width,height);
	}

	public void alterAtk(bool b){
		atk = b;
	}
	public void alterPulo(bool b){
		if (b) {
			pulo = true;
			buttonPuloDelay = 10;
		}
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

	private void Update(){
		if (buttonPuloDelay > 0) {
			buttonPuloDelay--;
		}else if(pulo){
			pulo = false;
		}
	}

}
