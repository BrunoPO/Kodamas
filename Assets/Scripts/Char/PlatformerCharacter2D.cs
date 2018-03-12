using System;
using UnityEngine;

namespace UnityStandardAssets._2D{
    public class PlatformerCharacter2D : MonoBehaviour{
		
		[Range(0, 50)] [SerializeField] public float m_MaxSpeed = 10f;                    // The fastest the player can travel in the x axis.
		[Range(0, 1000)] [SerializeField] public float m_JumpForce = 400f;                  // Amount of force added when the player jumps.
        [SerializeField] private bool m_AirControl = false;                 // Whether or not a player can steer while jumping;
		[SerializeField] private int Team = 0; //which team the kodama is party of

		
		[HideInInspector] public bool killed = false;
		[HideInInspector] public Vector3 IniPoint;
		private LayerMask m_WhatIsGround;
		private LayerMask m_WhatIsWall;
		private LayerMask m_WhatIsWallAndGround;
		private LayerMask m_WhatIsPlayer;
        [Range(0, 100)] public float k_jumpWallForce=37;
		private bool isNet;
        private Transform m_GroundCheck;    // A position marking where to check if the player is grounded.
        const float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded
        private bool m_Grounded;            // Whether or not the player is grounded.
		private bool m_OnWall;   			// Whether or not the player is on wall.
        private Transform m_CeilingCheck;   // A position marking where to check for ceilings
        const float k_CeilingRadius = .01f; // Radius of the overlap circle to determine if the player can stand up
       	private Collider2D[] colliders;
		private Collider2D[] collidersWall;
		private CharAttributesNet m_AttributesNet; //var with refence to Attributes on the Net
		private Animator m_Anim;            // Reference to the player's animator component.
		private Rigidbody2D m_Rigidbody2D; // Reference to the player's rigidbody component.
		private GameObject m_GM;
		private GameMaster gm;
		private bool init = false ;
        private int IniPulo = 0;
		private bool jumping = false;
		private bool teamParty = false;
		private improvePlayerBase improvePlayerScript;
		private float timeOfImprovement = -1; 

        private void Awake(){
			m_GroundCheck = transform.Find("GroundCheck");
			m_CeilingCheck = transform.Find("CeilingCheck");
			m_Anim = GetComponent<Animator>();
			m_Rigidbody2D = GetComponent<Rigidbody2D>();
			Init ();
		}

		public int getLife(){
			return GetComponent<CharAttributesNet> ().getLife ();
		}
		public int getTeam(){
			return Team;
		}

		private void Init(){
			if (GameObject.Find ("GM") == null)
				return;
			init = true;
			m_GM = GameObject.Find ("GM");
			isNet = (GetComponent<CharAttributesNet> () != null);
			m_AttributesNet = GetComponent<CharAttributesNet> ();
			gm = m_GM.GetComponent<GMNet> ();
			improvePlayerScript = gm.getImprovePlayer();
			teamParty = gm.isTeamParty();
			m_WhatIsGround = gm.whatIs("Ground");
			m_WhatIsWall = gm.whatIs("Wall");
			m_WhatIsPlayer = gm.whatIs("Player");
			m_WhatIsWallAndGround = gm.whatIs("WallAndGround");
		}

        private void Update() {//Fixed?
            if (!init) {
                Init();
                return;
            }

            if (IniPulo >= 0)
            {
                IniPulo--;
            }

			if(timeOfImprovement > -1){
				if(timeOfImprovement <=1){
					this.retornarParaPadrao();
				}else{
					timeOfImprovement -= Time.fixedDeltaTime;
				}
			}
			
            m_Grounded = false;
            m_OnWall = false;
            colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius/2, m_WhatIsGround);
            m_Grounded = (colliders != null && (colliders.Length > 0));

            //Verifica colisão com o muro na altura do pé ou da cabeça
            //Peito
            collidersWall = Physics2D.OverlapCircleAll(transform.position, k_GroundedRadius * 1.1f, m_WhatIsWall);
			if (collidersWall.Length < 1)
            {
				//Cabeça
				collidersWall = Physics2D.OverlapCircleAll(m_CeilingCheck.position, k_GroundedRadius*1.1f, m_WhatIsWall);
				if (collidersWall.Length < 1) {
					collidersWall = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius*0.9f, m_WhatIsWall);
				}
            }
			m_OnWall = (collidersWall != null && (collidersWall.Length > 0));

            m_Anim.SetBool("Ground", m_Grounded);
			//Verificar se o char está pisando em alguém
            if (!m_Grounded && !killed) {
                if (m_Rigidbody2D.velocity.y < 0) {
                    colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsPlayer);
                    foreach (Collider2D collider in colliders) {
                        if (collider.name == "CeilingCheck") {
							GameObject otherChar = collider.transform.parent.gameObject;
							int otherTeam = otherChar.GetComponent<PlatformerCharacter2D>().getTeam();
							if(this.getTeam() != otherTeam || !this.isTeamParty()){
								Kill(otherChar.GetComponent<PlatformerCharacter2D>().getHash());
								otherChar.GetComponent<PlatformerCharacter2D>().Killed();
								m_Rigidbody2D.velocity = new Vector2(m_Rigidbody2D.velocity.x * m_JumpForce / 50, m_JumpForce / 50);
								break;
							}
                        }
                    }
                }
            }


            m_Anim.SetBool("Wall", m_OnWall);

            Vector3 velo = m_Rigidbody2D.velocity;

            //Se passar do limite volte para -10
            if (velo.y < -15) {
                velo.y = -10;
                m_Rigidbody2D.velocity = velo;
            }

            Quaternion rot = transform.rotation;
            if (rot.w!=0 || rot.w != 1) {
                if (!isFacingRight()) {
                    rot = new Quaternion(0, 1, 0, 0);
                }
                else if (isFacingRight())
                {
                    rot = new Quaternion(0, 0, 0, 1);
                }
                transform.rotation = rot;
            }



            m_Anim.SetFloat("vSpeed", m_Rigidbody2D.velocity.y);
        }

		public bool isTeamParty(){
			return teamParty;
		}

		public int getHash(){
			return m_AttributesNet.getHash();
		}

		public void Move(float move, bool jump){
            if (m_Anim.GetBool("Died")) {
                return;
            }
           
			//Se não olhando para frente inverta.
			if ((move < 0 && isFacingRight()) || (move > 0 && !isFacingRight())) {
				InvertFacing ();
			}

			//Se não estiver pulando e no chão pule
			if (m_Grounded && jump && m_Anim.GetBool ("Ground") && !jumping) {
				jumping = true;
				m_Grounded = false;
				m_Anim.SetBool ("Ground", false);
				m_Rigidbody2D.velocity = Vector2.zero;
				m_Rigidbody2D.AddForce (new Vector2 (0f, m_JumpForce));
				IniPulo = 24;
			} else if (!m_Grounded && jumping) {
				jumping = false;
			}

            if (m_Grounded || m_AirControl){
				m_Anim.SetFloat("Speed", Mathf.Abs(move));
				
				//Se estiver no chão e perto de um muro ande
				if (m_Grounded && m_OnWall) {
					m_Rigidbody2D.velocity = new Vector2 (move * m_MaxSpeed, m_Rigidbody2D.velocity.y);

				//Se tiver perto de um muro pulando (E não no chão)
				} else if (m_OnWall && jump && !m_Grounded && IniPulo <= 1 && Mathf.Abs(move)>0.3f) {
						m_Grounded = false;
						m_Anim.SetBool ("Ground", false);

						if (isFacingRight()) {
							m_Rigidbody2D.AddForce (new Vector2 (-1 * k_jumpWallForce/3 , k_jumpWallForce/20), ForceMode2D.Impulse);
						} else {
							m_Rigidbody2D.AddForce (new Vector2 (1 * k_jumpWallForce/3 , k_jumpWallForce/20), ForceMode2D.Impulse);
						}
                        IniPulo = 24;
                        return;
				}
				//Se não voltou então passe a velocidade Horinzontal.
				m_Rigidbody2D.velocity = new Vector2 (move * m_MaxSpeed, m_Rigidbody2D.velocity.y);
				
            }
            

        }

		private void Kill(int enemyhash){//Had Kill someone jumping on the head
			gm.countKill(getHash(), enemyhash);
		}

    	public void Killed(){//get Killed
			m_AttributesNet.CmdKilled();
		}

		private bool isFacingRight(){
			return m_AttributesNet.m_FacingRight;
		}

		private void InvertFacing(){
			m_AttributesNet.InvertFlip();
		}

		public void ResetChar(){
			m_AttributesNet.ResetAttributes ();
		}

		public void retornarParaPadrao(){
			timeOfImprovement = -1;
			improvePlayerScript.CmdReturnDefault(this.gameObject); 
		}

		public void improvePlayer(int i){
			timeOfImprovement = improvePlayerScript.perform(this.gameObject,i); 
		}
    }
}
