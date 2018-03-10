using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Prototype.NetworkLobby;
using UnityStandardAssets._2D;

public class improvePlayer : NetworkBehaviour,improvePlayerBase {

	int improves = 7;

	[SerializeField] private int timeWithPower = 20;

	public int getImproves(){
		return improves;
	}
	public int perform(GameObject bo,int i){
		this.CmdPerform(bo,i);
		if(i<3)
			return -1;
		else if(i<7)
			return timeWithPower;
		else
			return -1;

	}

	[Command]
	private void CmdPerform(GameObject bo,int i){
		CharAttributesNet charAttri = bo.GetComponent<CharAttributesNet>();
		switch (i)
		{
			case 0:
				charAttri.gainBall();
				break;
			case 1:
				charAttri.gainBall();
				charAttri.gainBall();
				break;
			case 2:
				charAttri.gainLife();
				break;
			case 3:
				charAttri.gainLife();
				charAttri.gainLife();
				break;
			case 4:
				charAttri.improved = 1;
				break;
			case 5:
				charAttri.improved = 2;
				break;
			case 6:
				charAttri.improved = 3;
				break;
			case 7://scene change
				break;

		}
	}

	[Command]
	public void CmdReturnDefault(GameObject bo){
		CharAttributesNet charAttri = bo.GetComponent<CharAttributesNet>();
		charAttri.improved = 0;
	}
}
