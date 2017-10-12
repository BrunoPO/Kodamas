using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityStandardAssets._2D{
	public class RobotDieScript : StateMachineBehaviour {
		override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
			//Após terminar avisar ao char para que ele tome suas decisões.
			animator.gameObject.GetComponent<PlatformerCharacter2D> ().ResetChar ();
		}
	}
}
