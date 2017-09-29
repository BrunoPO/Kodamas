using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityStandardAssets._2D{
public class CharAttributes : MonoBehaviour {
	private Animator m_anim;
	public int balls=3;//To Private
	public int life=5;//To Private
	public bool m_FacingRight = true; // For determining which way the player is currently facing.

	void Start(){
		m_anim = GetComponent<Animator> ();
	}
	public bool unlimitedBalls = false;

	public bool getBall(){
		if(balls<6){
			CmdBallsPlus();
			return true;
		}
		return false;
	}

	public void CmdBallsMinus(){
		if(!unlimitedBalls)
			balls--;
	}
	public void CmdBallsPlus(){
		balls++;
	}

	public void CmdLifeMinus(){
		life--;
	}
	public void CmdKilled(){
		CmdLifeMinus ();
		m_anim.SetBool ("Died", true);
	}
	public void resetBalls(){
		balls=3;
	}

	public void getDamage(){
		CmdLifeMinus();
	}

}
}
