using UnityEngine;

public interface GameMaster {
	void Reset();
	void PlayerIn(GameObject ob);
	void PlayerOut(GameObject ob);
	void countKill (int killer, int killed);
	bool getEnded();
	int getTeamWinner();
	int getHashWinner();
	int initStones();
	int initLife();
	LayerMask whatIs(string s);
	bool isTeamParty();
}
