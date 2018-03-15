using System.Collections;
using UnityEngine;
using Pathfinding;
namespace UnityStandardAssets._2D{
public class EnemyAI : MonoBehaviour, Joystick {

	//What to chase?
	public Transform target;
	public GameObject targetGO;
	public Transform targetSec;
	private float distYMin = 2f;
	private float distXMin = 2f;
	private float distXMax = 5f;
	private float timeBtwnAtks = 2f;
	private float timeHoldingAtk = 1f;
	private float distYMax  = 0.7f;

	private float timerHoldingAtk=0;
	private float timerBtwnAtks=0;
	private GMNet gm;

	// How many secs is need to wait until recaculate the path
	public float waitTillUpdate = 1f;

	//Caching
	private Seeker m_seeker;
	private Rigidbody m_rb;

	//The calculated path
	public Path path;

	//The Ai's speed per sec
	public float speed = 300f;
	public ForceMode2D FMode;

	public bool Commented = false;

	[HideInInspector]
	public bool pathIsEnded = false;

	//The max distance from the AI to a waypoint for it to continue to the next waypoint
	public float nextWaypointDistance = 3f;

	public int currentWaypoint = 0;

	private bool m_jump = false;
	private bool m_atk = false;
	private Vector2 m_arrow = Vector2.zero;
	private float sceneDist = 20f;
	public bool isTaggingTargetMain = true;

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
		if(m_atk && timerHoldingAtk < timeHoldingAtk){
			dir = (target.position - transform.position).normalized;
			timerHoldingAtk+=Time.fixedDeltaTime;
			timerBtwnAtks=0;
			m_jump=false;
		}else{
			dir = (path.vectorPath[currentWaypoint] - transform.position);
			dirNormalized = dir.normalized;
			m_atk=false;
			timerBtwnAtks+=Time.fixedDeltaTime;
			if(dir.y > distYMin)
			m_jump = (m_arrow.y > 0.3f);
		}

		float dist3D = Vector3.Distance(transform.position, target.position);
		m_arrow.y = dirNormalized.y;
		m_arrow.x = dirNormalized.x;
		m_arrow.x *= Mathf.Abs(1 - (dist3D / sceneDist));//regula a intencidade da movimentação horizontal.
		

		float distY = Mathf.Abs(transform.position.y - target.position.y);
		if(!m_atk && timerBtwnAtks > timeBtwnAtks){
			if(m_arrow.y < 0.3f && dist3D > distXMax && distY < distYMax){
				m_atk = true;
				timerBtwnAtks=0;
			}else if( dist3D>distXMin && dist3D <distXMax && distY < distYMax){
				m_atk = true;
				timerBtwnAtks=0;
			}
		}

		isTaggingTargetMain = true;
		if(targetSec == null && dist3D>distXMax*1.2f){
			targetSec = gm.randomAvailablePoint("All");
		}

		if(!m_atk && dist3D<distXMin){
			m_jump=true;
		}else if(targetSec != null){
			float distSec = Vector3.Distance(transform.position, targetSec.position);
			if((distSec/dist3D)<=0.7f){
				isTaggingTargetMain = false;
			}
		}



		dist3D = Vector3.Distance(transform.position, path.vectorPath[currentWaypoint]);
		if(dist3D < nextWaypointDistance){
			currentWaypoint++;
			return;
		}



	}

	public bool getAtk(){
		return m_atk;
	}
	public bool getPulo(){
		return m_jump;
	}
	public float getHorizontal(){
		return m_arrow.x;
	}
	public float getVertical(){
		return m_arrow.y;
	}

}
}
