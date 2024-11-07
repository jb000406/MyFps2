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
            //����
            weaponsManager = GameObject.FindObjectOfType<PlayerWeaponsManager>();

            //�̺�Ʈ �Լ� ���
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