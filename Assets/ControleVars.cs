using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControleVars : MonoBehaviour {
	private bool atk;
	private bool dash;
	private int buttonAtkDelay=10;
	private int buttonDashDelay=10;

	public void alterAtk(bool b){
		atk = b;
	}
	public void alterDash(){
		dash = true;
		buttonDashDelay = 10;
	}

	public bool getAtk(){
		return atk;
	}
	public bool getDash(){
		return dash;
	}

	[SerializeField] private GameObject control;

	public float getHorizontal(){
		return control.GetComponent<ControleVirtual>().Horizontal ();
	}
	public float getVertical(){
		return control.GetComponent<ControleVirtual>().Vertical ();
	}

	private void Update(){
		//print (ButtonAtk.);
		if (buttonDashDelay > 0) {
			buttonDashDelay--;
		}else if(dash){
			dash = false;
		}
	}

}
