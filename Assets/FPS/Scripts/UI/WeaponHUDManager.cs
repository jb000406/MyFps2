using UnityEngine;
using Unity.FPS.Game;
using Unity.FPS.Gameplay;

namespace Unity.FPS.UI
{
    public class WeaponHUDManager : MonoBehaviour
    {
        #region Variables
        public RectTransform ammoPanel;             //ammoCountUI 부모 오브젝트
        public GameObject ammoCountPrefab;          //ammoCountUI 프리팹

        private PlayerWeaponsManager playerWeaponsManager;
        #endregion

        private void Awake()
        {
            //참조
            playerWeaponsManager = GameObject.FindObjectOfType<PlayerWeaponsManager>();

            playerWeaponsManager.OnAddedWeapon += AddWeapon;
            playerWeaponsManager.OnRemoveWeapon += RemoveWeapon;
            playerWeaponsManager.OnSwitchToWeapon += SwitchWeapon;
        }

        //무기추가 하면 ammo UI 하나 추가
        void AddWeapon(WeaponController newWeapon, int weaponIndex)
        {
            GameObject ammoCountGo = Instantiate(ammoCountPrefab, ammoPanel);
            AmmoCount ammoCount = ammoCountGo.GetComponent<AmmoCount>();
            ammoCount.Initialzie(newWeapon, weaponIndex);
        }

        //무기 제거 하면 ammo UI 하나 제거
        void RemoveWeapon(WeaponController oldWeapon, int weaponIndex)
        {

        }

        //
        void SwitchWeapon(WeaponController weapon)
        {
            UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(ammoPanel);
        }
    }
}
