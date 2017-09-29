using System;
using UnityEngine;

namespace UnityStandardAssets._2D
{
    public class PlatformerCharacter2D : MonoBehaviour
    {
        [SerializeField] private float m_MaxSpeed = 10f;                    // The fastest the player can travel in the x axis.
        [SerializeField] private float m_JumpForce = 400f;                  // Amount of force added when the player jumps.
        [Range(0, 1)] [SerializeField] private float m_CrouchSpeed = .36f;  // Amount of maxSpeed applied to crouching movement. 1 = 100%
        [SerializeField] private bool m_AirControl = false;                 // Whether or not a player can steer while jumping;
		[SerializeField] private LayerMask m_WhatIsGround;
		[SerializeField] private LayerMask m_WhatIsWall;
		[Range(0, 100)] [SerializeField] private int forcaSprint = 50;

		//[HideInInspector] 
		private bool isNet;
        private Transform m_GroundCheck;    // A position marking where to check if the player is grounded.
        const float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded
        private bool m_Grounded;            // Whether or not the player is grounded.
		private bool m_OnWall;   			// Whether or not the player is on wall.
        private Transform m_CeilingCheck;   // A position marking where to check for ceilings
        const float k_CeilingRadius = .01f; // Radius of the overlap circle to determine if the player can stand up
        private Animator m_Anim;            // Reference to the player's animator component.
        private Rigidbody2D m_Rigidbody2D;
		private int counterSprint;
		private Collider2D[] colliders;
		private Collider2D[] collidersOnChest;
		private CharAttributesNet attributesNet; //var with refence to Attributes on the Net
		private CharAttributes attributes;	//var with refence to Attributes on the Local

		private void Awake(){
			// Setting up references.
			m_GroundCheck = transform.Find("GroundCheck");
			m_CeilingCheck = transform.Find("CeilingCheck");
			m_Anim = GetComponent<Animator>();
			m_Rigidbody2D = GetComponent<Rigidbody2D>();
			counterSprint = forcaSprint;

			isNet = (GetComponent<CharAttributesNet> () != null);
			if(isNet)
				attributesNet = GetComponent<CharAttributesNet> ();
			else
				attributes = GetComponent<CharAttributes> ();
		}
		private bool isFacingRight(){
			if (isNet) {
				return attributesNet.m_FacingRight;
			} else {
				return attributes.m_FacingRight;
			}
		}
		private void InvertFacing(){
			if (isNet) {
				attributesNet.InvertFlip();
			} else {
				attributes.InvertFlip();
				//GetComponent<CharAttributes> ().m_FacingRight = !GetComponent<CharAttributes> ().m_FacingRight;
			}
		}
		public void Flip()
		{

			Vector3 rot = transform.rotation.eulerAngles;
			if (rot.y == 0) {
				rot = new Vector3 (rot.x, rot.y + 180, rot.z);
			} else {
				rot = new Vector3 (rot.x, rot.y - 180, rot.z);
			}
			transform.rotation = Quaternion.Euler(rot);
		}

        


        private void FixedUpdate()
        {
			
			if(transform.rotation.eulerAngles.y == 0 && !isFacingRight()){
				Flip ();
			}else if(transform.rotation.eulerAngles.y == 180 && isFacingRight()){
				Flip ();
			}

			
			m_Grounded = false;
			m_OnWall = false;
			// The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
			// This can be done using layers instead but Sample Assets will not overwrite your project settings.
			colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
			m_Grounded = (colliders != null && (colliders.Length > 0));

			//Verifica colisão com o muro na altura do pé ou do peito
			collidersOnChest = Physics2D.OverlapCircleAll (transform.position, k_GroundedRadius, m_WhatIsWall);
			if (collidersOnChest.Length < 1)
				colliders = Physics2D.OverlapCircleAll (m_GroundCheck.position, k_GroundedRadius, m_WhatIsWall);
			else
				colliders = collidersOnChest;
			m_OnWall = (colliders != null && (colliders.Length > 0));

			m_Anim.SetBool ("Ground", m_Grounded);
			if(!m_Grounded){
				m_Anim.SetBool ("Wall", m_OnWall);
			}else{
				m_Anim.SetBool ("Wall", false);
			}
            // Set the vertical animation
            m_Anim.SetFloat("vSpeed", m_Rigidbody2D.velocity.y);
        }




		
		public void Move(float move, bool crouch, bool jump,bool sprint)
        {
            // If crouching, check to see if the character can stand up
            if (!crouch && m_Anim.GetBool("Crouch"))
            {
                // If the character has a ceiling preventing them from standing up, keep them crouching
                if (Physics2D.OverlapCircle(m_CeilingCheck.position, k_CeilingRadius, m_WhatIsGround))
                {
                    crouch = true;
                }
            }

			if (sprint && counterSprint >= forcaSprint) {
				counterSprint = 0;
			}

			if (counterSprint < (forcaSprint * 0.7)) {//Redução da velocidade do sprint
				move *= 5*(forcaSprint-counterSprint)/forcaSprint;
			}

			counterSprint++;

            // Set whether or not the character is crouching in the animator
            m_Anim.SetBool("Crouch", crouch);

            //only control the player if grounded or airControl is turned on
            if (m_Grounded || m_AirControl)
            {
                // Reduce the speed if crouching by the crouchSpeed multiplier
                move = (crouch ? move*m_CrouchSpeed : move);

                // Move the character
				if (m_Grounded && m_OnWall && move != 0) {
					bool hasWalls = false;
					for (int i = 0; i < collidersOnChest.Length; i++) {
						if (move > 0) {
							if (collidersOnChest [i].transform.position.x - transform.position.x > 0) {
								hasWalls = true;
								break;
							}
						} else if (transform.position.x - collidersOnChest [i].transform.position.x > 0) {
							hasWalls = true;
							break;
						}
					}

					if (hasWalls) {
						move = 0;
					} else {
						m_Rigidbody2D.velocity = new Vector2 (move * m_MaxSpeed, m_Rigidbody2D.velocity.y);
					}
				} else {
					m_Rigidbody2D.velocity = new Vector2 (move * m_MaxSpeed, m_Rigidbody2D.velocity.y);
				}

				// The Speed animator parameter is set to the absolute value of the horizontal input.
				m_Anim.SetFloat("Speed", Mathf.Abs(move));
            }
            // If the player should jump...
			if (m_Grounded && jump && m_Anim.GetBool ("Ground")) {
				// Add a vertical force to the player.
				m_Grounded = false;
				m_Anim.SetBool ("Ground", false);
				m_Rigidbody2D.AddForce (new Vector2 (0f, m_JumpForce));
			} else if (jump) {
				if (m_OnWall) {
					m_Grounded = false;
					m_Anim.SetBool ("Ground", false);
					m_Rigidbody2D.AddForce (new Vector2 (-m_Rigidbody2D.velocity.x*10, m_JumpForce/40),ForceMode2D.Impulse);
				}
			}/* else {
				Collider2D[] colliders = Physics2D.OverlapCircleAll (transform.position, k_GroundedRadius, m_WhatIsWall);
				m_OnWall = (colliders != null && (colliders.Length > 0));

				if (m_OnWall) {
					m_Rigidbody2D.AddForce (new Vector2 (0, -130));
				}
			}*/
			if (move < 0 && isFacingRight()) {
				// ... flip the player.
				InvertFacing ();
			}
			// Otherwise if the input is moving the player left and the player is facing right...
			else if (move > 0 && !isFacingRight()) {
				// ... flip the player.
				InvertFacing ();
			}

        }


        
    }
}
