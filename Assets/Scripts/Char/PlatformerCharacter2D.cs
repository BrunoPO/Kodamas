using System;
using UnityEngine;

namespace UnityStandardAssets._2D{
    public class PlatformerCharacter2D : MonoBehaviour{
		
		[Range(0, 50)] [SerializeField] private float m_MaxSpeed = 10f;                    // The fastest the player can travel in the x axis.
		[Range(0, 1000)] [SerializeField] public float m_JumpForce = 400f;                  // Amount of force added when the player jumps.
        [SerializeField] private bool m_AirControl = false;                 // Whether or not a player can steer while jumping;


		[HideInInspector] public Vector3 IniPoint;
		private LayerMask m_WhatIsGround;
		private LayerMask m_WhatIsWall;
		private LayerMask m_WhatIsPlayer;
		private float k_jumpWallForce;
		private bool isNet;
        private Transform m_GroundCheck;    // A position marking where to check if the player is grounded.
        const float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded
        private bool m_Grounded;            // Whether or not the player is grounded.
		private bool m_OnWall;   			// Whether or not the player is on wall.
        private Transform m_CeilingCheck;   // A position marking where to check for ceilings
        const float k_CeilingRadius = .01f; // Radius of the overlap circle to determine if the player can stand up
       	private Collider2D[] colliders;
		private Collider2D[] collidersOnChest;
		private CharAttributesNet m_AttributesNet; //var with refence to Attributes on the Net
		private CharAttributes m_Attributes;	//var with refence to Attributes on the Local
		private Animator m_Anim;            // Reference to the player's animator component.
		private Rigidbody2D m_Rigidbody2D; // Reference to the player's rigidbody component.
		private bool init = false ;

		private void Start(){
			m_GroundCheck = transform.Find("GroundCheck");
			m_CeilingCheck = transform.Find("CeilingCheck");
			m_Anim = GetComponent<Animator>();
			m_Rigidbody2D = GetComponent<Rigidbody2D>();

			k_jumpWallForce = (m_MaxSpeed*m_JumpForce)/60; //75
			if (GetComponent<Platformer2DUserControl>().m_ControleVars != null)
				k_jumpWallForce *= 4.5f;
			
			print (k_jumpWallForce);
			Init ();
		}
		private void Init(){
			//print ("tentou");
			if (GameObject.Find ("GM") == null)
				return;
			init = true;

			isNet = (GetComponent<CharAttributesNet> () != null);
			if (isNet) {
				m_AttributesNet = GetComponent<CharAttributesNet> ();
				GMNet gm = GameObject.Find ("GM").GetComponent<GMNet> ();
				m_WhatIsGround = gm.whatIsGround;
				m_WhatIsWall = gm.whatIsWall;
				m_WhatIsPlayer = gm.whatIsPlayer;
			} else {
				m_Attributes = GetComponent<CharAttributes> ();
				GM gm = GameObject.Find ("GM").GetComponent<GM> ();
				m_WhatIsGround = gm.whatIsGround;
				m_WhatIsWall = gm.whatIsWall;
				m_WhatIsPlayer = gm.whatIsPlayer;
			}
		}

        private void Update(){//Fixed?
			if (!init) {
				Init ();
				return;
			}
			m_Grounded = false;
			m_OnWall = false;
			colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
			m_Grounded = (colliders != null && (colliders.Length > 0));

			//Verifica colisão com o muro na altura do pé ou do peito
			collidersOnChest = Physics2D.OverlapCircleAll (transform.position, k_GroundedRadius, m_WhatIsWall);
			if (collidersOnChest.Length < 1)
				colliders = Physics2D.OverlapCircleAll (m_GroundCheck.position, k_GroundedRadius, m_WhatIsWall);
			else
				colliders = collidersOnChest;
			m_OnWall = (collidersOnChest != null && (collidersOnChest.Length > 0));

			m_Anim.SetBool ("Ground", m_Grounded);
			if(!m_Grounded){
				m_Anim.SetBool ("Wall", m_OnWall);

				//Verificar se está pisando em alguém
				if (m_Rigidbody2D.velocity.y < 0) {
					colliders = Physics2D.OverlapCircleAll (m_GroundCheck.position, k_GroundedRadius, m_WhatIsPlayer);
					foreach (Collider2D collider in colliders) {
						if (collider.name == "Head") {
							Killed ();
							m_Rigidbody2D.velocity = new Vector2 (m_Rigidbody2D.velocity.x*m_JumpForce/50, m_JumpForce/50);
							//m_Rigidbody2D.AddForce (new Vector2 (m_Rigidbody2D.velocity.x*m_JumpForce/20, m_JumpForce));
							break;
						}
					}
				}
			}else{
				m_Anim.SetBool ("Wall", false);
			}

			Vector3 velo = m_Rigidbody2D.velocity;
			//Se passar do limite volte para -10
			if (velo.y < -15) {
				velo.y = -10;
				m_Rigidbody2D.velocity = velo;
			}

            m_Anim.SetFloat("vSpeed", m_Rigidbody2D.velocity.y);
        }

		public int getHash(){
			if (isNet)
				return m_AttributesNet.getHash();
			else
				return m_Attributes.getHash();
		}

		public void Move(float move, bool jump){
			
			//Se não olhando para frente inverta.
			if ((move < 0 && isFacingRight()) || (move > 0 && !isFacingRight())) {
				InvertFacing ();
			}

			//Se tiver pulando e no chão pule
			if (m_Grounded && jump && m_Anim.GetBool ("Ground")) {
				m_Grounded = false;
				m_Anim.SetBool ("Ground", false);
				m_Rigidbody2D.AddForce (new Vector2 (0f, m_JumpForce));
			}

            if (m_Grounded || m_AirControl){
				m_Anim.SetFloat("Speed", Mathf.Abs(move));

				if (m_Grounded && m_OnWall) {//Se estiver no chão e perto de um muro ande
					m_Rigidbody2D.velocity = new Vector2 (move * m_MaxSpeed, m_Rigidbody2D.velocity.y);

				//Se tiver perto de um muro pulando (E não no chão)
				} else if (m_OnWall && Mathf.Abs (move) > 0.3f && jump) { 
					bool facingWalls = false;

					//Procura se está olhando para o muro
					for (int i = 0; i < collidersOnChest.Length; i++) {
						if (move > 0.3f && collidersOnChest [i].transform.position.x - transform.position.x > 0) {
							facingWalls = true;
							break;
						} else if (move < -0.3f && transform.position.x - collidersOnChest [i].transform.position.x > 0) {
							facingWalls = true;
							break;
						}
					}

					if (facingWalls && !m_Grounded) {//Se olhando para um muro e está pulando
						m_Grounded = false;
						m_Anim.SetBool ("Ground", false);
						if (move > 0) {
							m_Rigidbody2D.AddForce (new Vector2 (-1 * k_jumpWallForce , k_jumpWallForce/3), ForceMode2D.Impulse);
						} else {
							m_Rigidbody2D.AddForce (new Vector2 (1 * k_jumpWallForce , k_jumpWallForce/3), ForceMode2D.Impulse);
						}
						return;
					}
				}
				//Se não voltou então passe a velocidade Horinzontal.
				m_Rigidbody2D.velocity = new Vector2 (move * m_MaxSpeed, m_Rigidbody2D.velocity.y);
				
            }

        }

		public void Killed(){
			if (isNet)
				m_AttributesNet.CmdKilled();
			else
				m_Attributes.CmdKilled();
		}

		private bool isFacingRight(){
			if (isNet) {
				return m_AttributesNet.m_FacingRight;
			} else {
				return m_Attributes.m_FacingRight;
			}
		}

		private void InvertFacing(){
			if (isNet) {
				m_AttributesNet.InvertFlip();
			} else {
				m_Attributes.InvertFlip();
			}
		}

		public void ResetChar(){
			if (isNet) {
				m_AttributesNet.ResetAttributes ();
			} else {
				m_Attributes.ResetAttributes ();
			}
		}

		public void Flip(){

			Vector3 rot = transform.rotation.eulerAngles;
			if (rot.y == 0) {
				rot = new Vector3 (rot.x, rot.y + 180, rot.z);
			} else {
				rot = new Vector3 (rot.x, rot.y - 180, rot.z);
			}
			transform.rotation = Quaternion.Euler(rot);
		}


        
    }
}
