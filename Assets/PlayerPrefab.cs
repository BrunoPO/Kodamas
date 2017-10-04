using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;

namespace UnityEngine.Networking
{
	public class PlayerPrefab : PlayerController {
	}
}

/*public class PlayerController
{
	internal const short kMaxLocalPlayers = 8;

	public short playerControllerId = -1;
	public NetworkIdentity unetView;
	public GameObject gameObject;

	public const int MaxPlayersPerClient = 32;

	public PlayerController()
	{
	}

	public bool IsValid { get { return playerControllerId != -1; } }



	public override string ToString()
	{
		return string.Format("ID={0} NetworkIdentity NetID={1} Player={2}", new object[] { playerControllerId, (unetView != null ? unetView.netId.ToString() : "null"), (gameObject != null ? gameObject.name : "null") });
	}
}
*/