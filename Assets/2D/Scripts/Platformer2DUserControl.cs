using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;


namespace UnityStandardAssets._2D
{
    [RequireComponent(typeof (PlatformerCharacter2D))]
	public class Platformer2DUserControl : MonoBehaviour
    {
		private int balls=3;
        private PlatformerCharacter2D m_Character;
        private bool m_Jump,atck;
		private Transform Ball;
		private GameObject ob;
		public GameObject SoulStone;
		Vector3 lastParamBall;//(v,h,rotation)
		public bool Commented = false;


        private void Awake(){
            m_Character = GetComponent<PlatformerCharacter2D>();
        }

		private void Start(){
			Camera.main.GetComponent<Camera2DFollow> ().target = this.transform;
			lastParamBall= new Vector3(1f,0f,0);
		}

		public bool getBall(){
			if(balls<6){
				balls++;
				return true;
			}
			return false;
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
			atck=CrossPlatformInputManager.GetButton("Fire3");
			if(Commented) print (atck);

            bool crouch = Input.GetKey(KeyCode.LeftControl);
            float h = CrossPlatformInputManager.GetAxis("Horizontal");
			float v = CrossPlatformInputManager.GetAxis("Vertical");
			//if(Commented) print(v + " " + h);

            // Pass all parameters to the character control script.
			if (!atck) {
				m_Character.Move (h, crouch, m_Jump);
				if (ob != null) {
					if(Commented) print (ob.transform.parent);
					ob.transform.parent = this.transform.parent;
					ob.GetComponent<Fire> ().enabled = true;
					ob.GetComponent<Fire> ().Ball (3);
					ob = null;
					balls--;
				}
			}else if(balls>0){
				float moveh;
				Ball = this.transform.Find("Ball");
				Vector3 p = transform.position;

				if (Ball == null && ob == null) {
					if(Commented) print ("Não Existe");
					ob = Instantiate (SoulStone) as GameObject;
					ob.name = "Ball";
					ob.transform.parent = this.transform;
					p.x += (m_Character.m_FacingRight) ? 1f : -1f;
					ob.transform.position = p;
					ob.transform.rotation = Quaternion.Euler(0, 0, ((m_Character.m_FacingRight)?0:180f));
					return;
				}
				if (h < 0 && v < 0) {
					lastParamBall = new Vector3 (-1f, -1f, 225f);
					if (Commented)
						print ("Esq Baixo");
				} else if (h > 0 && v < 0) {
					lastParamBall = new Vector3 (1f, -1f, 315f);
					if (Commented)
						print ("Dir Baixo");
				} else if (h < 0 && v > 0) {
					lastParamBall = new Vector3 (-1f, 1f, 135f);
					if (Commented)
						print ("Esq Cima");
				} else if (h > 0 && v > 0) {
					lastParamBall = new Vector3 (1f, 1f, 45f);
					if (Commented)
						print ("Dir Cima");
				} else if (h > 0) {
					lastParamBall = new Vector3 (1f, 0f, 0);
					if (Commented)
						print ("Dir ");
				} else if (h < 0) {
					lastParamBall = new Vector3 (-1f, 0f, 180f);
					if (Commented)
						print ("Esq");
				} else if (v < 0) {
					lastParamBall = new Vector3 (0f, -1f, 270f);
					if (Commented)
						print ("Baixo");
				} else if (v > 0) {
					lastParamBall = new Vector3 (0f, 1f, 90f);
					if (Commented)
						print ("Cima");
				} else if (m_Character.m_FacingRight) {
					lastParamBall = new Vector3 (1f, 0f, 0);
				} else {
					lastParamBall = new Vector3 (-1f, 0f, 0);
				}

				if (h != 0 || v != 0 || this.GetComponent<Rigidbody2D>().velocity != Vector2.zero) {
					p.x += lastParamBall.x;
					p.y += lastParamBall.y;
					if(Ball != null){
						Ball.transform.position = p;
						Ball.transform.rotation = Quaternion.Euler(0, 0, lastParamBall.z);
					}
				}
				if (h != 0) {
					if (h < 0 && m_Character.m_FacingRight)
						m_Character.Move (-0.1f, false, m_Jump);
					else if (h > 0 && !m_Character.m_FacingRight)
						m_Character.Move (0.1f, false, m_Jump);
					else 
						m_Character.Move (0f, false, m_Jump);
				} 
			}
            m_Jump = false;

        }
    }
}
