using UnityEngine;

public interface CharAttributesBase {
	GameObject getSoulStone();
	bool gainBall();
	bool isFacingRight();
	int getBall();
	void FireBall(Vector3 posi,Quaternion rotation,int hash,int hashTeam);
}
