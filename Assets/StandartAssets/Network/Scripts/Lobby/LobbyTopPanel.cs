using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Prototype.NetworkLobby
{
    public class LobbyTopPanel : MonoBehaviour
    {
        public bool isInGame = false;
		public bool isServer = false;

        protected bool isDisplayed = true;
        protected Image panelImage;

        void Start()
        {
            panelImage = GetComponent<Image>();
        }

		public void setIsServer(bool b){
			isServer = b;
		}

        void Update(){
            if (!isInGame)
                return;

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ToggleVisibility(!isDisplayed);
            }

        }

        public void ToggleVisibility(bool visible)
        {
            isDisplayed = visible;
            foreach (Transform t in transform){
				if(t.name == "BackButton")
                	t.gameObject.SetActive(isDisplayed);
				else if((isServer || !visible)&&(t.name == "Configuracao" || t.name == "InfoPanel" )){
					print (isServer + " "+!visible);
					t.gameObject.SetActive(isDisplayed);
				}
            }

            if (panelImage != null)
            {
                panelImage.enabled = isDisplayed;
            }
        }
    }
}