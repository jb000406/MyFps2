using System.Linq;
using Unity.FPS.Game;
using UnityEngine;
using UnityEngine.Events;

namespace Unity.FPS.AI
{
    /// <summary>
    /// 적 디텍팅 구현
    /// </summary>
    public class DetectionModule : MonoBehaviour
    {
        #region Variables
        private ActorManager actorManager;

        public UnityAction OnDetectedTarget;     //적을 감지하면 등록된 함수 호출
        public UnityAction OnLostTarget;         //적을 놓치면 등록됨 함수 호출

        public GameObject KnownDetectedTarget {  get; private set; }

        public bool HadKnownTarget {  get; private set; }

        public bool IsSeeingTarget { get; private set; }

        public Transform detectionSourcePoint;
        public float detectionRange = 20f;          //적 감지 거리

        public float knownTargetTimeout = 4f;
        private float TimeLastSeenTarget = Mathf.NegativeInfinity;

        //attack
        public float attackRange = 10f;            //적 공격 거리

        public bool IsTargetInAttackRange { get; private set; }
        #endregion

        private void Start()
        {
            //참조
            actorManager = GameObject.FindObjectOfType<ActorManager>();
        }

        //디텍팅
        public void HandleTargetDetection(Actor actor, Collider[] selfCollider)
        {
            if(KnownDetectedTarget && !IsSeeingTarget && (Time.time - TimeLastSeenTarget) > knownTargetTimeout)
            {
                KnownDetectedTarget = null;
            }

            float sqrDetectionRange = detectionRange * detectionRange;
            IsSeeingTarget = false;
            float closetSqrdistance = Mathf.Infinity;

            foreach ( var otherActor in actorManager.Actors )
            {
                //아군이면
                if ((otherActor.affiliation == actor.affiliation))
                    continue;
                
                float sqrDistance = (otherActor.aimPoint.position - detectionSourcePoint.position).sqrMagnitude;
                if(sqrDistance < sqrDetectionRange && sqrDistance < closetSqrdistance)
                {
                    RaycastHit[] hits = Physics.RaycastAll(detectionSourcePoint.position,
                        (otherActor.aimPoint.position - detectionSourcePoint.position).normalized, detectionRange,
                        -1, QueryTriggerInteraction.Ignore);

                    RaycastHit cloestHit = new RaycastHit();
                    cloestHit.distance = Mathf.Infinity;
                    bool foundValidHit = false;
                    foreach ( var hit in hits )
                    {
                        if(hit.distance < cloestHit.distance && selfCollider.Contains(hit.collider) == false)
                        {
                            cloestHit = hit;
                            foundValidHit = true;
                        }
                    }

                    //적을 찾았으면
                    if(foundValidHit )
                    {
                        Actor hitActor = cloestHit.collider.GetComponentInParent<Actor>();
                        if(hitActor == otherActor)
                        {
                            IsSeeingTarget = true;
                            closetSqrdistance = sqrDistance;

                            TimeLastSeenTarget = Time.time;
                            KnownDetectedTarget = otherActor.aimPoint.gameObject;
                        }
                    }

                }
                
            }

            //attack Range check
            IsTargetInAttackRange = (KnownDetectedTarget != null) &&
                Vector3.Distance(transform.position, KnownDetectedTarget.transform.position) <= attackRange;


            // 적을 모르고 있다가 적을 발견한 순간에 실행
            if(HadKnownTarget == false && KnownDetectedTarget != null)
            {
                OnDetected();
            }

            // 적을 주시하고 있다가 놓치는 순간 실행
            if (HadKnownTarget == true && KnownDetectedTarget == null)
            {
                OnLost();
            }

            //디텍팅 상태 저장
            HadKnownTarget = KnownDetectedTarget != null;
        }

        

        //적을 감지하면
        public void OnDetected()
        {
            OnDetectedTarget?.Invoke();
        }

        //적을 놓치면
        public void OnLost()
        {
            OnLostTarget?.Invoke();
        }
    }
}