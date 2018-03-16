using System.Collections;
using UnityEngine;
using Pathfinding;
namespace UnityStandardAssets._2D{
public class EnemyAI : MonoBehaviour, Joystick {

	//What to chase?
	public Transform target;
	public GameObject targetGO;
	public Transform targetSec;

	public int typeOfBot = 0;

	private Vector2 distMin = new Vector2(1f,1f);
	private Vector2 distMax = new Vector2(5f,3f);
	private float timeBtwnAtks = 2f;
	private float timeHoldingAtk = 1f;
	private bool ignoreSec = false;
	private bool searchRandomPoints = true;
	
	private float timerHoldingAtk=0;
	private float timerBtwnAtks=0;
	private GMNet gm;

	// How many secs is need to wait until recaculate the path
	private float waitTillUpdate = 1f;

	//Caching
	private Seeker m_seeker;
	private Rigidbody m_rb;

	//The calculated path
	private Path path;

	public bool Commented = false;

	[HideInInspector]
	private bool pathIsEnded = false;

	//The max distance from the AI to a waypoint for it to continue to the next waypoint
	private float nextWaypointDistance = 3f;

	private int currentWaypoint = 0;

	private bool m_jump = false;
	private bool m_atk = false;
	private Vector2 m_arrow = Vector2.zero;
	private float sceneDist = 20f;
	private bool isTaggingTargetMain = true;

	public void setBotType(int c){
		botConfig botConfig = botType.getBotByType(c);
		this.distMin = botConfig.distMin;
		this.distMax = botConfig.distMax;;
		this. timeBtwnAtks = botConfig.timeBtwnAtks;
		this.timeHoldingAtk = botConfig.timeHoldingAtk;
		this.ignoreSec = botConfig.ignoreSec;
		this.searchRandomPoints = botConfig.searchRandomPoints;
		typeOfBot = c;
	}

	void Start(){
		m_seeker = GetComponent<Seeker>();
		m_rb = GetComponent<Rigidbody>();

		m_arrow.x = 1f;
		if(GameObject.Find("GM"))
		gm = GameObject.Find("GM").GetComponent<GMNet>();

		StartCoroutine(PlayerIn());
		GetComponent<Platformer2DUserControl>().setController(this);
		sceneDist = Camera.main.GetComponent<Camera2DFollow>().getSceneDist();
	}

	IEnumerator enemyAlive(){
		if(target != null){
			if(!targetGO.activeSelf){
				target = null;
			}
		}
		yield return new WaitForSeconds (3);
		StartCoroutine(enemyAlive());
	}
	IEnumerator PlayerIn(){
		yield return new WaitForSeconds (1);
		if(gm == null){
			gm = GameObject.Find("GM").GetComponent<GMNet>();
			StartCoroutine(PlayerIn());
		}else{
			gm.PlayerIn (gameObject);
			StartCoroutine(UpdatePath());
			StartCoroutine(enemyAlive());
		}
		
	}

	IEnumerator UpdatePath(){
		if(target == null){
			searchTarget();
		}else{
			//Start a new path to the target position, return the result to OnPathComplete method
			if(isTaggingTargetMain)
				m_seeker.StartPath (transform.position,target.position,OnPathComplete);
			else if(targetSec != null)
				m_seeker.StartPath (transform.position,targetSec.position,OnPathComplete);
		}

		yield return new WaitForSeconds (waitTillUpdate);
		StartCoroutine(UpdatePath());
	}

	private void searchTarget(){
		targetGO = gm.getEnemy(GetComponent<PlatformerCharacter2D>().getHash());
		if(targetGO != null){
			target = targetGO.transform;
		}
	}

	public void OnPathComplete(Path p){
		if(p.error){
			if(Commented) Debug.Log("error:"+p.error);
		}else{
			path = p;
			currentWaypoint = 0;
		}

	}

	public void FixedUpdate(){
		if(target == null || path == null){
			timerBtwnAtks=timeBtwnAtks;
			m_arrow = Vector2.zero;
			m_jump = false;
			m_atk = false;
			return;
		}

		if(currentWaypoint >= path.vectorPath.Count){
			if(pathIsEnded)
				return;

			isTaggingTargetMain=true;
			targetSec = null;
			if(Commented) Debug.Log("End of path reached");
			pathIsEnded = true;
			return;
		}
		pathIsEnded = false;

		Vector3 dir = new Vector3();
		Vector3 dirNormalized = new Vector3();
		float distFromTarget = Vector3.Distance(transform.position, target.position);
		float intecityFromDistance = Mathf.Abs(1 - (distFromTarget / sceneDist));

		if(m_atk && timerHoldingAtk < timeHoldingAtk*(2 - intecityFromDistance)){//Quando estiver atirando mire no target
			dir = (target.position - transform.position).normalized;
			timerHoldingAtk+=Time.fixedDeltaTime;
			timerBtwnAtks=0;
			m_jump=false;
		}else{//se não direcione-se para o proximo ponto
			dir = (path.vectorPath[currentWaypoint] - transform.position);
			dirNormalized = dir.normalized;
			m_atk=false;
			timerBtwnAtks+=Time.fixedDeltaTime;
			if(dir.y > distMin.y && dir.y < distMax.y)
				m_jump = true;
		}
		m_arrow.y = dirNormalized.y;
		m_arrow.x = dirNormalized.x;
		m_arrow.x *= intecityFromDistance;//regula a intencidade da movimentação horizontal.
		

		float distFromTargetOnY = Mathf.Abs(transform.position.y - target.position.y);
		if(!m_atk && timerBtwnAtks > timeBtwnAtks){
			if(m_arrow.y < 0.3f && distFromTarget > distMax.x && distFromTargetOnY < distMax.y){
				m_atk = true;
				timerBtwnAtks=0;
			}else if( distFromTarget>distMin.x && distFromTarget <distMax.x && distFromTargetOnY < distMax.y){
				m_atk = true;
				timerBtwnAtks=0;
			}
		}

		isTaggingTargetMain = true;
		if(searchRandomPoints && targetSec == null && distFromTarget>distMax.x*1.2f){
			targetSec = gm.randomAvailablePoint("All");
		}

		if(!m_atk && distFromTarget<distMin.x){
			m_jump = true;
		}else if(targetSec != null){
			float distSec = Vector3.Distance(transform.position, targetSec.position);
			if(!ignoreSec && (distSec/distFromTarget)<=0.7f){
				isTaggingTargetMain = false;
			}
		}

		
		float distFromPoint = Vector3.Distance(transform.position, path.vectorPath[currentWaypoint]);
		if(distFromPoint < nextWaypointDistance){
			currentWaypoint++;
			return;
		}



	}
	private void OnDrawGizmosSelected() {
		Gizmos.color = Color.blue;
		Vector3 IniX = transform.position;
		IniX.x += distMin.x;
		Vector3 FimX = transform.position;
		FimX.x += distMax.x;
		Gizmos.DrawLine(IniX, FimX);

		Gizmos.color = Color.green;
		Vector3 IniY = transform.position;
		IniY.y += distMin.y;
		Vector3 FimY = transform.position;
		FimY.y += distMax.y;
		Gizmos.DrawLine(IniY, FimY);
    }

	public bool getAtk(){
		return m_atk;
	}
	public bool getPulo(){
		bool auxJump = m_jump;
		m_jump = false;
		return auxJump;
	}
	public float getHorizontal(){
		return m_arrow.x;
	}
	public float getVertical(){
		return m_arrow.y;
	}

}
}
