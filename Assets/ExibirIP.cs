using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace UnityStandardAssets._2D{
	public class ExibirIP : NetworkBehaviour {
		void Start () {
			Camera.main.GetComponent<Camera2DFollow> ().m_WinTxt.text = Network.player.ipAddress;
		}
	}
}
