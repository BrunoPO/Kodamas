using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface improvePlayerBase {
	int perform(GameObject bo,int i);
	void CmdReturnDefault(GameObject bo);
	int getImproves();
}
