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
		public GameObject SoulStone;
		Vector3 lastParamBall;//(v,h,rotation)
		public bool Commented = false;
		private int autoAttackCounter=20;
		public bool autoAttack = false;
		private Vector3 IniPoint;
		public CharAttributesNet m_Attributes;

        private void Awake(){
			m_Attributes = GetComponent<CharAttributesNet> ();
            m_Character = GetComponent<PlatformerCharacter2D>();
        }

		private void Start(){
			Camera.main.GetComponent<Camera2DFollow> ().target = this.transform;
			//lastParamBall= new Vector3(1f,0f,0);
			IniPoint = transform.position;
		}

		/*public Vector3 getIniPoint(){
			return IniPoint;
		}*/

		public void ResetChar(){
			GetComponent<Rigidbody2D> ().velocity = new Vector3 (0, 0, 0);
			transform.position = IniPoint;
			GetComponent<CharAttributes>().resetBalls();
		}

        private void Update()
        {
			//Debug Slow Time
			if (Input.GetButtonDown("Fire2")) {
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

			if (!autoAttack) {
				atck = Input.GetKey(KeyCode.LeftControl); 
				if (Commented)
					print (atck);
			} else if (!atck && autoAttackCounter>=100) {
				autoAttackCounter = 0;
				atck = true;
				m_Jump = false;crouch = false;h = 0;v = 0;
			} else {
				atck = false;
				autoAttackCounter++;
				m_Jump = false;crouch = false;h = 0;v = 0;
			}



            
			//if(Commented) print(v + " " + h);

            // Pass all parameters to the character control script.
			if (!atck) {
				m_Character.Move (h, crouch, m_Jump,sprint);
				if (ob != null) {
					if(Commented) print (ob.transform.parent);
					CmdSpwnBall (ob.transform.position,ob.transform.rotation,gameObject.GetHashCode());
					Destroy (ob);
				}
			}else if(GetComponent<CharAttributes>().balls>0){
				Ball = this.transform.Find("Ball");
				Vector3 p = transform.position;

				if (Ball == null && ob == null) {
					if(Commented) print ("Não Existe");
					ob = Instantiate (SoulStone) as GameObject;
					ob.name = "Ball";
					ob.transform.parent = this.transform;
					p.x += (m_Attributes.m_FacingRight) ? 1f : -1f;
					ob.transform.position = p;
					ob.transform.rotation = Quaternion.Euler(0, 0, ((m_Attributes.m_FacingRight)?0:180f));
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
				if (h != 0) {
					if (h < 0 && m_Attributes.m_FacingRight)
						m_Character.Move (-0.1f, false, m_Jump,sprint);
					else if (h > 0 && !m_Attributes.m_FacingRight)
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
			} else if (m_Attributes.m_FacingRight) {
				return new Vector3 (1f, 0f, 0);
			}

			return new Vector3 (-1f, 0f, 180f);

		}

		void CmdSpwnBall(Vector3 posi,Quaternion rotation,int Hash){
			GameObject inst = Instantiate (SoulStone,posi,rotation) as GameObject;
			inst.transform.parent = transform.parent;
			inst.GetComponent<Stone> ().enabled = true;
			inst.GetComponent<Stone>().Fire (3,Hash);
			GetComponent<CharAttributes>().CmdBallsMinus();
			//NetworkServer.Spawn (inst);

			GameObject inst2 = Instantiate (inst.GetComponent<Stone>().effect,posi,rotation) as GameObject;
			inst2.name = "Effect";
			inst2.transform.parent = transform.parent;
			inst2.GetComponent<Animator> ().SetBool ("Fired", true);
			//NetworkServer.Spawn (inst2);
		}
    }
}
