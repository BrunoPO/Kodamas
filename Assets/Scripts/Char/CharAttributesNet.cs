using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace UnityStandardAssets._2D{
	public class CharAttributesNet : NetworkBehaviour {
		public bool unlimitedBalls = false;
		public GameObject SoulStone;
		private Text m_StonesTxt;
		private Text m_LifeTxt;
		private Text m_WinTxt;
		private bool AfterReset = false;
		[SerializeField] private int ballsIni;
		[SerializeField] private int lifeIni;

		[SyncVar]
		public int balls;//To Private
		[SyncVar]
		public int life;//To Private
		[SyncVar]
		public bool m_FacingRight = true;
		[SyncVar]
		public bool m_Killed = false;

		void Start(){
			if (isLocalPlayer) {
				m_StonesTxt = Camera.main.GetComponent<Camera2DFollow>().m_StonesTxt;
				m_LifeTxt = Camera.main.GetComponent<Camera2DFollow>().m_LifeTxt;
				m_WinTxt = Camera.main.GetComponent<Camera2DFollow>().m_WinTxt;
				SetStonesText (balls);
				SetLifeText (life);
				m_WinTxt.text = "";
				Camera.main.GetComponent<Camera2DFollow> ().target = this.transform;
			} else {
				GetComponent<Platformer2DUserControl>().enabled = false;
				//GetComponent<PlatformerCharacter2D>().enabled = false;
			}
			Reset ();
		}

		private void Reset(){
			balls = ballsIni;
			life = lifeIni;
			GameObject.Find ("GM").GetComponent<GM> ().PlayerIn (gameObject);
			if (isLocalPlayer) {
				AfterReset = true;
				clearTxt ();
			}
		}

		private void Update(){
			if (GameObject.Find ("GM").GetComponent<GM> ().m_Reset) {
				Reset ();
				return;
			}
			if (AfterReset) {
				AfterReset = false;
				GetComponent<Platformer2DUserControl> ().enabled = true;
			}
			if (isLocalPlayer) {
				SetLifeText (life);
				SetStonesText (balls);
				int hashWinner = GameObject.Find ("GM").GetComponent<GM> ().getHashWinner ();
				if (hashWinner != -1) {
					if (hashWinner == getHash ()) {
						youWon ();
					} else {
						youLose ();
					}
				}
			}
			if(m_Killed)
				GetComponent<Animator> ().SetBool ("Died", true);
			else
				GetComponent<Animator> ().SetBool ("Died", false);
		}

		public void SetStonesText(int i){
			m_StonesTxt.text = "Stones:" + i;
		}

		public void SetLifeText(int i){
			m_LifeTxt.text = "Life:" + i;
		}

		public void youLose(){
			if (isLocalPlayer) {
				clearTxt ();
				m_WinTxt.text = "You Lose";
				GetComponent<Platformer2DUserControl>().enabled = false;
			}
		}
		public void youWon(){
			if (isLocalPlayer) {
				clearTxt ();
				m_WinTxt.text = "You Won";
				GetComponent<Platformer2DUserControl>().enabled = false;
			}
		}
		public void clearTxt(){
			if (isLocalPlayer) {
				m_WinTxt.text = "";
				m_StonesTxt.text = "";
				m_LifeTxt.text = "";
			}
		}
		public int getHash(){
			return gameObject.GetComponent<NetworkIdentity> ().netId.GetHashCode ();
		}

		public bool gainBall(){
			if(balls<6){
				CmdBallsPlus();
				return true;
			}
			return false;
		}

		[Command]
		public void CmdBallsMinus(){
			if (!unlimitedBalls) {
				balls--;
				if(isLocalPlayer)
					SetStonesText (balls);
			}
		}

		[Command]
		public void CmdBallsPlus(){
			balls++;
			if(isLocalPlayer)
				SetStonesText (balls);
		}

		[Command]
		public void CmdLifeMinus(){
			life--;
			if(life<=0)
				GameObject.Find ("GM").GetComponent<GM> ().PlayerOut (gameObject);
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
			m_FacingRight = !m_FacingRight;
		}

		[Command]
		public void CmdSpwnBall(Vector3 posi,Quaternion rotation,int Hash){
			GameObject inst = Instantiate (SoulStone,posi,rotation) as GameObject;
			inst.GetComponent<Stone> ().enabled = true;
			inst.GetComponent<Stone>().Fire (3,Hash);
			GetComponent<CharAttributesNet>().CmdBallsMinus();
			NetworkServer.Spawn (inst);

			GameObject inst2 = Instantiate (inst.GetComponent<Stone>().effect,posi,rotation) as GameObject;
			inst2.name = "Effect";
			inst2.GetComponent<Animator> ().SetBool ("Fired", true);
			NetworkServer.Spawn (inst2);
			//ob.transform.rotation = Quaternion.Euler(0, 0, ((m_Character.m_FacingRight)?0:180f));
		}

	}
}
