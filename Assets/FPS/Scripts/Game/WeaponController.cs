using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.FPS.Game
{
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