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

		[HideInInspector] [SyncVar] public bool m_Reset=false;//Reset deprecated
		List<GameObject> m_Players;
		List<bool> m_PlayersAlive;
		Dictionary<GameObject, bool> m_PlayersDic = new Dictionary<GameObject, bool> ();
		private LobbyManager my_inst;
		[SyncVar] private bool endOfMatch=false;
		[SyncVar] private int teamWinner=-1;
		[SyncVar] private int hashWinner=-1;
		private int m_stones = 0;
		private int m_lifes = 0;
		private Dictionary<string, Dictionary<string, string> > matchHistory = new Dictionary<string, Dictionary<string, string> >();


		private void Awake(){
			endCount *= 60;

			m_stones = 5;
			m_lifes = 2;
			if (GameObject.Find ("LobbyManager") != null) {
				my_inst = GameObject.Find ("LobbyManager").GetComponent<LobbyManager> ();

				m_stones = my_inst.m_quantStones;
				m_lifes = my_inst.m_quantLife;
			}
		}

		public void countKill (int killer, int killed){
			Dictionary<string, string> killlerHistory = matchHistory[killer.ToString()];
			Dictionary<string, string> killledHistory = matchHistory[killed.ToString()];

			if (killlerHistory != null) {
				string KillsStri = (int.Parse (killlerHistory ["Kill"]) + 1).ToString ();//++1 Kills
				matchHistory[killer.ToString()]["Kill"] = KillsStri;
				print ("player:" + killer + " has " + KillsStri + " Kills");
			}

			if (killledHistory != null) {
				string KilledStri = (int.Parse (killledHistory ["Death"]) + 1).ToString ();//++1 Kills
				matchHistory[killed.ToString()]["Death"] = KilledStri;
				print ("player:" + killed + " has " + KilledStri + " Deaths");
			}

		}

		public bool getEnded(){
			return endOfMatch;
		}
		public int getTeamWinner(){
			return teamWinner;
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
				if (alive - 1 == m_PlayersAlive.Count - 1) {//Pq dos -1?
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
			
			/*if (m_Players == null) {
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
			}*/
			m_PlayersDic.Add(ob, true);
			int hash = ob.GetComponent<PlatformerCharacter2D> ().getHash ();
			Dictionary<string, string> history = new Dictionary<string, string> ();
			history.Add ("Life",""+ob.GetComponent<PlatformerCharacter2D>().getLife());
			history.Add ("Kill","0");
			history.Add ("Death","0");
			history.Add ("Colectable","0");
			matchHistory.Add ("" + hash, history);


		}

		[Command]
		private void CmdPlayerLosed(GameObject ob){
			ob.GetComponent<CharAttributesNet> ().m_Losed=true;
			ob.GetComponent<CharAttributesNet> ().OnLosed(true);
		}

		public void PlayerOut(GameObject ob){
			print ("PlayerOut"+ob);
			if (!isServer)
				return;
			
			//m_PlayersAlive[m_Players.IndexOf(ob)]=false;
			m_PlayersDic [ob] = false;
			CmdPlayerLosed (ob);

			int alive = 0;
			GameObject lastAlive = null;
			/*for(int i =0;i<m_PlayersAlive.Count;i++){
				if (m_PlayersAlive [i] == true) { 
					lastAlive = i;
					++alive;
				}
			}*/
			foreach(KeyValuePair<GameObject,bool> player in m_PlayersDic)
			{
				if (player.Value) {
					lastAlive = player.Key;
					++alive;
				}
			}

			if (alive <= 1 && lastAlive != null) {//alterar lógica para team
				endOfMatch = true;
				hashWinner = lastAlive.GetComponent<CharAttributesNet> ().getHash ();
			}

			foreach(KeyValuePair<string, Dictionary<string, string> > player in matchHistory)
			{
				print ("------------------------------");
				print ("Player:"+player.Key+"---------");
				foreach(KeyValuePair<string, string> history in player.Value)
				{
					print (history.Key + ":" + history.Value);
				}
				print ("------------------------------");
			}
		}

	}
}
