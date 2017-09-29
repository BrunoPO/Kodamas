using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace UnityStandardAssets._2D{
	public class CharAttributesNet : NetworkBehaviour {
		private Animator m_anim;

		[SyncVar]
		public int balls=3;//To Private
		[SyncVar]
		public int life=5;//To Private
		[SyncVar]
		public bool m_FacingRight = true;

		[SyncVar]
		public bool m_Killed = false;



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
		/*public void Flip(){
			CmdFlip();
		}*/
		/*public void BallsMinus(){
			CmdBallsMinus();
		}*/

		[Command]
		public void CmdFacingRightInvert(){
			
		}

		[Command]
		public void CmdBallsMinus(){
			if(!unlimitedBalls)
				balls--;
		}

		[Command]
		public void CmdBallsPlus(){
			balls++;
		}
		[Command]
		public void CmdLifeMinus(){
			life--;
		}

		[Command]
		public void CmdKilled(){
			print("Called Killed");
			m_Killed = true;
			CmdLifeMinus ();
		}

		[Command]
		public void CmdResetAttributes(){
			//m_FacingRight = false;
			m_Killed = false;
			balls=3;
		}
		[Command]
		public void CmdGetDamage(){
			CmdLifeMinus();
		}

		
		public void InvertFlip(){
			if(!isServer)
				m_FacingRight = !m_FacingRight;
		
			CmdInvertFlip ();
		}

		[Command]
		private void CmdInvertFlip(){
			/*if (!isLocalPlayer)
				return;*/
			// Switch the way the player is labelled as facing.
			m_FacingRight = !m_FacingRight;
		}

	}
}
