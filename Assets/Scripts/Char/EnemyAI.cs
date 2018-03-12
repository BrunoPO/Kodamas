using System.Collections;
using UnityEngine;
using Pathfinding;
namespace UnityStandardAssets._2D{
public class EnemyAI : MonoBehaviour, Joystick {

	//What to chase?
	public Transform target;
	public GameObject targetGO;
	public float distMin = 3f;
	public float distMax = 7f;
	public float timeBtwnAtks = 5;

	private float timerAtks=0;
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

	void Start(){
		m_seeker = GetComponent<Seeker>();
		m_rb = GetComponent<Rigidbody>();

		m_arrow.x = 1f;
		gm = GameObject.Find("GM").GetComponent<GMNet>();

		StartCoroutine(PlayerIn());
		GetComponent<Platformer2DUserControl>().setController(this);
		sceneDist = Camera.main.GetComponent<Camera2DFollow>().getSceneDist();
	}
	IEnumerator PlayerIn(){
		yield return new WaitForSeconds (3);
		gm.PlayerIn (gameObject);
		StartCoroutine(UpdatePath());
	}
	IEnumerator UpdatePath(){
		if(target == null){
			searchTarget();
		}else{
			//Start a new path to the target position, return the result to OnPathComplete method
			m_seeker.StartPath (transform.position,target.position,OnPathComplete);
		}

		yield return new WaitForSeconds (waitTillUpdate);
		StartCoroutine(UpdatePath());
	}

	private void searchTarget(){
		target = gm.getEnemy(GetComponent<PlatformerCharacter2D>().getHash());
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
		if(target == null){
			timerAtks=timeBtwnAtks;
			return;
		}

		if(path == null){
			timerAtks=timeBtwnAtks;
			return;
		}

		if(currentWaypoint >= path.vectorPath.Count){
			if(pathIsEnded)
				return;

			if(Commented) Debug.Log("End of path reached");
			pathIsEnded = true;
			return;
		}
		pathIsEnded = false;

		Vector3 dir = (path.vectorPath[currentWaypoint] - transform.position).normalized;
		
		m_arrow.x = dir.x;
		m_arrow.y = dir.x;
		m_jump = (m_arrow.y > 0.3f);

		float dist = Vector3.Distance(transform.position, target.position);
		m_arrow.x *= Mathf.Abs(1 - (dist / sceneDist));//regula a intencidade da movimentação horizontal.

		if(m_arrow.y < 0.3f && dist > distMax){
			if(timerAtks > timeBtwnAtks*5){
				m_atk = true;
				timerAtks=0;
			}
		}else if(dist>distMin && dist <distMax){
			if(timerAtks > timeBtwnAtks*5){
				m_atk = true;
				timerAtks=0;
			}
		}else{
			m_atk=false;
		}
		timerAtks++;

		dist = Vector3.Distance(transform.position, path.vectorPath[currentWaypoint]);
		if(dist < nextWaypointDistance){
			currentWaypoint++;
			return;
		}



	}

	public bool getAtk(){
		bool m_auxAtk = m_atk;
		m_atk = false;
		return m_auxAtk;
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
