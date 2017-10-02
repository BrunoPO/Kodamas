using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine;

public class ControleVirtual : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler{

	private Image bgImage;
	private Image controleImg;
	private Vector2 inputVetor;

	void Start () {
		bgImage = GetComponent<Image> ();
		controleImg = transform.GetChild (0).GetComponent<Image> ();
	}

	public virtual void OnDrag(PointerEventData ped){
		Vector2 pos;

		if (RectTransformUtility.ScreenPointToLocalPointInRectangle (bgImage.rectTransform, ped.position, ped.pressEventCamera, out pos)) {
			pos.x = (pos.x / (bgImage.rectTransform.sizeDelta.x));
			pos.y = (pos.y / (bgImage.rectTransform.sizeDelta.y));

			inputVetor = pos * 2;//new Vector2 (pos.x*2, pos.y*2);
			inputVetor = (inputVetor.magnitude > 1) ? inputVetor.normalized : inputVetor;		
			//print ("x: "+inputVetor.x+" y: "+inputVetor.y);
			//Bolinha do Analógico
			controleImg.rectTransform.anchoredPosition = new Vector2(
				inputVetor.x * bgImage.rectTransform.sizeDelta.x/4, 
				inputVetor.y * bgImage.rectTransform.sizeDelta.y/4);
		}
	}

	public virtual void OnPointerDown(PointerEventData ped){ OnDrag (ped); }

	public virtual void OnPointerUp(PointerEventData ped){
		inputVetor = Vector2.zero;
		controleImg.rectTransform.anchoredPosition = inputVetor;
	}

	public float Horizontal(){
		if (inputVetor.x != 0) {
			return inputVetor.x;
		} else {
			return Input.GetAxis("Horizontal");
		}
	}

	public float Vertical(){
		if(inputVetor.y != 0){
			return inputVetor.y;
		}else {
			return Input.GetAxis("Vertical");
		}
	}
}
