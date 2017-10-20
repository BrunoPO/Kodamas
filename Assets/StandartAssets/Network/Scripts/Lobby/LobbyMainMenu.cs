using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Prototype.NetworkLobby
{
    //Main menu, mainly only a bunch of callback called by the UI (setup throught the Inspector)
    public class LobbyMainMenu : MonoBehaviour 
    {
        public LobbyManager lobbyManager;
		public GameObject ChooseScene;
        public RectTransform lobbyPanel;
        public InputField ipInput;

        public void OnEnable() {
            lobbyManager.topPanel.ToggleVisibility(true);
            ipInput.onEndEdit.RemoveAllListeners();
            ipInput.onEndEdit.AddListener(onEndEditIP);
        }

        public void OnClickHost(){
			ChooseScene.SetActive (true);
            lobbyManager.StartHost();
        }

        public void OnClickJoin(){
			ChooseScene.SetActive (false);
            lobbyManager.ChangeTo(lobbyPanel);
			print (ipInput.text);
			print (Convert.HashToIP (ipInput.text));
			lobbyManager.networkAddress = Convert.HashToIP(ipInput.text);
            lobbyManager.StartClient();
            lobbyManager.backDelegate = lobbyManager.StopClientClbk;
            lobbyManager.DisplayIsConnecting();
            lobbyManager.SetServerInfo("Connecting...", lobbyManager.networkAddress);
        }

        public void OnClickDedicated(){//Pode vir a ser util.
            lobbyManager.ChangeTo(null);
            lobbyManager.StartServer();

            lobbyManager.backDelegate = lobbyManager.StopServerClbk;

            lobbyManager.SetServerInfo("Dedicated Server", lobbyManager.networkAddress);
        }

        void onEndEditIP(string text)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                OnClickJoin();
            }
        }

    }
}
