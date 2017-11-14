using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class IncreaseAttributes : MonoBehaviour {
	public InputField tfLife, tfStone;
	private int vida = 3, stone = 3;
	void Start () {}
	void Update () {}

	public void maisVida(){
		if(vida < 9){
			vida++;
			atribuirValor(tfLife, vida);
		}
	}

	public void menosVida(){
		if (vida > 1){
			vida--;
			atribuirValor(tfLife, vida);
		}
	}

	public void maisStone(){
		if(stone < 9){
			stone++;
			atribuirValor(tfStone, stone);
		}
	}

	public void menosStone(){
		if (stone > 1){
			stone--;
			atribuirValor(tfStone, stone);
		}
	}

	void atribuirValor(InputField input, int valor){
		input.text = valor.ToString ();
	}
}
