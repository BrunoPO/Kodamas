using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReproduzirQRCode : MonoBehaviour {

	public GameObject imagemQRCode;
	private Texture img; 

	void Start () {
		img = imagemQRCode.GetComponent<RawImage>().texture;
		transform.GetChild (0).GetComponent<RawImage> ().texture = img;
	}
	

	void Update () {
		
	}
}
