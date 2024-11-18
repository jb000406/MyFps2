using Unity.FPS.Gameplay;
using UnityEngine;

namespace Unity.FPS.UI
{
    public class ScopeUIManager : MonoBehaviour
    {
        #region Variables
        public GameObject scopeUI;

        private PlayerWeaponsManager weaponsManager;
        #endregion

        private void Start()
        {
            //참조
            weaponsManager = GameObject.FindObjectOfType<PlayerWeaponsManager>();
            //Action 함수 등록
            weaponsManager.OnScopedWeapon += OnScope;
            weaponsManager.OffScopedWeapon += OffScope;
        }

        public void OnScope()
        {
            scopeUI.SetActive(true);
        }

        public void OffScope()
        { 
            scopeUI.SetActive(false);
        }
    }
}