using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityStandardAssets._2D{
	public class CharAttributes : MonoBehaviour {
		public bool unlimitedBalls = false;
		public GameObject SoulStone;
		public bool isLocalPlayer;

		public int balls=3;//To Private
		public int life=5;//To Private
		public bool m_FacingRight = true;
		public bool m_Killed = false;
		[HideInInspector] public bool wasKilled=false;

		void Start(){
			if (isLocalPlayer) {
				Camera.main.GetComponent<Camera2DFollow> ().target = this.transform;
			} else {
				GetComponent<Platformer2DUserControl>().enabled = false;
				//GetComponent<PlatformerCharacter2D>().enabled = false;
			}
		}
		private void Update(){
			if(m_Killed)
				GetComponent<Animator> ().SetBool ("Died", true);
			else
				GetComponent<Animator> ().SetBool ("Died", false);
		}
		public int getHash(){
			return gameObject.GetHashCode();
		}

		public bool gainBall(){
			if(balls<6){
				CmdBallsPlus();
				return true;
			}
			return false;
		}

		public int getBall(){
			return balls;
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
			print("Called Killed");
			m_Killed = true;
			CmdLifeMinus ();
		}
		public void CmdResetAttributes(){
			//m_FacingRight = false;
			m_Killed = false;
			balls=3;
		}
		public void CmdGetDamage(){
			CmdLifeMinus();
		}
		public void InvertFlip(){
			CmdInvertFlip ();
		}
		private void CmdInvertFlip(){
			m_FacingRight = !m_FacingRight;
		}

		public void CmdSpwnBall(Vector3 posi,Quaternion rotation,int Hash){
			GameObject inst = Instantiate (SoulStone,posi,rotation) as GameObject;
			inst.GetComponent<Stone> ().enabled = true;
			inst.GetComponent<Stone>().Fire (3,Hash);
			GetComponent<CharAttributes>().CmdBallsMinus();
			//NetworkServer.Spawn (inst);

			GameObject inst2 = Instantiate (inst.GetComponent<Stone>().effect,posi,rotation) as GameObject;
			inst2.name = "Effect";
			inst2.GetComponent<Animator> ().SetBool ("Fired", true);
			//NetworkServer.Spawn (inst2);
		}

	}
}
