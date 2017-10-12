using System;
using UnityEngine;

namespace UnityStandardAssets._2D
{
	[RequireComponent(typeof (PlatformerCharacter2D))]
	public class Platformer2DUserControl : MonoBehaviour{

		public ControleVars m_ControleVars;
		[SerializeField] private bool autoAttack = false;
		[SerializeField] private bool Commented = false;

		private PlatformerCharacter2D m_Character;
		private bool m_Jump,atck;
		private Transform t_stone;
		private GameObject go_stone;
		private Vector3 lastParamBall;//(v,h,rotation)
		private int autoAttackCounter=20;
		private CharAttributesNet m_AttributesNet;
		private CharAttributes m_Attributes;
		private bool isNet = false;
		private bool init = false;
		private GameObject gm;
		private GameObject SoulStone;

		private void Awake(){
			m_Character = GetComponent<PlatformerCharacter2D>();
			//isNet = m_Character.isNet;
			m_AttributesNet = GetComponent<CharAttributesNet> ();
			m_Attributes = GetComponent<CharAttributes> ();
			Init ();
		}
		private void Init(){
			if (GameObject.Find ("GM") == null)
				return;
			init = true;
			isNet = (GetComponent<CharAttributesNet> () != null);
			if (isNet)
				SoulStone = m_AttributesNet.SoulStone;
			else
				SoulStone = m_Attributes.SoulStone;
			GameObject m_Controle= GameObject.Find ("Controle");
			if (m_Controle != null) {
				m_ControleVars = m_Controle.GetComponent<ControleVars> ();
			}else
				GetComponent<PlatformerCharacter2D> ().m_JumpForce *= 4.5f;
			print ("Teste controle:"+(m_ControleVars != null));
		}

		private void Update() {
			if (!init) {
				Init ();
				return;
			}
			float h,v;
			//Pega valores
			if (m_ControleVars == null) {
				if (!m_Jump) { 
					m_Jump = Input.GetButtonDown ("Jump");
				}
				h = Input.GetAxis ("Horizontal");
				v = Input.GetAxis ("Vertical");
				atck = Input.GetKey(KeyCode.LeftControl);
			} else {
				h = m_ControleVars.getHorizontal (); 
				v = m_ControleVars.getVertical ();
				if (!m_Jump) {
					m_Jump = m_ControleVars.getPulo ();
				}
				atck = m_ControleVars.getAtk ();
			}

			if(Commented) print(v + " " + h);

			//Ataca se estiver no automatico
			if (!autoAttack) {
				if (Commented) print (atck);
			} else if (autoAttack && !atck && autoAttackCounter>=100) {
				autoAttackCounter = 0;
				atck = true;
				m_Jump = false;h = 0;v = 0;
			} else if(autoAttack){
				atck = false;
				autoAttackCounter++;
				m_Jump = false;h = 0;v = 0;
			}

			//Se não estiver atacando
			if (!atck) {
				//Se mova
				m_Character.Move (h, m_Jump);
				//Se possuir um orbe em seu poder, solte-a.
				if (go_stone != null) {
					if(Commented) print (go_stone.transform.parent);
					Vector3 position = (go_stone.transform.position + transform.position) / 2;//Alter position na hora de lançar
					Quaternion rotation = go_stone.transform.rotation;
					Destroy (go_stone);
					if(isNet)
						m_AttributesNet.CmdSpwnBall (position,rotation,getHash());
					else
						m_Attributes.CmdSpwnBall (position,rotation,getHash());
				}
			//Se não estiver atacando e tem stone
			}else if(getBalls()>0){
				t_stone = this.transform.Find("Stone");
				Vector3 p = transform.position;

				if (t_stone == null && go_stone == null) {//Se stone não estiver no ar,crie-a na frente.
					p.x += (isFacingRight()) ? 1f : -1f;
					go_stone = Instantiate (SoulStone,p,Quaternion.Euler(0, 0, ((isFacingRight())?0:180f))) as GameObject;
					go_stone.name = "Stone";
					go_stone.transform.parent = this.transform;
					if(Commented) print ("Stone criada");
					return;
				}

				//Coloca em posição obedecendo o controle
				if (m_ControleVars != null)
					lastParamBall = DirecaoOnControl (h, v);
				else
					lastParamBall = Direcao(h, v);

				if (h != 0 || v != 0 || this.GetComponent<Rigidbody2D>().velocity != Vector2.zero) {
					p.x += lastParamBall.x;
					p.y += lastParamBall.y;
					if(t_stone != null){
						t_stone.transform.position = p;
						t_stone.transform.rotation = Quaternion.Euler(0, 0, lastParamBall.z);
					}
				}
				if (h != 0 ) {
					if (h < 0 && isFacingRight())
						m_Character.Move (-0.1f, false);
					else if (h > 0 && !isFacingRight())
						m_Character.Move (0.1f, false);
					else 
						m_Character.Move (0f, false);
				} 
			}
			m_Jump = false;



		}


		//Metodos chamados externamente
		public bool gainBall(){
			if (isNet)
				return m_AttributesNet.gainBall();
			else
				return m_Attributes.gainBall();
		}

		private bool isFacingRight(){
			if (isNet)
				return m_AttributesNet.m_FacingRight;
			else
				return m_Attributes.m_FacingRight;
		}

		public int getBalls(){
			if (isNet)
				return m_AttributesNet.getBall();
			else
				return m_Attributes.getBall();
		}

		public int getHash(){
			if (isNet)
				return m_AttributesNet.getHash();
			else
				return m_Attributes.getHash();
		}

		public void Killed(){
			if (isNet)
				m_AttributesNet.CmdKilled();
			else
				m_Attributes.CmdKilled();
		}

		//Metodos chamados internamente
		Vector3 Direcao(float h, float v){
			if (h < 0 && v <0) {
				if (Commented) print ("Esq Baixo");
				return new Vector3 (-1f, -1f, 225f);
			} else if (h > 0 && v <0) {
				if (Commented) print ("Dir Baixo");
				return new Vector3 (1f, -1f, 315f);
			} else if (h < 0 && v >0) {
				if (Commented) print ("Esq Cima");
				return new Vector3 (-1f, 1f, 135f);
			} else if (h > 0 && v >0) {
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

		Vector3 DirecaoOnControl(float h, float v){
			if (h < 0 && v >= -0.85 && v <= -0.3) {
				if (Commented) print ("Esq Baixo");
				return new Vector3 (-1f, -1f, 225f);
			} else if (h > 0 && v >= -0.85 && v <= -0.3) {
				if (Commented) print ("Dir Baixo");
				return new Vector3 (1f, -1f, 315f);
			} else if (h < 0 && v <= 0.85 && v>=0.3) {
				if (Commented) print ("Esq Cima");
				return new Vector3 (-1f, 1f, 135f);
			} else if (h > 0 && v <= 0.85 && v>=0.3) {
				if (Commented) print ("Dir Cima");
				return new Vector3 (1f, 1f, 45f);
			} else if (h > 0.3  && v<0.3 && v>-0.3) {
				if (Commented) print ("Dir ");
				return new Vector3 (1f, 0f, 0);
			} else if (h < -0.3 && v<0.3 && v>-0.3) {
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
