using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace UnityStandardAssets._2D{
	public class CharAttributes : MonoBehaviour {
		public bool unlimitedBalls = false;
		public GameObject SoulStone;
		private Text m_StonesTxt;
		private Text m_LifeTxt;
		private Text m_WinTxt;
		private bool AfterReset = false;

		[SerializeField] private int ballsIni;
		[SerializeField] private int lifeIni;
		[SerializeField] private bool wasKilled=false;

		private int balls;//To Private
		private int life;//To Private
		public bool m_FacingRight = true;

		public bool m_Killed = false;
		private bool isLocalPlayer;
		void Start(){

			GetComponent<PlatformerCharacter2D>().IniPoint = transform.position;
			isLocalPlayer = GetComponent<Platformer2DUserControl> ().enabled;
			if (isLocalPlayer) {
				m_StonesTxt = Camera.main.GetComponent<Camera2DFollow>().m_StonesTxt;
				m_LifeTxt = Camera.main.GetComponent<Camera2DFollow>().m_LifeTxt;
				m_WinTxt = Camera.main.GetComponent<Camera2DFollow>().m_WinTxt;
				SetStonesText (balls);
				SetLifeText (life);
				m_WinTxt.text = "";
				Camera.main.GetComponent<Camera2DFollow> ().target = this.transform;

			}
			Reset ();
		}

		private void Reset(){
			balls = ballsIni;
			life = lifeIni;
			GameObject.Find ("GM").GetComponent<GM> ().PlayerIn (gameObject);
			gameObject.GetComponent<SpriteRenderer> ().enabled = true;
			AfterReset = true;
			if (isLocalPlayer) {
				clearTxt ();
			}

		}

		private void Update(){
			if (m_Killed) {
				GetComponent<Animator> ().SetBool ("Died", true);
				return;
			}else
				GetComponent<Animator> ().SetBool ("Died", false);
			if (GameObject.Find ("GM").GetComponent<GM> ().m_Reset) {
				Reset ();
				return;
			}
			if (AfterReset) {
				AfterReset = false;
				if(isLocalPlayer)
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

		}

		public void RpcResetInitPoint(){
			GetComponent<Rigidbody2D> ().velocity = new Vector3 (0, 0, 0);
			transform.position = GetComponent<PlatformerCharacter2D>().IniPoint;
			//gameObject.GetComponent<SpriteRenderer> ().enabled = true;
			CmdShow();
		}

		void CmdShow(){
			gameObject.GetComponent<SpriteRenderer> ().enabled = true;
		}

		public void SetStonesText(int i){
			m_StonesTxt.text = "Stones:" + i;
		}

		public void SetLifeText(int i){
			m_LifeTxt.text = "Life:" + i;
		}

		public void youLose(){
			clearTxt ();
			m_WinTxt.text = "You Lose";
			GetComponent<Platformer2DUserControl>().enabled = false;
		}

		public void youWon(){
			clearTxt ();
			m_WinTxt.text = "You Won";
			GetComponent<Platformer2DUserControl>().enabled = false;
		}
		public void clearTxt(){
			m_WinTxt.text = "";
			m_StonesTxt.text = "";
			m_LifeTxt.text = "";
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
			if (--life <= 0) {
				GameObject.Find ("GM").GetComponent<GM> ().PlayerOut (gameObject);
			}
		}
		public void CmdKilled(){
			print("Called Killed");
			m_Killed = true;
			CmdLifeMinus ();
		}
		public void ResetAttributes(){
			CmdResetAttributes ();
		}
		private void CmdResetAttributes(){
			m_Killed = false;
			gameObject.GetComponent<SpriteRenderer> ().enabled = false; 
			print("Out of death");
			balls=3;
			RpcResetInitPoint();
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
			GetComponent<CharAttributes>().CmdBallsMinus();
			inst.GetComponent<Stone>().Fire (3,Hash);
			//NetworkServer.Spawn (inst);

			GameObject inst2 = Instantiate (inst.GetComponent<Stone>().effect,posi,rotation) as GameObject;
			inst2.name = "Effect";
			inst2.GetComponent<Animator> ().SetBool ("Fired", true);
			//NetworkServer.Spawn (inst2);
		}

	}
}
