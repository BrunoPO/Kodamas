using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

namespace UnityStandardAssets._2D{
	public class CharAttributesNet : NetworkBehaviour {
		public bool unlimitedBalls = false;
		public GameObject SoulStone;
		public int myOrder=0;

		[HideInInspector] [SyncVar(hook = "OnChangeFacing")] public bool m_FacingRight = true;
		[SyncVar(hook = "OnKilled")] private bool m_Killed = false;
		[SyncVar(hook = "OnLosed")] public bool m_Losed = false;
		[SerializeField] [SyncVar] private int balls;
		[SerializeField] [SyncVar] private int life;
		private bool wasKilled=false;
		private Text m_StonesTxt;
		private Text m_LifeTxt;
		private Text m_WinTxt;
		private GameObject m_GameHUD;
		private GameObject m_EndingImg;
		private bool AfterReset = false;
		private bool toReset = true;
		private GameObject m_GM;
		private int ballsIni;
		private int lifeIni;
		private Animator m_Anim;
		private PlatformerCharacter2D m_PlatChar2D;

		private void Start(){
			m_PlatChar2D = GetComponent<PlatformerCharacter2D> ();
			m_PlatChar2D.IniPoint = transform.position;
			m_Anim = GetComponent<Animator> ();
			m_GM = GameObject.Find ("GM");

			if (isLocalPlayer) {
				Camera.main.GetComponent<Camera2DFollow> ().target = this.transform;
				m_GameHUD = Camera.main.GetComponent<Camera2DFollow> ().m_GameHUD;
				m_EndingImg = Camera.main.GetComponent<Camera2DFollow> ().m_EndingImg;
				m_StonesTxt = Camera.main.GetComponent<Camera2DFollow> ().m_StonesTxt;
				m_LifeTxt = Camera.main.GetComponent<Camera2DFollow> ().m_LifeTxt;
				m_WinTxt = Camera.main.GetComponent<Camera2DFollow> ().m_WinTxt;

				m_GameHUD.GetComponent<AlterImg> ().Alter (myOrder);
				m_EndingImg.GetComponent<AlterImg> ().Alter (myOrder);
                Transform t = transform.GetChild(3);
                if (t.name == "Target")
                {
                    t.gameObject.SetActive(true);
                }
			} else {
				GetComponent<Platformer2DUserControl>().enabled = false;
			}
			Reset ();
        }

		private void Update(){

			if(wasKilled && m_Anim.GetBool("Ground"))
				gameObject.GetComponent<SpriteRenderer> ().enabled = true; 

			if (toReset) {
				Reset ();
				return;
			}

			if (m_GM == null) 
				return;
			

			if (GameObject.Find ("GM").GetComponent<GMNet> ().m_Reset) {
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
				if (m_GM.GetComponent<GMNet> ().getEnded ()) {
					endOfMatch ();
				}
			}

		}

		private void endOfMatch(){
			int hashWinner = m_GM.GetComponent<GMNet> ().getHashWinner ();
			if (hashWinner != -1) {
				if (hashWinner == getHash ()) {
					youWon ();
				} else {
					youLose ();
				}
			}
		}

		public int getLife(){
			return life;
		}

		public void SetStonesText(int i){
			m_StonesTxt.text = ""+i;//"Stones:" + 
		}

		public void SetLifeText(int i){
			m_LifeTxt.text =  ""+i;
		}


		public void youLose(){
			this.gameObject.SetActive (false);

			if (!isLocalPlayer)
				return;
			Camera.main.GetComponent<Camera2DFollow> ().target = Camera.main.transform;
			clearTxt ();
			m_WinTxt.gameObject.SetActive (true);
			m_WinTxt.text = "     Você Perdeu!";
			GetComponent<Platformer2DUserControl>().enabled = false;
			m_PlatChar2D.Move (0, false);
		}



		private void OnChangeFacing(bool newBool){

            //print("OnChangeFacing newBool:" + newBool + "facing:" + m_FacingRight);

            m_FacingRight = newBool;
            //print(transform.rotation.eulerAngles);
            //print(transform.rotation);
            if (transform.rotation.y < 0.5f && !newBool){
				Flip ();
			}else if(transform.rotation.y > 0.5f && newBool){
				Flip ();
			}
		}

        public void Flip()
        {

            Quaternion rot = transform.rotation;
            /*print("Fliped----------------");

            print(rot);
            print(transform.rotation);

            print(rot.y);*/

            if (rot.y <= 0.5f)
            {
                rot = new Quaternion(0, 1, 0, 0);
            }
            else
            {
                rot = new Quaternion(0, 0 , 0, 1);
            }

            transform.rotation = rot;
            /*print("After Flip --------------------");
            print(rot);
            print(transform.rotation);*/
        }

        public void OnLosed(bool newBool){
			print("OnLosed");
			if (newBool) {
				youLose ();
			}
		}

        

        private void OnKilled(bool newBool){

            //print (m_Killed +" "+ newBool);
            if (!isServer)//isLocalPlayer
                return;
            if (m_Killed == newBool)
				return;
            m_Killed = newBool;
            //CmdCallDied(newBool);
            m_Anim.SetBool("Died", newBool);
            //m_Anim.SetBool ("Died", newBool);
			if (!newBool) {//se killed for falso reset a posição
				gameObject.GetComponent<SpriteRenderer> ().enabled = false; 
				GetComponent<Rigidbody2D> ().velocity = new Vector3 (0, 0, 0);
				transform.position = m_PlatChar2D.IniPoint;
				wasKilled = true;
				CmdLifeMinus ();
				balls = ballsIni;
			} else {
				#if UNITY_ANDROID || UNITY_IOS
				Handheld.Vibrate();
				#endif
			}
		}

		private void Reset(){
			print ("Trying");
			m_GM = GameObject.Find ("GM");
			if (m_GM == null) {
				toReset = true;
				return;
			}

			GMNet m_GMNet = m_GM.GetComponent<GMNet>();
			toReset = false;
			gameObject.GetComponent<SpriteRenderer> ().enabled = true;
			balls = ballsIni = m_GMNet.initStones();

			life = m_GMNet.initLife();

			//print (balls + "b and l " + life);

			if (isLocalPlayer) {
				AfterReset = true;
				clearTxt ();
				m_StonesTxt.enabled = true;
				m_LifeTxt.enabled = true;
				SetLifeText (life);
				SetStonesText (balls);
				m_WinTxt.gameObject.SetActive (false);
			}
			if (isServer) {
				GameObject.Find ("GM").GetComponent<GMNet> ().PlayerIn (gameObject);
			}
		}



		public void youWon(){
			if (!isLocalPlayer)
				return;
			clearTxt ();
			gameObject.SetActive (false);
			m_WinTxt.gameObject.SetActive (true);
			m_WinTxt.text = "     Você Ganhou!";
			GetComponent<Platformer2DUserControl>().enabled = false;
			m_PlatChar2D.Move (0, false);
		}

		public void clearTxt(){
			if (!isLocalPlayer)
				return;
			m_WinTxt.text = "";
			m_StonesTxt.text = "";
			m_LifeTxt.text = "";
			m_WinTxt.gameObject.SetActive (false);
			m_StonesTxt.enabled = false;
			m_LifeTxt.enabled = false;
		}

		public int getHash(){
			return gameObject.GetComponent<NetworkIdentity> ().netId.GetHashCode ();
		}

		public int getBall(){
			return balls;
		}

		public bool gainBall(){
			if(balls<ballsIni*2){
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
			//print("Was Killed");
			m_Killed = true;
		}

		public void ResetAttributes(){
			CmdResetAttributes ();
		}

		[Command] private void CmdResetAttributes(){
			m_Killed = false;
		}

		[Command] public void CmdGetDamage(){
			CmdLifeMinus();
		}

		public void InvertFlip(){
			if (!isLocalPlayer)
				return;

            m_FacingRight = !m_FacingRight;
            CmdInvertFlip (m_FacingRight);
        }

		[Command] private void CmdInvertFlip(bool facing){
			m_FacingRight = facing;
		}

		[Command] public void CmdSpwnBall(Vector3 posi,Quaternion rotation,int Hash){
			
			GameObject inst = Instantiate (SoulStone,posi,rotation) as GameObject;
			inst.GetComponent<Stone> ().enabled = true;
			GetComponent<CharAttributesNet>().CmdBallsMinus();
			inst.GetComponent<Stone>().Fire (5,Hash);
			NetworkServer.Spawn (inst);

			GameObject inst2 = Instantiate (inst.GetComponent<Stone>().effect,posi,rotation) as GameObject;
			inst2.name = "Effect";
			NetworkServer.Spawn (inst2);
			inst2.GetComponent<Animator> ().SetBool ("Fired", true);
		}

	}
}
