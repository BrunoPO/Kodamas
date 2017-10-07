using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControleVars : MonoBehaviour {
	private bool atk;
	private bool pulo;
	private int buttonAtkDelay=10;
	private int buttonPuloDelay=5;

	public void Start(){
		float width = this.transform.parent.GetComponent<RectTransform> ().sizeDelta.x;
		float height = this.transform.parent.GetComponent<RectTransform> ().sizeDelta.y;
		this.GetComponent<RectTransform> ().sizeDelta = new Vector2(width,height);
		//this.GetComponent<RectTransform> ().sizeDelta.y = height;
	}

	public void alterAtk(bool b){
		atk = b;
	}
	public void alterPulo(bool b){
		pulo = b;
		//buttonPuloDelay = 10;
	}

	public bool getAtk(){
		return atk;
	}
	public bool getPulo(){
		return pulo;
	}

	[SerializeField] private GameObject control;

	public float getHorizontal(){
		print ("Horizontal:" + control.GetComponent<ControleVirtual> ().Horizontal ());
		return control.GetComponent<ControleVirtual>().Horizontal ();
	}
	public float getVertical(){
		return control.GetComponent<ControleVirtual>().Vertical ();
	}

	private void Update(){
		//print (ButtonAtk.);
		/*if (buttonPuloDelay > 0) {
			buttonPuloDelay--;
		}else if(pulo){
			pulo = false;
		}*/
	}

}
