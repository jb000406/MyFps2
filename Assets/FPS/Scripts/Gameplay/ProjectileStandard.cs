using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.FPS.Game;
using Unity.FPS.Gameplay;

namespace Unity.FPS.Gameplay
{
    /// <summary>
    /// 발사체 표준형
    /// </summary>
    public class ProjectileStandard : ProjectileBase
    {
        #region Variables
        //생성
        private ProjectileBase projectileBase;
        private float maxLiftTime = 5f;

        //이동
        public float speed = 20f;
        public float gravityDown = 0f;
        public Transform root;
        public Transform tip;

        private Vector3 velocity;
        private Vector3 lastRootPosition;
        private float shootTime;

        //충돌
        public float radius = 0.01f;                   //충돌 검사하는 구체의 반경

        public LayerMask hittableLayers = -1;           //Hit가 가능한 Layer
        private List<Collider> ignoredColliers;         //Hit 판정시 무시하는 충돌체 리스트

        //충돌 연출
        public GameObject impackVfxPrefab;              //타격 이펙트
        [SerializeField] private float impactVfxLifeTime = 5f;
        private float impactVfxSpawnOffset = 0.1f;

        public AudioClip impactSfxClip;                //타격 효과음

        //데미지
        public float damage = 20f;
        private DamageArea damageArea;
        #endregion

        private void OnEnable()
        {
            projectileBase = GetComponent<ProjectileBase>();
            projectileBase.OnShoot += OnShoot;

            damageArea = GetComponent<DamageArea>();

            Destroy(gameObject, maxLiftTime);
        }


        //shoot 값 설정
        new void OnShoot()
        {
            velocity = transform.forward * speed;
            transform.position += projectileBase.InheritedMuzzleVelocity * Time.deltaTime;

            lastRootPosition = root.position;

            //무시 충돌 리스트 생성 - projectil을 발사하는 자신의 모든 충돌체를 가져와서 등록
            ignoredColliers = new List<Collider>();
            Collider[] ownerColliders = projectileBase.Owner.GetComponentsInChildren<Collider>();
            ignoredColliers.AddRange(ownerColliders);

            //프로젝타일이 벽을 뚫고 날아가는 버그 수정
            PlayerWeaponsManager weaponsManager = projectileBase.Owner.GetComponent<PlayerWeaponsManager>();
            if (weaponsManager)
            {
                Vector3 cameraToMuzzle = projectileBase.InitialPosition - weaponsManager.weaponCamera.transform.position;
                if(Physics.Raycast(weaponsManager.weaponCamera.transform.position, cameraToMuzzle.normalized,
                    out RaycastHit hit, cameraToMuzzle.magnitude, hittableLayers,
                    QueryTriggerInteraction.Collide))
                {
                    if (IsHitValid(hit))
                    {
                        OnHit(hit.point, hit.normal, hit.collider);
                    }
                }
            }
        }

        private void Update()
        {
            //이동
            transform.position += velocity * Time.deltaTime;

            //중력
            if (gravityDown > 0f)
            {
                velocity += Vector3.down * gravityDown * Time.deltaTime;
            }

            //충돌
            RaycastHit cloestHit = new RaycastHit();
            cloestHit.distance = Mathf.Infinity;
            bool foundHit = false;                  //hit한 충돌체를 찾았는지 여부

            //Sphere Cast
            Vector3 displacementSinceLastFrame = tip.position - lastRootPosition;
            RaycastHit[] hits = Physics.SphereCastAll(lastRootPosition, radius,
                displacementSinceLastFrame.normalized, displacementSinceLastFrame.magnitude,
                hittableLayers, QueryTriggerInteraction.Collide);

            foreach (var hit in hits)
            {
                if(IsHitValid(hit) && hit.distance < cloestHit.distance)
                {
                    foundHit = true;
                    cloestHit = hit;
                }
            }

            //hit한 충돌체를 찾았다
            if (foundHit)
            {
                if(cloestHit.distance <= 0f)
                {
                    cloestHit.point = root.position;
                    cloestHit.normal = -transform.forward;
                }

                OnHit(cloestHit.point, cloestHit.normal, cloestHit.collider);
            }

            lastRootPosition = root.position;
        }

        //유효한 hit인지 판정
        bool IsHitValid(RaycastHit hit)
        {
            //IgnoreHitDectection 컴포넌트를 가진 콜라이더 무시
            if(hit.collider.GetComponent<IgnoreHitDectection>())
            {
                return false;
            }

            //ignoredColliers에 포함된 콜라이더 무시
            if (ignoredColliers != null && ignoredColliers.Contains(hit.collider))
            {
                return false;
            }

            //trigger collider가 Damageable이 없어야 된다
            if (hit.collider.isTrigger && hit.collider.GetComponent<Damageable>() == null)
            {
                return false;
            }

            return true;
        }

        //Hit 구현: 데미지, Vfx, Sfx,
        void OnHit(Vector3 point, Vector3 normal, Collider collider)
        {
            //데미지
            if(damageArea)
            {
                damageArea.InflictDamageArea(damage, point, hittableLayers, QueryTriggerInteraction.Collide, projectileBase.Owner);
            }
            else
            {
                Damageable damageable = collider.GetComponent<Damageable>();
                if (damageable)
                {
                    damageable.InflictDamage(damage, false, projectileBase.Owner);
                }
            }
            

            //Vfx
            if (impackVfxPrefab)
            {
                GameObject impactObject = Instantiate(impackVfxPrefab, point + (normal * impactVfxSpawnOffset), Quaternion.LookRotation(normal));
                if(impactVfxLifeTime > 0f)
                {
                    Destroy(impactObject, impactVfxLifeTime);
                }
            }

            //Sfx
            if(impactSfxClip)
            {
                //충돌위치에 게임오브젝트을 생성하고 AudioSource 컴포넌트를 추가해서 지정된 클립을 플레이한다
                AudioUtility.CreateSfx(impactSfxClip, point, 1f, 3f);
            }

            //발사체 킬
            Destroy(gameObject);
        }


    }
}
