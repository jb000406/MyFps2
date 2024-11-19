using System.Collections.Generic;
using Unity.FPS.Game;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AI;

namespace Unity.FPS.AI
{
    /// <summary>
    /// 렌더러 데이터: 메터리얼 정보 저장
    /// </summary>
    [System.Serializable]
    public struct RendererIndexData
    {
        public Renderer renderer;
        public int metarialIndx;

        public RendererIndexData(Renderer _renderer, int index)
        {
            renderer = _renderer;
            metarialIndx = index;
        }
    }

    /// <summary>
    /// Enemy 를 관리하는 클래스
    /// </summary>
    public class EnemyController : MonoBehaviour
    {
        #region Variables
        private Health health;

        //death
        public GameObject deathVfxPrefab;
        public Transform deathVfxSpawnPostion;

        //damamge
        public UnityAction Damaged;

        //Sfx
        public AudioClip damageSfx;

        //Vfx
        public Material bodyMaterial;           //데미지를 줄 메터리얼
        [GradientUsage(true)]
        public Gradient OnHitBodyGradient;      //데미지를 컬러 그라디언트 효과로 표현

        //body Material을 가지고 있는 렌더러 데이터 리스트
        private List<RendererIndexData> bodyRenderer = new List<RendererIndexData>();
        MaterialPropertyBlock bodyFlashMaterialPropertyBlock;

        [SerializeField] private float flashOnHitDuration = 0.5f;
        float lastTimeDamaged = float.NegativeInfinity;
        bool wasDamagedThisFrame = false;

        //Patrol
        public NavMeshAgent Agent { get; private set; }
        public PatrolPath PatrolPath { get; set; }
        private int pathDestinationIndex;               //목표 웨이포인트 인덱스
        private float pathReachingRadius = 1f;          //도착판정

        //Detection
        private Actor actor;
        private Collider[] selfColliders;

        public DetectionModule DetectionModule { get; private set; }

        public GameObject KnownDetectedTarget => DetectionModule.KnownDetectedTarget;

        public bool IsSeeingTarget => DetectionModule.IsSeeingTarget;

        public bool HadKnownTarget => DetectionModule.HadKnownTarget;

        public Material eyeColorMaterial;
        [ColorUsage(true, true)] public Color defaultEyeColor;
        [ColorUsage(true, true)] public Color attackEyeColor;

        //eye Material을 가지고 있는 렌더러 데이터
        private RendererIndexData eyeRendererData;
        private MaterialPropertyBlock eyeColorMatarialPropertyBlock;

        public UnityAction OnDetectedTarget;
        public UnityAction OnLostTarget;

        //Attack
        public UnityAction OnAttack;

        private float orientSpeed = 10f;

        public bool IsTargetInAttackRange => DetectionModule.IsTargetInAttackRange;

        public bool swapToNextWeapon = false;
        public float delayAfterWeaponSwap = 0f;
        private float lastTimeWeaponSwapped = Mathf.NegativeInfinity;

        public int currentWeaponIndex;
        private WeaponController currentWeapon;
        private WeaponController[] weapons;

        //enemymanager
        private EnemyManager enemyManager;
        #endregion

        private void Start()
        {
            //참조
            enemyManager = GameObject.FindObjectOfType<EnemyManager>();
            enemyManager.RegisterEnemy(this);                           // //enemyManager 리스트에 제거

            Agent = GetComponent<NavMeshAgent>();

            actor = GetComponent<Actor>();
            selfColliders = GetComponentsInChildren<Collider>();

            var detectionModules = GetComponentsInChildren<DetectionModule>();
            DetectionModule = detectionModules[0];
            DetectionModule.OnDetectedTarget += OnDetected;
            DetectionModule.OnLostTarget += OnLost;

            health = GetComponent<Health>();
            health.OnDamaged += OnDamaged;
            health.OnDie += OnDie;

            //무기 초기화
            FindAndInitializeAllWeapons();
            var weapon = GetCurrentWeapon();
            weapon.ShowWeapon(true);

            //body Material을 가지고 있는 렌더러 정보 리스트 만들기
            Renderer[] renderers = GetComponentsInChildren<Renderer>(true);
            foreach (var renderer in renderers)
            {
                for (int i = 0; i < renderer.sharedMaterials.Length; i++)
                {
                    //body
                    if (renderer.sharedMaterials[i] == bodyMaterial)
                    {
                        bodyRenderer.Add(new RendererIndexData(renderer, i));
                    }

                    //eye
                    if (renderer.sharedMaterials[i] == eyeColorMaterial)
                    {
                        eyeRendererData = new RendererIndexData(renderer, i);
                    }
                }
            }

            //body
            bodyFlashMaterialPropertyBlock = new MaterialPropertyBlock();

            //eye
            if (eyeRendererData.renderer)
            {
                eyeColorMatarialPropertyBlock = new MaterialPropertyBlock();
                eyeColorMatarialPropertyBlock.SetColor("_EmissionColor", defaultEyeColor);
                eyeRendererData.renderer.SetPropertyBlock(eyeColorMatarialPropertyBlock,
                    eyeRendererData.metarialIndx);
            }
        }

        private void Update()
        {
            //디텍션
            //HandleTargetDetection();
            DetectionModule.HandleTargetDetection(actor, selfColliders);

            //데미지 효과
            Color currentColor = OnHitBodyGradient.Evaluate((Time.time - lastTimeDamaged) / flashOnHitDuration);
            bodyFlashMaterialPropertyBlock.SetColor("_EmissionColor", currentColor);
            foreach (var data in bodyRenderer)
            {
                data.renderer.SetPropertyBlock(bodyFlashMaterialPropertyBlock, data.metarialIndx);
            }

            //
            wasDamagedThisFrame = false;
        }

        private void OnDamaged(float damage, GameObject damageSource)
        {
            if (damageSource && damageSource.GetComponent<EnemyController>() == null)
            {
                //등록된 함수 호출
                Damaged?.Invoke();

                //데미지를 준 시간
                lastTimeDamaged = Time.time;

                //Sfx
                if (damageSfx && wasDamagedThisFrame == false)
                {
                    AudioUtility.CreateSfx(damageSfx, this.transform.position, 0f);
                }
                wasDamagedThisFrame = true;
            }
        }

        private void OnDie()
        {
            //enemyManager 리스트에 제거
            enemyManager.RemoveEnemy(this);

            //폭발 효과
            GameObject effectGo = Instantiate(deathVfxPrefab, deathVfxSpawnPostion.position, Quaternion.identity);
            Destroy(effectGo, 5f);

            //Enemy 킬
            Destroy(gameObject);
        }

        //패트롤이 유효한지? 패트롤이 가능한지?
        private bool IsPathVaild()
        {
            return PatrolPath && PatrolPath.wayPoints.Count > 0;
        }

        //가장 가까운 WayPoint 찾기
        private void SetPathDestinationToClosestWayPoint()
        {
            if (IsPathVaild() == false)
            {
                pathDestinationIndex = 0;
                return;
            }

            int closestWayPointIndex = 0;
            for (int i = 0; i < PatrolPath.wayPoints.Count; i++)
            {
                float distance = PatrolPath.GetDistanceToWayPoint(transform.position, i);
                float closestDistance = PatrolPath.GetDistanceToWayPoint(transform.position, closestWayPointIndex);
                if (distance < closestDistance)
                {
                    closestWayPointIndex = i;
                }
            }
            pathDestinationIndex = closestWayPointIndex;
        }

        //목표 지점의 위치 값 얻어오기
        public Vector3 GetDestinationOnPath()
        {
            if (IsPathVaild() == false)
            {
                return this.transform.position;
            }

            return PatrolPath.GetPositionOfWayPoint(pathDestinationIndex);
        }

        //목표 지점 설정 - Nav 시스템 이용
        public void SetNavDestination(Vector3 destination)
        {
            if (Agent)
            {
                Agent.SetDestination(destination);
            }
        }

        //도착 판정 후 다음 목표지점 설정
        public void UpdatePathDestination(bool inverseOrder = false)
        {
            if (IsPathVaild() == false)
                return;

            //도착판정
            float distance = (transform.position - GetDestinationOnPath()).magnitude;
            if (distance <= pathReachingRadius)
            {
                pathDestinationIndex = inverseOrder ? (pathDestinationIndex - 1) : (pathDestinationIndex + 1);
                if (pathDestinationIndex < 0)
                {
                    pathDestinationIndex += PatrolPath.wayPoints.Count;
                }
                if (pathDestinationIndex >= PatrolPath.wayPoints.Count)
                {
                    pathDestinationIndex -= PatrolPath.wayPoints.Count;
                }
            }
        }

        //
        public void OrientTowatd(Vector3 lookPosition)
        {
            Vector3 lookDirect = Vector3.ProjectOnPlane(lookPosition - transform.position, Vector3.up).normalized;
            if (lookDirect.sqrMagnitude != 0)
            {
                Quaternion targetRotation = Quaternion.LookRotation(lookDirect);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, orientSpeed);
            }
        }


        //적 감시지 호출되는 함수
        private void OnDetected()
        {
            OnDetectedTarget?.Invoke();

            if (eyeRendererData.renderer)
            {
                eyeColorMatarialPropertyBlock.SetColor("_EmissionColor", attackEyeColor);
                eyeRendererData.renderer.SetPropertyBlock(eyeColorMatarialPropertyBlock,
                    eyeRendererData.metarialIndx);
            }
        }

        //적 잃어버렸을때 호출되는 함수
        private void OnLost()
        {
            OnLostTarget?.Invoke();

            if (eyeRendererData.renderer)
            {
                eyeColorMatarialPropertyBlock.SetColor("_EmissionColor", defaultEyeColor);
                eyeRendererData.renderer.SetPropertyBlock(eyeColorMatarialPropertyBlock,
                    eyeRendererData.metarialIndx);
            }
        }

        //가지고 있는 무기 찾고 초기화
        private void FindAndInitializeAllWeapons()
        {
            if (weapons == null)
            {
                weapons = this.GetComponentsInChildren<WeaponController>();

                for (int i = 0; i < weapons.Length; i++)
                {
                    weapons[i].Owner = this.gameObject;
                }
            }
        }

        //지정한 인덱스에 해당하는 무기를 current로 지정
        private void SetCurrentWeapon(int index)
        {
            currentWeaponIndex = index;
            currentWeapon = weapons[currentWeaponIndex];
            if (swapToNextWeapon)
            {
                lastTimeWeaponSwapped = Time.time;
            }
            else
            {
                lastTimeWeaponSwapped = Mathf.NegativeInfinity;
            }
        }

        //현재 current weapon 찾기
        public WeaponController GetCurrentWeapon()
        {
            FindAndInitializeAllWeapons();
            if (currentWeapon == null)
            {
                SetCurrentWeapon(0);
            }

            return currentWeapon;
        }

        //적에게 총구를 돌린다
        public void OrientWeaponsToward(Vector3 lookPosition)
        {
            for (int i = 0; i < weapons.Length; i++)
            {
                Vector3 weaponForward = (lookPosition - weapons[i].transform.position).normalized;
                weapons[i].transform.forward = weaponForward;
            }

        }

        //공격 - 공격 성공, 실패
        public bool TryAttack(Vector3 targetPosition)
        {
            //무기 교체시 딜레이 시간동안 공격 불능
            if(lastTimeWeaponSwapped + delayAfterWeaponSwap >= Time.time)
            {
                return false;
            }

            //무기 Shoot
            bool didFire = GetCurrentWeapon().HandleShootInputs(false, true, false);

            if (didFire && OnAttack != null)
            {
                OnAttack?.Invoke();

                //발사를 한번 할때 마다 다음 무기로 교체
                if(swapToNextWeapon == true && weapons.Length > 1)
                {
                    int nextWeaponIndex = currentWeaponIndex + 1% weapons.Length;
                    SetCurrentWeapon(nextWeaponIndex);
                }


            }


            return true;
        }

    }
}


