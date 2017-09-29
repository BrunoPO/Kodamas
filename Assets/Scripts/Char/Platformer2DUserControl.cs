using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace UnityStandardAssets._2D
{
	[RequireComponent(typeof (PlatformerCharacter2D))]
	public class Platformer2DUserControl : MonoBehaviour{
		private PlatformerCharacter2D m_Character;
		private bool m_Jump,atck;
		private Transform Ball;
		private GameObject ob;
		private Vector3 lastParamBall;//(v,h,rotation)
		public bool Commented = false;
		private int autoAttackCounter=20;
		public bool autoAttack = false;
		private Vector3 IniPoint;
		private CharAttributesNet m_AttributesNet;
		private CharAttributes m_Attributes;
		private bool isNet = false;

		private GameObject SoulStone;

		private void Awake(){
			m_Character = GetComponent<PlatformerCharacter2D>();
			isNet = (GetComponent<CharAttributesNet> () != null);
			//isNet = m_Character.isNet;
			m_AttributesNet = GetComponent<CharAttributesNet> ();
			m_Attributes = GetComponent<CharAttributes> ();
		}

		private bool isFacingRight(){
			if (isNet)
				return m_AttributesNet.m_FacingRight;
			else
				return m_Attributes.m_FacingRight;
		}
		private int getBalls(){
			if (isNet)
				return m_AttributesNet.balls;
			else
				return m_Attributes.balls;
		}
		private int getHash(){
			if (isNet)
				return m_AttributesNet.getHash();
			else
				return m_Attributes.getHash();
		}




		private void Start(){
			IniPoint = transform.position;
			if (isNet)
				SoulStone = m_AttributesNet.SoulStone;
			else
				SoulStone = m_Attributes.SoulStone;
			print (SoulStone);
		}

		public void ResetChar(){
			GetComponent<Rigidbody2D> ().velocity = new Vector3 (0, 0, 0);
			transform.position = IniPoint;
			if (isNet)
				m_AttributesNet.CmdResetAttributes ();
			else
				m_Attributes.CmdResetAttributes();
		}

		private void Update() {
			if (Input.GetKeyDown("\\")) {
				if (Time.timeScale == 1.0F)
					Time.timeScale = 0.3F;
				else
					Time.timeScale = 1.0F;
				Time.fixedDeltaTime = 0.02F * Time.timeScale;
			}

			if (!m_Jump){ // Read the jump input in Update so button presses aren't missed.
				m_Jump = CrossPlatformInputManager.GetButtonDown("Jump");

			}
			bool crouch = false;
			bool sprint = CrossPlatformInputManager.GetButton ("Fire3");
			float h = CrossPlatformInputManager.GetAxis("Horizontal");
			float v = CrossPlatformInputManager.GetAxis("Vertical");
			//if(Commented) print(v + " " + h);
			if (!autoAttack) {
				atck = Input.GetKey(KeyCode.LeftControl); 
				if (Commented)
					print (atck);
			} else if (autoAttack && !atck && autoAttackCounter>=100) {
				autoAttackCounter = 0;
				atck = true;
				m_Jump = false;crouch = false;h = 0;v = 0;
			} else if(autoAttack){
				atck = false;
				autoAttackCounter++;
				m_Jump = false;crouch = false;h = 0;v = 0;
			}
			if(Commented) print (atck);
			// Pass all parameters to the character control script.

			if (!atck) {
				m_Character.Move (h, crouch, m_Jump,sprint);
				if (ob != null) {
					if(Commented) print (ob.transform.parent);
					print ("Aqui" + getHash());
					Vector3 position = (ob.transform.position + transform.position) / 2;//Alter position na hora de lançar
					Quaternion rotation = ob.transform.rotation;
					Destroy (ob);
					if(isNet)
						m_AttributesNet.CmdSpwnBall (position,rotation,getHash());
					else
						m_Attributes.CmdSpwnBall (position,rotation,getHash());


				}
			}else if(getBalls()>0){
				Ball = this.transform.Find("Ball");
				Vector3 p = transform.position;

				if (Ball == null && ob == null) {
					p.x += (isFacingRight()) ? 1f : -1f;
					//ob.transform.position = posi;
					ob = Instantiate (SoulStone,p,Quaternion.Euler(0, 0, ((isFacingRight())?0:180f))) as GameObject;
					ob.name = "Ball";
					ob.transform.parent = this.transform;
					if(Commented) print ("Não Existe");
					return;
				}

				lastParamBall = Direcao(h, v);

				if (h != 0 || v != 0 || this.GetComponent<Rigidbody2D>().velocity != Vector2.zero) {
					p.x += lastParamBall.x;
					p.y += lastParamBall.y;
					if(Ball != null){
						Ball.transform.position = p;
						Ball.transform.rotation = Quaternion.Euler(0, 0, lastParamBall.z);
					}
				}
				if (h != 0 ) {
					if (h < 0 && isFacingRight())
						m_Character.Move (-0.1f, false, m_Jump,sprint);
					else if (h > 0 && !isFacingRight())
						m_Character.Move (0.1f, false, m_Jump,sprint);
					else 
						m_Character.Move (0f, false, m_Jump,sprint);
				} 
			}
			m_Jump = false;



		}

		Vector3 Direcao(float h, float v){
			if (h < 0 && v < 0) {
				if (Commented) print ("Esq Baixo");
				return new Vector3 (-1f, -1f, 225f);
			} else if (h > 0 && v < 0) {
				if (Commented) print ("Dir Baixo");
				lastParamBall = new Vector3 (1f, -1f, 315f);
			} else if (h < 0 && v > 0) {
				if (Commented) print ("Esq Cima");
				return new Vector3 (-1f, 1f, 135f);
			} else if (h > 0 && v > 0) {
				if (Commented) print ("Dir Cima");
				return new Vector3 (1f, 1f, 45f);
			} else if (h > 0) {
				if (Commented) print ("Dir ");
				return new Vector3 (1f, 0f, 0);
			} else if (h < 0) {
				if (Commented) print ("Esq");
				return new Vector3 (-1f, 0f, 180f);
			} else if (v < 0) {
				if (Commented) print ("Baixo");
				return new Vector3 (0f, -1f, 270f);
			} else if (v > 0) {
				if (Commented) print ("Cima");
				return new Vector3 (0f, 1f, 90f);
			} else if (isFacingRight()) {
				return new Vector3 (1f, 0f, 0);
			}

			return new Vector3 (-1f, 0f, 180f);

		}


	}
}
