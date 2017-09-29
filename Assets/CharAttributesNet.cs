﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace UnityStandardAssets._2D{
	public class CharAttributesNet : NetworkBehaviour {
		private Animator m_anim;
		public bool unlimitedBalls = false;
		public GameObject SoulStone;

		[SyncVar]
		public int balls=3;//To Private
		[SyncVar]
		public int life=5;//To Private
		[SyncVar]
		public bool m_FacingRight = true;
		[SyncVar]
		public bool m_Killed = false;

		void Start(){
			if (isLocalPlayer) {
				Camera.main.GetComponent<Camera2DFollow> ().target = this.transform;
			}
			m_anim = GetComponent<Animator> ();
		}
		private void Update(){
			if(m_Killed)
				GetComponent<Animator> ().SetBool ("Died", true);
			else
				GetComponent<Animator> ().SetBool ("Died", false);
		}

		public int getHash(){
			return gameObject.GetComponent<NetworkIdentity> ().netId.GetHashCode ();
		}

		public bool getBall(){
			if(balls<6){
				CmdBallsPlus();
				return true;
			}
			return false;
		}

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

		[Command]
		public void CmdSpwnBall(Vector3 posi,Quaternion rotation,int Hash){
			GameObject inst = Instantiate (SoulStone,posi,rotation) as GameObject;
			//inst.transform.parent = this.transform.parent;
			inst.GetComponent<StoneNet> ().enabled = true;
			inst.GetComponent<StoneNet>().Fire (3,Hash);
			GetComponent<CharAttributesNet>().CmdBallsMinus();
			NetworkServer.Spawn (inst);

			GameObject inst2 = Instantiate (inst.GetComponent<StoneNet>().effect,posi,rotation) as GameObject;
			inst2.name = "Effect";
			//inst2.transform.parent = t.parent;
			inst2.GetComponent<Animator> ().SetBool ("Fired", true);
			NetworkServer.Spawn (inst2);
			//ob.transform.rotation = Quaternion.Euler(0, 0, ((m_Character.m_FacingRight)?0:180f));
		}

	}
}
