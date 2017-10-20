using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlterImg : MonoBehaviour {
	
	public Sprite[] imgs;

	public void Alter(int i){
		print("Trocou Img"+i);
		if(i < imgs.Length){
			gameObject.GetComponent<Image> ().sprite = imgs[i];
		}
	}
}
