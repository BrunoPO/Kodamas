﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Prototype.NetworkLobby;

namespace UnityStandardAssets._2D{
	public class GMNet : NetworkBehaviour, GameMaster {
		
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
		private float timePassed = 0;
		private int MaxTime = 2;
		private int alive = 0;
		private	GameObject lastAlive = null;
		private int partyType = 1;
		private int lastTime = -1;



		private void Awake(){
			timePassed=0;
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
				string KilledLifeStri = (int.Parse (killledHistory ["Life"]) -1).ToString ();//--1 Life
				matchHistory[killed.ToString()]["Death"] = KilledStri;
				print ("player:" + killed + " has " + KilledStri + " Deaths");
				matchHistory[killed.ToString()]["Life"] = KilledLifeStri;
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
				timePassed = 0;
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
					if(my_inst != null){
						my_inst.m_ServerReturnToLobby ();//cai o servidor
					}
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

			timePassed += Time.fixedDeltaTime;
			AlterTimeText(MaxTime - timePassed);
		}

		public void Reset(){
			m_Reset=true;
			hashWinner=-1;
		}

		public void PlayerIn(GameObject ob){
			print ("PlayerIn"+ob);
			if (!isServer || ob.tag != "Player") 
				return;
				
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
			
			m_PlayersDic [ob] = false;
			CmdPlayerLosed (ob);

			alive = 0;
			lastAlive = null;
			foreach(KeyValuePair<GameObject,bool> player in m_PlayersDic)
			{
				string playerHash = "" + player.Key.GetComponent<PlatformerCharacter2D> ().getHash();
				if (player.Value) {// se o player estiver vivo 
					
					matchHistory [playerHash] ["Life"] = //atualiza as vidas no historico
					""+player.Key.GetComponent<PlatformerCharacter2D> ().getLife();
					

					lastAlive = player.Key;
					++alive;
				} else {
					matchHistory[playerHash]["Life"] = "0";
				}
			}

			bool isTheEnd = isEndOfTheGame();

			if(isTheEnd){
				showHistory();
			}
		}

		private void showHistory(){
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

		private string[] searchInHistory(string comparison, string searchedKey, bool unique){
			string hash = "";
			int searchedAux = -1;
			bool higher = (comparison=="Highest");

			if(higher){
				searchedAux = -1;
			}else{
				searchedAux = 1000;
			}

			foreach(KeyValuePair<string, Dictionary<string, string> > player in matchHistory)
			{
				int value = int.Parse(matchHistory [player.Key] [searchedKey]);
				if(Commented) print(player.Key+"("+searchedKey+")"+searchedAux+":"+value);
				if(higher && value>searchedAux){
					hash = player.Key;
					searchedAux = value;
				}else if(unique && value == searchedAux ){
					hash="";
				}else if(!higher && value<searchedAux){
					hash = player.Key;
					searchedAux = value;
				}
			}
			return new string[] {hash,searchedAux+""};
		}

		private string hashWhoHasMoreLifes(){
			string[] result = searchInHistory("Highest","Life",true);
			return result[0];
		}

		private void AlterTimeText(float timer){
			if(partyType == 1){
				if(timer <= 0){
					timer = 0;
				}else{
					timer = Mathf.Ceil(timer);
				}
				if(lastTime != timer){
					lastTime = (int)timer;
					print("Timer:"+timer);
				}
				if(lastTime == 0){
					isEndOfTheGame();
				}
			}
		}

		private bool isEndOfTheGame(){
			switch (partyType){
				case 0://Estilo Last one standing
					if (alive <= 1 && lastAlive != null) {
						endOfMatch = true;
						hashWinner = lastAlive.GetComponent<CharAttributesNet> ().getHash ();
						return true;
					}
					break;
				case 1://

					string lastHash = hashWhoHasMoreLifes();
					if (lastTime == 0 && lastHash != ""){
						endOfMatch = true;
						hashWinner = int.Parse(lastHash);
						return true;
					}
					break;
			}
			return false;
		}

		public LayerMask whatIs(string s){
			if (s == "Ground") {
				return whatIsGround;
			}else if (s == "Wall") {
				return whatIsWall;
			} else if (s == "WallAndGround") {
				return whatIsGround+whatIsWall;
			} else if (s == "Player") {
				return whatIsPlayer;
			}
			return new LayerMask ();
		}

	}
}
