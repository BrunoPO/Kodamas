using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UnityStandardAssets._2D{
	public class GM : MonoBehaviour {
		public bool Commented = false;

		[Header("As camadas de cada")]
		public LayerMask whatIsGround;
		public LayerMask whatIsWall;
		public LayerMask whatIsPlayer;

		[Header("Textos HUD que serão alterados pelo char")]
		public Text m_StonesTxt;
		public Text m_LifeTxt;
		public Text m_WinTxt;

		List<GameObject> m_Players;
		List<bool> m_PlayersAlive;
		public int hashWinner=-1;
		public bool m_Reset=false;

		public int getHashWinner(){
			return hashWinner;
		}

		public void Update(){
			if (m_Reset) {
				int alive = 0;
				for(int i =0;i<m_PlayersAlive.Count;i++){
					if (m_PlayersAlive [i] == true) { 
						alive++;
					}
				}
				if (alive - 1 == m_PlayersAlive.Count - 1) {
					m_Reset = false;
				}
				return;
			}
			if (Input.GetKeyDown ("\\")) {
				if (Time.timeScale == 1.0F)
					Time.timeScale = 0.3F;
				else
					Time.timeScale = 1.0F;
				Time.fixedDeltaTime = 0.02F * Time.timeScale;
			} else if (Input.GetKey(KeyCode.R)) {
				print ("R foi apertado");
				Reset ();
			}
		}

		public void Reset(){
			m_Reset=true;
			hashWinner=-1;
		}

		public void PlayerIn(GameObject ob){
			if (ob.tag != "Player")
				return;
			if (m_Players == null) {
				m_Players = new List<GameObject> ();
				m_PlayersAlive = new List<bool> ();
			}
			int i = m_Players.IndexOf (ob);
			print (i);
			if (i != -1) {
				m_PlayersAlive [i] = true;
			} else {
				m_Players.Add (ob);
				m_PlayersAlive.Add (true);
			}
		}

		public void PlayerOut(GameObject ob){
			print ("Died" + ob);
			m_PlayersAlive[m_Players.IndexOf(ob)]=false;
			int alive = 0,hash = -1;
			for(int i =0;i<m_PlayersAlive.Count;i++){
				print (m_PlayersAlive [i]);
				if (m_PlayersAlive [i] == true) { 
					hash = m_Players [i].GetComponent<CharAttributes> ().getHash ();
					alive++;
				}
			}
			print(alive);
			if (alive <= 1) {
				hashWinner = hash;
			}
		}

	}
}
