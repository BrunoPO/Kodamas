using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Prototype.NetworkLobby;

namespace UnityStandardAssets._2D{
	public class GMNet : NetworkBehaviour {
		
		[SerializeField] private int endCount = 5;
		public bool Commented = false;

		[Header("As camadas de cada")]
		public LayerMask whatIsGround;
		public LayerMask whatIsWall;
		public LayerMask whatIsPlayer;

		[Header("Textos HUD que serão alterados pelo char")]
		public Text m_StonesTxt;
		public Text m_LifeTxt;
		public Text m_WinTxt;

		[HideInInspector] [SyncVar] public bool m_Reset=false;
		List<GameObject> m_Players;
		List<bool> m_PlayersAlive;
		private LobbyManager my_inst;
		[SyncVar] private int hashWinner=-1;
		private int m_stones = 0;
		private int m_lifes = 0;

		private void Awake(){
			endCount *= 60;

			m_stones = 3;
			m_lifes = 3;
			if (GameObject.Find ("LobbyManager") != null) {
				my_inst = GameObject.Find ("LobbyManager").GetComponent<LobbyManager> ();

				m_stones = my_inst.m_quantStones;
				m_lifes = my_inst.m_quantLife;
			}
		}

		public int getHashWinner(){
			return hashWinner;
		}

		public int initStones(){
			return m_stones;
		}
		public int initLife(){
			return m_lifes;
		}

		[ServerCallback]
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
			if (hashWinner != -1) {
				if(endCount<=0 && isServer){
					endCount = 1000;//Delay para não retornar denovo
					my_inst.m_ServerReturnToLobby ();//cai o servidor
				}else{
					endCount--;
				}
			}
			if (Input.GetKeyDown ("\\")) {
				if (Time.timeScale == 1.0F)
					Time.timeScale = 0.3F;
				else
					Time.timeScale = 1.0F;
				Time.fixedDeltaTime = 0.02F * Time.timeScale;
			} else if (Input.GetKey(KeyCode.R)) {//Resetar sem voltar ao lobby
				if(Commented) print ("R foi apertado");
				Reset ();
			}else if (isServer && Input.GetKeyDown(KeyCode.N)) {//Volta ao lobby
				if(Commented) print ("N foi apertado");
				my_inst.m_ServerReturnToLobby ();
			}
		}

		public void Reset(){
			m_Reset=true;
			hashWinner=-1;
		}



		public void PlayerIn(GameObject ob){
			print ("PlayerIn"+ob);
			if (!isServer || ob.tag != "Player") 
				return;
			
			if (m_Players == null) {
				m_Players = new List<GameObject> ();
				m_PlayersAlive = new List<bool> ();
			}
			int i = m_Players.IndexOf (ob);
			if(Commented) print ("Added Player:"+i);
			if (i != -1) {
				m_PlayersAlive [i] = true;
			} else {
				m_Players.Add (ob);
				m_PlayersAlive.Add (true);
			}
		}

		public void PlayerOut(GameObject ob){
			print ("PlayerOut"+ob);
			if (!isServer)
				return;
			
			m_PlayersAlive[m_Players.IndexOf(ob)]=false;

			int alive = 0,lastAlive=0;
			for(int i =0;i<m_PlayersAlive.Count;i++){
				if (m_PlayersAlive [i] == true) { 
					lastAlive = i;
					if (++alive >= 2)
						break;
				}
			}

			if (alive <= 1) {
				hashWinner = m_Players[lastAlive].GetComponent<CharAttributesNet> ().getHash ();
			}
		}

	}
}
