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

        //CrossHair
        public CrossHairData crosshairDefault;              //기본, 평상시
        public CrossHairData crosshairTargetInSight;        //적을 포착했을때, 타겟팅 되었을때

        //조준
        public float aimZoomRatio = 1f;             //조준시 
        public Vector3 aimOffset;
        #endregion

        private void Awake()
        {
            shootAudioSource = this.GetComponent<AudioSource>();
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
    }
}