using System.Collections;
using System.Collections.Generic;
using Unity.FPS.Gameplay;
using UnityEngine;

namespace Unity.FPS.UI
{
    /// <summary>
    /// 
    /// </summary>
    public class ScopeUIManager : MonoBehaviour
    {
        #region Variables
        public GameObject ScopeUI;

        private PlayerWeaponsManager weaponsManager;
        #endregion

        private void Start()
        {
            //참조
            weaponsManager = GameObject.FindObjectOfType<PlayerWeaponsManager>();

            //이벤트 함수 등록
            weaponsManager.OnScopedWeapon += OnScope;
            weaponsManager.OffScopedWeapon += OffScope;
        }

        public void OnScope()
        {
            ScopeUI.SetActive(true);
        }

        public void OffScope()
        {
            ScopeUI.SetActive(false);
        }

    }
}