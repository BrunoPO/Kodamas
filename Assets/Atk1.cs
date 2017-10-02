using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Atk1 : MonoBehaviour, IPointerDownHandler,IPointerUpHandler {

	public GameObject controle;
	public void OnPointerDown(PointerEventData eventData){
		Debug.Log ("Apertou");
		controle.GetComponent<ControleVars> ().alterAtk (true);
	}
	public void OnPointerUp(PointerEventData eventData){
		Debug.Log ("Apertou");
		controle.GetComponent<ControleVars> ().alterAtk (false);
	}
}
