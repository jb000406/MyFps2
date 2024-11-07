using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.FPS.Game
{
    /// <summary>
    /// 크로스헤어를 관리하는 데이터
    /// </summary>
    [System.Serializable]
    public struct CrossHairData
    {
        public Sprite CrossHairSprite;
        public float CrossHairSize;
        public Color CrossHairColor;

    }

    /// <summary>
    /// 무기 슛 타입
    /// </summary>
    public enum WeaponShootType
    {
        Manual,
        Automatic,
        Charge,
        Sniper
    }


    /// <summary>
    /// 무기(총기)를 관리하는 클래스
    /// </summary>
    public class WeaponController : MonoBehaviour
    {
        #region Variables
        //무기를 활성화, 비활성화
        public GameObject weaponRoot;

        //
        public GameObject Owner {  get; set; }          //무기의 주인

        public GameObject SourcePrefab { get; set; }    //무기를 생성한 오리지널 프리팹

        public bool IsWeaponActive { get; private set; }    //무기 활성화 여부

        private AudioSource shootAudioSource;
        public AudioClip switchWeaponSfx;

        //shooting
        public WeaponShootType shootType;

        [SerializeField] private float maxAmmo = 8f;            //장전할수 있는 최대 총알 갯수
        private float currentAmmo;

        [SerializeField] private float delayBetweenShots = 0.5f;    //슛 간격
        private float lastTimeShot;                                 //마지막으로 슛한 시간

        //Vfx, Sfx
        public Transform weaponMuzzle;                              //총구 위치
        public GameObject muzzleFlashPrefab;                        //총구 발사 효과
        public AudioClip shootSfx;                                  //총 발사 사운드

        //CrossHair
        public CrossHairData crosshairDefault;              //기본, 평상시
        public CrossHairData crosshairTargetInSight;        //적을 포착했을때, 타겟팅 되었을때

        //조준
        public float aimZoomRatio = 1f;             //조준시 줌인 설정값
        public Vector3 aimOffset;                   //조준시 무기 위치 조정값

        //반동
        public float recoilForce = 0.5f;

        //Projectile
        public ProjectileBase projectilePrefab;


        public Vector3 MuzzleWorldVelocity {  get; private set; }
        private Vector3 lastMuzzlePosition;

        public float CurrentCharge { get; private set; }
        #endregion

        private void Awake()
        {
            //참조
            shootAudioSource = this.GetComponent<AudioSource>();
        }

        private void Start()
        {
            //초기화
            currentAmmo = maxAmmo;
        }



        //무기를 활성화, 비활성화
        public void ShowWeapon(bool show)
        {
            weaponRoot.SetActive(show);

            //this 무기로 변경
            if (show == true && switchWeaponSfx != null)
            {
                //무기 변경 효과음 플레이
                shootAudioSource.PlayOneShot(switchWeaponSfx);
            }

            IsWeaponActive = show;
        }

        //키 입력에 따른 슛 타입 구현
        public bool HandleShootInputs(bool inputDown, bool inputHeld, bool inputUp)
        {
            switch(shootType)
            {
                case WeaponShootType.Manual:
                    if(inputDown)
                    {
                        return TryShoot();
                    }
                    break;
                case WeaponShootType.Automatic:
                    if(inputHeld)
                    {
                       return TryShoot();
                    }
                    break;
                case WeaponShootType.Charge:

                    break;
                case WeaponShootType.Sniper:
                    if(inputDown)
                    {
                        return TryShoot();
                    }
                    break;
            }

            return false;
        }

        bool TryShoot()
        {
            //
            if(currentAmmo >= 1f && (lastTimeShot + delayBetweenShots) < Time.time)
            {
                currentAmmo -= 1f;
                Debug.Log($"currentAmmo: {currentAmmo}");

                HandleShoot();

                return true;
            }

            return false;
        }

        //슛 연출
        void HandleShoot()
        {
            //Vfx
            if (muzzleFlashPrefab)
            {
                GameObject effectGo = Instantiate(muzzleFlashPrefab, weaponMuzzle.position, weaponMuzzle.rotation, weaponMuzzle);
                Destroy(effectGo, 2f);
            }

            //Sfx
            if (shootSfx)
            {
                shootAudioSource.PlayOneShot(shootSfx);
            }

            //마지막으로 슛한 시간 저장
            lastTimeShot = Time.time;
        }
    }
}