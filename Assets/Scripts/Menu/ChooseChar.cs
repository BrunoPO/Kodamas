using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChooseChar : MonoBehaviour {

	private int choosedOne=0;
	public int getChoosed(){
		return choosedOne;
	}
	public void setChoosed(int index){
		choosedOne = index;
	}

}
