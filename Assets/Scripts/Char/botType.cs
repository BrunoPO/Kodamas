using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class botType : MonoBehaviour {

	public static int Length(){
		return 5;
	}
	public static botConfig getBotByType(int i){
		botConfig Bot = new botConfig();
		switch (i){
			case 0://Default
				break;
			case 1://Shooter
				Bot.distMin = new Vector2(1f,0.5f);
				Bot.distMax = new Vector2(5f,3f);
				Bot.timeBtwnAtks = 1f;
				Bot.timeHoldingAtk = 0.5f;
				break;
			case 2://Jumper
				Bot.distMin = new Vector2(2f,0f);
				Bot.distMax = new Vector2(4f,5f);
				Bot.timeBtwnAtks = 4f;
				Bot.timeHoldingAtk = 1f;
				break;
			case 3: //ignora obj secudario
				Bot.ignoreSec = true;
				break;
			case 4: //target player
				Bot.searchRandomPoints = false;
				Bot.ignoreSec = true;
				break;
			default:
				break;
		}
		return Bot;
	}
}

public class botConfig{
	public Vector2 distMin = new Vector2(1f,1f);
	public Vector2 distMax = new Vector2(5f,3f);
	public float timeBtwnAtks = 2f;
	public float timeHoldingAtk = 1f;
	public bool ignoreSec = false;
	public bool searchRandomPoints = true;
}
