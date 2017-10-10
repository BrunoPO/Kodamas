using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Prototype.NetworkLobby
{
    //Player entry in the lobby. Handle selecting color/setting name & getting ready for the game
    //Any LobbyHook can then grab it and pass those value to the game player prefab (see the Pong Example in the Samples Scenes)
    public class LobbyPlayer : NetworkLobbyPlayer
    {
		
		[Header("ChoosePlayerButtons")]
		public Dictionary<int, int> currentPlayers;
		public GameObject controle;
		public GameObject Scn_ChooseChar;
		private Button player1Button;
		private Button player2Button;
		private Button player3Button;
		public int avatarIndex = 0;

		[Header("Lobby Stuf")]
        static Color[] Colors = new Color[] { Color.magenta, Color.red, Color.cyan, Color.blue, Color.green, Color.yellow };
        //used on server to avoid assigning the same color to two player
        static List<int> _colorInUse = new List<int>();



		public GameObject Scn_ChooseScene;
		public Button Btn_ChooseScene;


        public Button colorButton;
        public InputField nameInput;
        public Button readyButton;
        public Button waitingPlayerButton;

        //OnMyName function will be invoked on clients when server change the value of playerName
        [SyncVar(hook = "OnMyName")]
        public string playerName = "";
        [SyncVar(hook = "OnMyColor")]
        public Color playerColor = Color.white;

        public Color OddRowColor = new Color(250.0f / 255.0f, 250.0f / 255.0f, 250.0f / 255.0f, 1.0f);
        public Color EvenRowColor = new Color(180.0f / 255.0f, 180.0f / 255.0f, 180.0f / 255.0f, 1.0f);

        static Color JoinColor = new Color(255.0f/255.0f, 0.0f, 101.0f/255.0f,1.0f);
        static Color NotReadyColor = new Color(34.0f / 255.0f, 44 / 255.0f, 55.0f / 255.0f, 1.0f);
        static Color ReadyColor = new Color(0.0f, 204.0f / 255.0f, 204.0f / 255.0f, 1.0f);
        static Color TransparentColor = new Color(0, 0, 0, 0);

        //static Color OddRowColor = new Color(250.0f / 255.0f, 250.0f / 255.0f, 250.0f / 255.0f, 1.0f);
        //static Color EvenRowColor = new Color(180.0f / 255.0f, 180.0f / 255.0f, 180.0f / 255.0f, 1.0f);


        public override void OnClientEnterLobby()
        {
            base.OnClientEnterLobby();

            if (LobbyManager.s_Singleton != null) LobbyManager.s_Singleton.OnPlayersNumberModified(1);

            LobbyPlayerList._instance.AddPlayer(this);
            LobbyPlayerList._instance.DisplayDirectServerWarning(isServer && LobbyManager.s_Singleton.matchMaker == null);

            if (isLocalPlayer)
            {
                SetupLocalPlayer();
            }
            else
            {
                SetupOtherPlayer();
            }

            //setup the player data on UI. The value are SyncVar so the player
            //will be created with the right value currently on server
            OnMyName(playerName);
            OnMyColor(playerColor);
        }

		public void onSceneSelect(){
			print("Passed");
			Scn_ChooseScene.SetActive(true);
		}

        public override void OnStartAuthority()
        {
            base.OnStartAuthority();

            //if we return from a game, color of text can still be the one for "Ready"
            readyButton.transform.GetChild(0).GetComponent<Text>().color = Color.white;


			Btn_ChooseScene = GameObject.Find ("LobbyManager").GetComponent<LobbyManager>().Btn_ChooseScene;
			//print (Btn_ChooseScene);

			if (isServer){
				Scn_ChooseScene = GameObject.Find ("LobbyManager").GetComponent<LobbyManager>().Scn_ChooseScene;
				SetupOtherPlayer();
				Btn_ChooseScene.interactable = true;
				Btn_ChooseScene.onClick.RemoveAllListeners ();
				Btn_ChooseScene.onClick.AddListener (onSceneSelect);
			}else{
				SetupLocalPlayer();
				Btn_ChooseScene.interactable = false;
				Btn_ChooseScene.onClick.RemoveAllListeners ();
			}


           SetupLocalPlayer();
        }

        void ChangeReadyButtonColor(Color c)
        {
            ColorBlock b = readyButton.colors;
            b.normalColor = c;
            b.pressedColor = c;
            b.highlightedColor = c;
            b.disabledColor = c;
            readyButton.colors = b;
        }

        void SetupOtherPlayer()
        {
            nameInput.interactable = false;

            ChangeReadyButtonColor(NotReadyColor);

            readyButton.transform.GetChild(0).GetComponent<Text>().text = "...";
            readyButton.interactable = false;

            OnClientReady(false);
        }
		void Update(){
			if (isLocalPlayer) {
				if (Scn_ChooseChar == null) {
					Scn_ChooseChar = GameObject.Find ("LobbyPanel").GetComponent<LobbyPlayerList>().Scn_ChooseChar;
				}

				if (Scn_ChooseChar == null)
					return;
				else {
					controle = Scn_ChooseChar.transform.GetChild (0).gameObject;
					if (controle == null)
						return;
				}

				int c = controle.GetComponent<ChooseChar> ().getChoosed ();

				if (c != avatarIndex) {
					AvatarPicker (c);
				}
			}
		}

        void SetupLocalPlayer()
        {


			//Add Listeners to buttons //MeAcha
			//if (isLocalPlayer) {
				
				
				//Button[] buttons = controle.GetComponentsInChildren<Button>();

				/*player1Button = controle.transform.GetChild (0).GetComponent<Button>() ;
				player2Button = controle.transform.GetChild (1).GetComponent<Button>() ;
				player3Button = controle.transform.GetChild (2).GetComponent<Button>() ;*/
				/*player1Button.onClick.AddListener (delegate {
				print("Clicou");
					AvatarPicker (0);
				});
				player2Button.onClick.AddListener (delegate {
					AvatarPicker (1);
				});
				player3Button.onClick.AddListener (delegate {
					AvatarPicker (2);
				});*/
			//}
			//End


            nameInput.interactable = true;

            CheckRemoveButton();

            if (playerColor == Color.white)
                CmdColorChange();

            ChangeReadyButtonColor(JoinColor);

            readyButton.transform.GetChild(0).GetComponent<Text>().text = "JOIN";
            readyButton.interactable = true;

            //have to use child count of player prefab already setup as "this.slot" is not set yet
            if (playerName == "")
                CmdNameChanged("Player" + (LobbyPlayerList._instance.playerListContentTransform.childCount-1));

            //we switch from simple name display to name input
            colorButton.interactable = true;
            nameInput.interactable = true;

            nameInput.onEndEdit.RemoveAllListeners();
            nameInput.onEndEdit.AddListener(OnNameChanged);

            colorButton.onClick.RemoveAllListeners();
			colorButton.onClick.AddListener(OnCharSelect);

            readyButton.onClick.RemoveAllListeners();
            readyButton.onClick.AddListener(OnReadyClicked);

            //when OnClientEnterLobby is called, the loval PlayerController is not yet created, so we need to redo that here to disable
            //the add button if we reach maxLocalPlayer. We pass 0, as it was already counted on OnClientEnterLobby
            if (LobbyManager.s_Singleton != null) LobbyManager.s_Singleton.OnPlayersNumberModified(0);
        }

		public void AvatarPicker(int number)
		{
			avatarIndex = number;

			print (avatarIndex + " " + number);
			if (isServer)
				RpcAvatarPicked (avatarIndex);
			else
				CmdAvatarPicked (avatarIndex);

		}

		[ClientRpc]
		public void RpcAvatarPicked(int avIndex)
		{
			CmdAvatarPicked (avIndex);
		}

		[Command]
		public void CmdAvatarPicked(int avIndex)
		{
			LobbyManager.s_Singleton.SetPlayerTypeLobby(GetComponent<NetworkIdentity>().connectionToClient, avIndex);
		}

		public void SetPlayerTypeLobby (NetworkConnection conn, int type)
		{
			if (currentPlayers.ContainsKey (conn.connectionId))
				currentPlayers [conn.connectionId] = type;
		}

        //This enable/disable the remove button depending on if that is the only local player or not
        public void CheckRemoveButton()
        {
            if (!isLocalPlayer)
                return;

            int localPlayerCount = 0;
            foreach (PlayerController p in ClientScene.localPlayers)
                localPlayerCount += (p == null || p.playerControllerId == -1) ? 0 : 1;
        }

        public override void OnClientReady(bool readyState)
        {
            if (readyState)
            {
                ChangeReadyButtonColor(TransparentColor);

                Text textComponent = readyButton.transform.GetChild(0).GetComponent<Text>();
                textComponent.text = "READY";
                textComponent.color = ReadyColor;
                readyButton.interactable = false;
                colorButton.interactable = false;
                nameInput.interactable = false;
            }
            else
            {
                ChangeReadyButtonColor(isLocalPlayer ? JoinColor : NotReadyColor);

                Text textComponent = readyButton.transform.GetChild(0).GetComponent<Text>();
                textComponent.text = isLocalPlayer ? "JOIN" : "...";
                textComponent.color = Color.white;
                readyButton.interactable = isLocalPlayer;
                colorButton.interactable = isLocalPlayer;
                nameInput.interactable = isLocalPlayer;
            }
        }

        public void OnPlayerListChanged(int idx)
        { 
            GetComponent<Image>().color = (idx % 2 == 0) ? EvenRowColor : OddRowColor;
        }

        ///===== callback from sync var

        public void OnMyName(string newName)
        {
            playerName = newName;
            nameInput.text = playerName;
        }

        public void OnMyColor(Color newColor)
        {
            playerColor = newColor;
            colorButton.GetComponent<Image>().color = newColor;
        }

        //===== UI Handler

        //Note that those handler use Command function, as we need to change the value on the server not locally
        //so that all client get the new value throught syncvar
		public void OnCharSelect(){
		print (Scn_ChooseChar);
			Scn_ChooseChar.SetActive(true);
		}
		public void OnColorClicked()
        {
            CmdColorChange();
        }

        public void OnReadyClicked()
        {
            SendReadyToBeginMessage();
        }

        public void OnNameChanged(string str)
        {
            CmdNameChanged(str);
        }

        public void OnRemovePlayerClick()
        {
            if (isLocalPlayer)
            {
                RemovePlayer();
            }
            else if (isServer)
                LobbyManager.s_Singleton.KickPlayer(connectionToClient);
                
        }

        public void ToggleJoinButton(bool enabled)
        {
            readyButton.gameObject.SetActive(enabled);
            waitingPlayerButton.gameObject.SetActive(!enabled);
        }

        [ClientRpc]
        public void RpcUpdateCountdown(int countdown)
        {

        }

        [ClientRpc]
        public void RpcUpdateRemoveButton()
        {
            CheckRemoveButton();
        }

        //====== Server Command

        [Command]
        public void CmdColorChange()
        {
            int idx = System.Array.IndexOf(Colors, playerColor);

            int inUseIdx = _colorInUse.IndexOf(idx);

            if (idx < 0) idx = 0;

            idx = (idx + 1) % Colors.Length;

            bool alreadyInUse = false;

            do
            {
                alreadyInUse = false;
                for (int i = 0; i < _colorInUse.Count; ++i)
                {
                    if (_colorInUse[i] == idx)
                    {//that color is already in use
                        alreadyInUse = true;
                        idx = (idx + 1) % Colors.Length;
                    }
                }
            }
            while (alreadyInUse);

            if (inUseIdx >= 0)
            {//if we already add an entry in the colorTabs, we change it
                _colorInUse[inUseIdx] = idx;
            }
            else
            {//else we add it
                _colorInUse.Add(idx);
            }

            playerColor = Colors[idx];
        }

        [Command]
        public void CmdNameChanged(string name)
        {
            playerName = name;
        }

        //Cleanup thing when get destroy (which happen when client kick or disconnect)
        public void OnDestroy()
        {
            LobbyPlayerList._instance.RemovePlayer(this);
            if (LobbyManager.s_Singleton != null) LobbyManager.s_Singleton.OnPlayersNumberModified(-1);

            int idx = System.Array.IndexOf(Colors, playerColor);

            if (idx < 0)
                return;

            for (int i = 0; i < _colorInUse.Count; ++i)
            {
                if (_colorInUse[i] == idx)
                {//that color is already in use
                    _colorInUse.RemoveAt(i);
                    break;
                }
            }
        }
    }
}
