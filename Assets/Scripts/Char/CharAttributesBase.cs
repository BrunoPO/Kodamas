using UnityEngine;

public interface CharAttributesBase {
	GameObject getSoulStone();
	bool gainBall();
	bool isFacingRight();
	int getBall();
	void CmdSpwnBall(Vector3 posi,Quaternion rotation,int Hash);
}
