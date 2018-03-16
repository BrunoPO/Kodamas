using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.Networking;
using Prototype.NetworkLobby;

namespace UnityStandardAssets._2D{
	public class GMNet : NetworkBehaviour, GameMaster {
		
		[SerializeField] private int endCount = 5;
		[SerializeField] private GameObject itemBox;
		[SerializeField] private GameObject itemBoxSpawnPoints;
		[SerializeField] private float spawnItemTime = 0;
		[SerializeField] private GameObject Bot;
		public bool Commented = false;

		[Header("As camadas de cada")]
		public LayerMask whatIsGround;
		public LayerMask whatIsWall;
		public LayerMask whatIsPlayer;

		[Header("Textos HUD que serão alterados pelo char")]
		public Text m_StonesTxt;
		public Text m_LifeTxt;
		public Text m_WinTxt;

		[Header("Tranforms dos Spawn points")]
		public Transform SpawnsGO;
		public Transform SpawnsItem;

		[HideInInspector] [SyncVar] public bool m_Reset=false;//Reset deprecated
		List<GameObject> m_Players;
		List<bool> m_PlayersAlive;
		Dictionary<GameObject, bool> m_PlayersDicAlive = new Dictionary<GameObject, bool> ();
		private LobbyManager my_inst;
		[SyncVar] private bool endOfMatch=false;
		[SyncVar] private int teamWinner=-1;
		[SyncVar] private int hashWinner=-1;
		private int m_stones = 0;
		private int m_lifes = 0;
		private Dictionary<string, Dictionary<string, string> > matchHistory = new Dictionary<string, Dictionary<string, string> >();
		private Dictionary<string, GameObject> m_PlayersDicHashGO = new Dictionary<string, GameObject>();
		private float timePassed = 0;
		private float timerSpawnItem = 0;
		private int timeOfParty = 2;
		private int alive = 0;
		private	GameObject lastAlive = null;
		private int partyType = 0;//de 0 até 5
		private bool partyUseTimer = false;
		private bool partyTeam = false;
		private int lastTime = -1;
		private improvePlayerBase improvePlayer; 

		private void Awake(){
			timePassed=0;
			endCount *= 60;

			m_stones = 5;
			m_lifes = 2;

			improvePlayer = GetComponent<improvePlayer>();

			if (GameObject.Find ("LobbyManager") != null) {
				my_inst = GameObject.Find ("LobbyManager").GetComponent<LobbyManager> ();

				m_stones = my_inst.m_quantStones;
				m_lifes = my_inst.m_quantLife;
				partyType = 0;//my_inst.m_partyType;
				timeOfParty = my_inst.timeOfParty;
			}

			partyUseTimer = (partyType == 1 || partyType == 2 || partyType == 4 || partyType == 5);
			partyTeam = (partyType == 3 || partyType == 4 || partyType == 5);
		}
		void Start(){
			AddBotChoosedFromUser();
		}

		public Transform randomAvailablePoint(string type){
			int children = 0;
			int m_type = 0;
			float radius = 3f;
			Transform retorno = null;

			if(type == "Player"){
				children = SpawnsGO.childCount;
				m_type = 1;
			}else if(type == "Item"){
				children = SpawnsItem.childCount;
				m_type = 2;
			}else{
				children = SpawnsItem.childCount + SpawnsGO.childCount;
			}

			if(children == 0){
				return retorno;
			}
			List<int> nums = Enumerable.Range(0,children-1).ToList();
			while(retorno == null){
				int range = nums.Count;
				if(range <= 0){
					break;
				}
				int index = Random.Range(0, range);
				int num = nums[index];
				Transform point = null;
				if(m_type == 1 ){
					point = SpawnsGO.GetChild(num);
				}else if(m_type == 2){
					point = SpawnsItem.GetChild(num);
				}else if(num < SpawnsGO.childCount){
					point = SpawnsGO.GetChild(num);
				}else{
					num = num - SpawnsGO.childCount;
					point = SpawnsItem.GetChild(num);
				}

				if(point == null){
					return point;
				}
				
				Collider2D c = Physics2D.OverlapCircle(point.position, radius,whatIsPlayer);
				if(c == null){
					retorno = point;
				}else{
					nums.RemoveAt(index);
				}

			}
			return retorno;
		}
		
		void AddBotChoosedFromUser(){
			if(!isServer)
				return;
			List<int[]> Bots = new List<int[]>();
			if(my_inst != null)	
				Bots=my_inst.bots;
			if(Bots.Count < 1){
				StartCoroutine(AutoAddBot());
			}
			foreach(int[] bot in Bots){
				int botType = 0;
				if(bot.Length > 1){
					botType = bot[1];
				}
				this.AddBot(my_inst.spawnPrefabs[bot[0]],botType);
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

		public bool isTeamParty(){
			return partyTeam;
		}

		public improvePlayerBase getImprovePlayer(){
			return improvePlayer;
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

			timerSpawnItem += Time.fixedDeltaTime;
			if(spawnItemTime != 0 && timerSpawnItem > spawnItemTime){
				spawnItemBox();
			}

			timePassed += Time.fixedDeltaTime;
			AlterTimeText(timeOfParty - timePassed);
		}

		private void spawnItemBox(){
			timerSpawnItem = 0;
			if(itemBoxSpawnPoints == null)
				return;
			Quaternion rotation = new Quaternion(0f, 0f, 0f,0f);
			int children = itemBoxSpawnPoints.transform.childCount;

			if(children <= 0)
				return;

			Vector3 posi = randomAvailablePoint("Item").position;
			posi.z = 0;
			int randomItemType = Random.Range(0, improvePlayer.getImproves() - 1);
			GameObject inst = Instantiate (itemBox,posi,rotation) as GameObject;
			inst.GetComponent<itemBox>().itemType = randomItemType;
		}

		public void Reset(){
			m_Reset=true;
			hashWinner=-1;
		}

		IEnumerator AutoAddBot(){
			yield return new WaitForSeconds (2);
			if(m_PlayersDicHashGO.Count<2){
				int randomBotType = Random.Range(0,botType.Length()-1);
				this.AddBot(Bot,randomBotType);
				if(partyTeam){
					timeOfParty = 0;
					partyUseTimer = false;
					partyTeam = false;
				}
			}
		}

		private void AddBot(GameObject go, int botType){
			Transform t = randomAvailablePoint("Player");
			if(t != null){
				GameObject inst = Instantiate (go,t.position,new Quaternion(0, 0, 0, 0)) as GameObject;
				inst.GetComponent<EnemyAI> ().enabled = true;
				inst.GetComponent<EnemyAI> ().setBotType(botType);
				NetworkServer.Spawn (inst);
			}
		}

		public void PlayerIn(GameObject ob){
			print ("PlayerIn"+ob);
			if (!isServer || ob.tag != "Player") 
				return;

			EnemyAI ai = ob.GetComponent<EnemyAI>();
			if((ai != null && ai.enabled) || m_PlayersDicHashGO.ContainsKey(""+ob.GetComponent<CharAttributesNet> ().getHash())){
				int randomNum = Random.Range(100, 200) ;
				while(m_PlayersDicHashGO.ContainsKey(randomNum+"")){
					randomNum = Random.Range(100, 1000);
				}
				ob.GetComponent<CharAttributesNet> ().setHash(randomNum);
			}
				
			m_PlayersDicAlive.Add(ob, true);
			int hash = ob.GetComponent<PlatformerCharacter2D> ().getHash ();

			Dictionary<string, string> history = new Dictionary<string, string> ();
			history.Add ("Life",""+ob.GetComponent<PlatformerCharacter2D>().getLife());
			history.Add ("Kill","0");
			history.Add ("Death","0");
			history.Add ("Colectable","0");
			matchHistory.Add ("" + hash, history);

			m_PlayersDicHashGO.Add(""+hash, ob);



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
			
			m_PlayersDicAlive [ob] = false;
			CmdPlayerLosed (ob);
			
			if(partyTeam){//Não há diferença na lógica da execução a variação está em como a variavel alive terá seu valor retornado.
				updateLifeOnTeam();
			}else{
				updateLifeSolo();
			}

			bool isTheEnd = isEndOfTheGame();

			if(isTheEnd){
				showHistory();
			}
		}

		private void updateLifeOnTeam(){
			alive = 0;
			lastAlive = null;
			List<int> Teams = new List<int>();
			foreach(KeyValuePair<GameObject,bool> player in m_PlayersDicAlive)
			{

				string playerHash = "" + player.Key.GetComponent<PlatformerCharacter2D> ().getHash();
				if (player.Value) {// se o player estiver vivo 
					matchHistory [playerHash] ["Life"] = //atualiza as vidas no historico
					""+player.Key.GetComponent<PlatformerCharacter2D> ().getLife();
					lastAlive = player.Key;
					int Team = player.Key.GetComponent<PlatformerCharacter2D> ().getTeam();
					if(!Teams.Contains(Team)){
						Teams.Add(Team);
						++alive;
					}
				} else {
					matchHistory[playerHash]["Life"] = "0";
				}
			}
		}

		private void updateLifeSolo(){
			alive = 0;
			lastAlive = null;
			foreach(KeyValuePair<GameObject,bool> player in m_PlayersDicAlive)
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
		private string[] searchInHistoryTeam(string comparison, string searchedKey, bool unique){
			string hash = "";
			int searchedAux = -1;
			bool higher = (comparison=="Highest");
			int lastTeam = -1;

			if(higher){
				searchedAux = -1;
			}else{
				searchedAux = 1000;
			}

			Dictionary<int,int> Teams = new Dictionary<int,int>();
			foreach(KeyValuePair<string, Dictionary<string, string> > player in matchHistory)
			{
				int value = int.Parse(matchHistory [player.Key] [searchedKey]);
				int team = m_PlayersDicHashGO[player.Key].GetComponent<PlatformerCharacter2D>().getTeam();
				if(!Teams.ContainsKey(team)){
					Teams.Add(team,value);
				}else{
					Teams[team] += value;
				}
			}

			foreach(KeyValuePair<int, int > teamHist in Teams)
			{
				if(higher && teamHist.Value>searchedAux){
					hash = teamHist.Key+"";
					searchedAux = teamHist.Value;
				}else if(unique && teamHist.Value == searchedAux){
					hash="";
				}else if(!higher && teamHist.Value<searchedAux){
					hash = teamHist.Key+"";
					searchedAux = teamHist.Value;
				}
			}


			return new string[] {hash,searchedAux+""};
		}

		private string[] searchInHistorySolo(string comparison, string searchedKey, bool unique){
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

		private void AlterTimeText(float timer){
			if(partyUseTimer){
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

		public GameObject getEnemy(int hash){
			GameObject go = m_PlayersDicHashGO[hash+""];
			if(this.isTeamParty()){
				int team = go.GetComponent<PlatformerCharacter2D>().getTeam();
				int enemyTeam = (team == 0)?1:0;
				string hashEnemy = ""+hash;
				Dictionary<int,List<int>> Teams = new Dictionary<int,List<int>>();
				int myIndex = -1;
				foreach(KeyValuePair<GameObject, bool> player in m_PlayersDicAlive)
				{
					if(player.Key){
						int teamAux = player.Key.GetComponent<PlatformerCharacter2D>().getTeam();
						int hashAux = player.Key.GetComponent<PlatformerCharacter2D>().getTeam();
						int indexAux = -1;
						if(Teams.ContainsKey(team)){
							Teams[team].Add(hashAux);
							indexAux = Teams[team].Count -1;
						}else{
							List<int> teamate = new List<int>();
							teamate.Add(hashAux);
							Teams.Add(team,teamate);
							indexAux = 0;
						}
						if(hashAux == hash){
							myIndex = indexAux ;
						}

					}
				}
				for(int i = myIndex;i>-1;i--){
					if(Teams[enemyTeam].Count > i){
						return m_PlayersDicHashGO[(Teams[enemyTeam][i])+""];
					}
				}
			}else{
				//string[] playersAlive = m_PlayersDicHashGO.Keys.ToArray();
				List<string> playersAlive = new List<string>();
				foreach(KeyValuePair<GameObject, bool> player in m_PlayersDicAlive){
					if(player.Value){
						playersAlive.Add(player.Key.GetComponent<CharAttributesNet>().getHash()+"");
					}
				}
				string[] players = playersAlive.ToArray();
				string lastHash = "-1";
				for(int i = 0;i<players.Length;i++){
					if(lastHash == "-2"){
						lastHash =  players[i];
						break;
					}else if(players[i] == (hash+"")){
						if(i%2 != 0){
							if(lastHash == "-1"){
								lastHash = "-2";
							}else{
								break;
							}
						}else{
							lastHash = players[i];
							break;
						}
					}else{
						lastHash = players[i];
					}
				}
				if(lastHash != "-1" && lastHash != "-2" && lastHash != (hash+"")){
					return m_PlayersDicHashGO[lastHash];
				}
			}
			
			return null;
		}
		private bool isEndOfTheGame(){
			string[] result;
			string lastHash = "";
			switch (partyType){
				case 0://Estilo Last one standing
					if (alive <= 1 && lastAlive != null) {
						endOfMatch = true;
						hashWinner = lastAlive.GetComponent<CharAttributesNet> ().getHash ();
						return true;
					}
					break;
				case 1://
					result = searchInHistorySolo("Highest","Life",true);
					lastHash = result[0];//get hash
					if (lastTime == 0 && lastHash != ""){
						endOfMatch = true;
						hashWinner = int.Parse(lastHash);
						return true;
					}
					break;
				case 2://
					result = searchInHistorySolo("Lowest","Death",true);
					lastHash = result[0];//get hash
					if (lastTime == 0 && lastHash != ""){
						endOfMatch = true;
						hashWinner = int.Parse(lastHash);
						return true;
					}
					break;
				case 3://Ultimo time vivo
					if (alive <= 1 && lastAlive != null) {
						endOfMatch = true;
						hashWinner = lastAlive.GetComponent<PlatformerCharacter2D> ().getTeam();
						print(lastAlive+""+hashWinner);
						return true;
					}
					break;
				case 4://Time com mais vida
					result = searchInHistoryTeam("Highest","Life",true);
					lastHash = result[0];//get hash
					if (lastTime == 0 && lastHash != ""){
						endOfMatch = true;
						hashWinner = int.Parse(lastHash);
						return true;
					}
					break;
				case 5://Time com menos mortes
					result = searchInHistoryTeam("Highest","Life",true);
					lastHash = result[0];//get hash
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
