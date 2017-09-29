using System;
using UnityEngine;

namespace UnityStandardAssets._2D
{
    public class Camera2DFollow : MonoBehaviour
    {
        public Transform target;
        public float damping = 1;
        public float lookAheadFactor = 3;
        public float lookAheadReturnSpeed = 0.5f;
        public float lookAheadMoveThreshold = 0.1f;

        private float m_OffsetZ;
        private Vector3 m_LastTargetPosition;
        private Vector3 m_CurrentVelocity;
        private Vector3 m_LookAheadPos;
		public Vector2 Min,Max;

		Vector2 raioDeVisao;
        // Use this for initialization
        private void Start(){
            m_LastTargetPosition = target.position;
            m_OffsetZ = (transform.position - target.position).z;
            transform.parent = null;
			//Camera.main.ViewportToWorldPoint(
			raioDeVisao.x = Camera.main.ViewportToWorldPoint (new Vector3 (1, 1, 0)).x - transform.position.x;
			raioDeVisao.y = Camera.main.ViewportToWorldPoint (new Vector3 (1, 1, 0)).y - transform.position.y;
			print (transform.position);
			print (raioDeVisao);
        }

		//DrawPoint
		void OnDrawGizmosSelected() {
			if (target != null) {
				Gizmos.color = Color.blue;
				Gizmos.DrawLine(transform.position, Min);
				Gizmos.DrawLine(transform.position, Max);
				/*Gizmos.DrawLine(transform.position, East);
				Gizmos.DrawLine(transform.position, West);*/
			}
		}


        // Update is called once per frame
        private void Update()
        {
			
			if (target == null) {
				target = this.transform;
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
