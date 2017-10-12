using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Btn_pulo : MonoBehaviour, IPointerDownHandler,IPointerUpHandler {

	public GameObject controle;
	ControleVars m_vars;
	public void Start(){
		m_vars = controle.GetComponent<ControleVars> ();
	}
	public void OnPointerDown(PointerEventData eventData){
		m_vars.alterPulo (true);
	}
	public void OnPointerUp(PointerEventData eventData){
		m_vars.alterPulo (false);
	}
}
