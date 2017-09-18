using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : MonoBehaviour {
	private float i = 2;
	private float peso = 3; 
	public bool fired = false;
	private float horin,vert;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(fired){
			trageto ();
		}
	}
	void OnTriggerStay2D(Collider2D col) {
		Debug.DrawRay(transform.position, col.transform.position-transform.position, Color.white);
		Quaternion r = transform.rotation;

		r.SetFromToRotation (transform.position, col.transform.position-transform.position);

		r =Quaternion.Euler( (((r.eulerAngles) + (50 * transform.rotation.eulerAngles))/51) );

		transform.rotation = r;
		//Ball.transform.rotation = Quaternion.Euler(0, 0, lastParamBall.z);
		print (col.transform.position);
	}
	private void trageto(){
		//print ("i:"+i+"seno:"+Mathf.Sin(Mathf.PI/i));
		float velo = Mathf.Sin(Mathf.PI/i);
		if (peso != 0) {
			transform.position += transform.right * velo * peso/10;
		}
		if(i<10)
			i += 0.5f;
	}
	public void Ball(float peso){
		fired=true;
		this.peso = peso;
	}
}
