using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

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
		[SerializeField] private bool wasKilled=false;

		[SyncVar] private int balls;//To Private
		[SyncVar] private int life;//To Private
		[SyncVar] public bool m_FacingRight = true;

		[SyncVar] public bool m_Killed = false;

		void Start(){
			GetComponent<PlatformerCharacter2D>().IniPoint = transform.position;
			if (isLocalPlayer) {
				m_StonesTxt = Camera.main.GetComponent<Camera2DFollow>().m_StonesTxt;
				m_LifeTxt = Camera.main.GetComponent<Camera2DFollow>().m_LifeTxt;
				m_WinTxt = Camera.main.GetComponent<Camera2DFollow>().m_WinTxt;
				SetStonesText (balls);
				SetLifeText (life);
				m_WinTxt.text = "";
				Camera.main.GetComponent<Camera2DFollow> ().target = this.transform;
				//GetComponent<Platformer2DUserControl> ().IniPoint = transform.position;
			} else {
				GetComponent<Platformer2DUserControl>().enabled = false;
				//GetComponent<PlatformerCharacter2D>().enabled = false;
			}
			Reset ();
		}

		private void Reset(){
			balls = ballsIni;
			life = lifeIni;
			GameObject.Find ("GM").GetComponent<GMNet> ().PlayerIn (gameObject);
			gameObject.GetComponent<SpriteRenderer> ().enabled = true;
			if (isLocalPlayer) {
				AfterReset = true;
				clearTxt ();
			}
		}

		private void Update(){
			if (m_Killed) {
				GetComponent<Animator> ().SetBool ("Died", true);
				return;
			} else 
				GetComponent<Animator> ().SetBool ("Died", false);
			
			
			if (GameObject.Find ("GM").GetComponent<GMNet> ().m_Reset) {
				Reset ();
				return;
			}
			if (AfterReset) {
				AfterReset = false;
				//if(isLocalPlayer)
					GetComponent<Platformer2DUserControl> ().enabled = true;
			}
			if (isLocalPlayer) {
				SetLifeText (life);
				SetStonesText (balls);

				int hashWinner = GameObject.Find ("GM").GetComponent<GMNet> ().getHashWinner ();
				if (hashWinner != -1) {
					if (hashWinner == getHash ()) {
						youWon ();
					} else {
						youLose ();
					}
				}
			}
		}

		[ClientRpc] public void RpcResetInitPoint(){
			if (!isLocalPlayer)
				return;
			GetComponent<Rigidbody2D> ().velocity = new Vector3 (0, 0, 0);
			transform.position = GetComponent<PlatformerCharacter2D>().IniPoint;
			//gameObject.GetComponent<SpriteRenderer> ().enabled = true;
			CmdShow();
		}

		[Command] void CmdShow(){
			gameObject.GetComponent<SpriteRenderer> ().enabled = true;
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
				GetComponent<PlatformerCharacter2D> ().Move (0, false, false, false);
			}
		}

		public void youWon(){
			if (isLocalPlayer) {
				clearTxt ();
				m_WinTxt.text = "You Won";
				GetComponent<Platformer2DUserControl>().enabled = false;
				GetComponent<PlatformerCharacter2D> ().Move (0, false, false, false);
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

		public int getBall(){
			return balls;
		}

		public bool gainBall(){
			if(balls<6){
				CmdBallsPlus();
				return true;
			}
			return false;
		}

		[Command] public void CmdBallsMinus(){
			if (!unlimitedBalls) {
				balls--;
			}
		}

		[Command] public void CmdBallsPlus(){
			balls++;
		}

		[Command] public void CmdLifeMinus(){
			if (--life <= 0) {
				GameObject.Find ("GM").GetComponent<GMNet> ().PlayerOut (gameObject);
			}
		}

		[Command] public void CmdKilled(){
			print("Called Killed");
			m_Killed = true;
			CmdLifeMinus ();
		}
		public void ResetAttributes(){
			//if (isLocalPlayer && !isServer) {
				//RpcResetInitPoint();
				CmdResetAttributes ();
			//}
		}

		[Command] private void CmdResetAttributes(){
			/*if (!isServer)
				return;*/
			//GetComponent<Rigidbody2D> ().velocity = new Vector3 (0, 0, 0);
			//transform.position = GetComponent<PlatformerCharacter2D>().IniPoint;
			m_Killed = false;
			gameObject.GetComponent<SpriteRenderer> ().enabled = false; 
			print("Out of death");
			//m_FacingRight = false;

			balls=3;
			RpcResetInitPoint();
			if (isServer) {
				GetComponent<Rigidbody2D> ().velocity = new Vector3 (0, 0, 0);
				transform.position = GetComponent<PlatformerCharacter2D>().IniPoint;
				//CmdShow ();
			}

		}

		[Command] public void CmdGetDamage(){
			CmdLifeMinus();
		}

		public void InvertFlip(){
			if(!isServer)
				m_FacingRight = !m_FacingRight;
		
			CmdInvertFlip ();
		}

		[Command] private void CmdInvertFlip(){
			m_FacingRight = !m_FacingRight;
		}

		[Command] public void CmdSpwnBall(Vector3 posi,Quaternion rotation,int Hash){
			GameObject inst = Instantiate (SoulStone,posi,rotation) as GameObject;
			inst.GetComponent<Stone> ().enabled = true;
			GetComponent<CharAttributesNet>().CmdBallsMinus();
			inst.GetComponent<Stone>().Fire (3,Hash);
			NetworkServer.Spawn (inst);

			GameObject inst2 = Instantiate (inst.GetComponent<Stone>().effect,posi,rotation) as GameObject;
			inst2.name = "Effect";
			inst2.GetComponent<Animator> ().SetBool ("Fired", true);
			NetworkServer.Spawn (inst2);
			//ob.transform.rotation = Quaternion.Euler(0, 0, ((m_Character.m_FacingRight)?0:180f));
		}

	}
}
