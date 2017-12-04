using BarcodeScanner;
using BarcodeScanner.Scanner;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Wizcorp.Utils.Logger;
using ZXing;
using ZXing.QrCode;

public class QRCodeGnrtTexture : MonoBehaviour {

	private RawImage Image;
	private String lastText;
	// Use this for initialization
	void Start () {
		Image = GetComponent<RawImage> ();
		print("QRCode Generate Start");
		//Texture2D myQR = generateQR ("Texto padrão");
		//Image.texture = myQR;		
	}

	private static Color32[] Encode(string textForEncoding, int width, int height) {
		var writer = new BarcodeWriter {
			Format = BarcodeFormat.QR_CODE,
			Options = new QrCodeEncodingOptions {
				Height = height,
				Width = width
			}
		};
		return writer.Write(textForEncoding);
	}

	public Texture2D generateQR(string text) {
		lastText = text;
		print ("Tentando gerar=" + text);
		var encoded = new Texture2D (256, 256);
		var color32 = Encode(text, encoded.width, encoded.height);
		encoded.SetPixels32(color32);
		encoded.Apply();
		//Aplicar textura;
		Image.texture = encoded;
		return encoded;
	}

	public void ClickBack(){
		SceneManager.LoadScene("SceneHome");
	}

	void Update () {

	}
}

