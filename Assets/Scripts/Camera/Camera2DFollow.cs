using System;
using UnityEngine;
using UnityEngine.UI;

namespace UnityStandardAssets._2D
{
    public class Camera2DFollow : MonoBehaviour{

		public GameObject m_GameHUD;
		public GameObject m_EndingImg;
		public Text m_StonesTxt;
		public Text m_LifeTxt;
		public Text m_WinTxt;

        

        [Header("Configurações da Camera")]
        [SerializeField] private float CamPlayerZoom = 0f;
        [SerializeField] private float CamSceneZoom = 0f;
        public Transform target ;//Poderá ser gravado pelo Char para que esse seja o foco.
		[Range(0, 2)] [SerializeField] private float damping = 1;
		[Range(0, 6)] [SerializeField] private float lookAheadFactor = 3;
		[Range(0, 2)] [SerializeField] private float lookAheadReturnSpeed = 0.5f;
		[Range(0, 2)] [SerializeField] private float lookAheadMoveThreshold = 0.1f;
		[SerializeField] private Vector3 m_LookAheadPos;
		private Vector2 raioDeVisao;//Vetor reponsável por saber o raio de visão da camera(Bounds)

		private float m_OffsetZ;
		private Vector3 m_LastTargetPosition;
		private Vector3 m_CurrentVelocity;
		private Transform GMTrans;
		private float viewSize = 0;

		[Header("Pontos limites da camera")]
		[SerializeField] private Vector2 Min;
		[SerializeField] private Vector2 Max;

        private void Start(){
            m_LastTargetPosition = target.position;
            m_OffsetZ = (transform.position - target.position).z;
			raioDeVisao.x = Camera.main.ViewportToWorldPoint (new Vector3 (1, 1, 0)).x - transform.position.x;
			raioDeVisao.y = Camera.main.ViewportToWorldPoint (new Vector3 (1, 1, 0)).y - transform.position.y;
			viewSize = Camera.main.orthographicSize;

			if(GameObject.Find ("GM"))
				GMTrans = GameObject.Find ("GM").transform;
			
			print (transform.position);
			print (raioDeVisao);
        }

		public float getSceneDist(){
			return Vector2.Distance(Min,Max);;
		}

		//Desenhando pontos delimitadores no momento de seleção do objeto para facilitar a vizualização do limites
		void OnDrawGizmosSelected() {
			if (target != null) {
				Gizmos.color = Color.blue;
				Gizmos.DrawLine(transform.position, Min);
				Gizmos.DrawLine(transform.position, Max);
			}
		}


        // Update is called once per frame
        private void Update()
        {
			
			
			if (target == null) {
				target = this.transform;
			}
			if (target == this.transform) {
				if (GMTrans != null) {
					target = GMTrans;
				} else {
					if(GameObject.Find ("GM"))
						GMTrans = GameObject.Find ("GM").transform;
				}
			}

			if (target == GMTrans) {
				if (viewSize <= CamSceneZoom) {
					viewSize += 0.1f;
					Camera.main.orthographicSize = viewSize;
				}
			} else if(viewSize> CamPlayerZoom){
				viewSize -= 0.1f;
				Camera.main.orthographicSize = viewSize;
			}
            // only update lookahead pos if accelerating or changed direction
            float xMoveDelta = (target.position - m_LastTargetPosition).x;

            bool updateLookAheadTarget = Mathf.Abs(xMoveDelta) > lookAheadMoveThreshold;

            if (updateLookAheadTarget)
            {
                m_LookAheadPos = lookAheadFactor*Vector3.right*Mathf.Sign(xMoveDelta);
            }
            else
            {
                m_LookAheadPos = Vector3.MoveTowards(m_LookAheadPos, Vector3.zero, Time.deltaTime*lookAheadReturnSpeed);
            }

            Vector3 aheadTargetPos = target.position + m_LookAheadPos + Vector3.forward*m_OffsetZ;
            Vector3 newPos = Vector3.SmoothDamp(transform.position, aheadTargetPos, ref m_CurrentVelocity, damping);
			newPos.z = -10;

			newPos.x = Mathf.Clamp (newPos.x, Min.x+raioDeVisao.x, Max.x-raioDeVisao.x);
			newPos.y = Mathf.Clamp (newPos.y, Min.y+raioDeVisao.y, Max.y-raioDeVisao.y);

            transform.position = newPos;

            m_LastTargetPosition = target.position;
        }
    }
}
