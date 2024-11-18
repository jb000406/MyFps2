using UnityEngine;
using Unity.FPS.Game;

namespace Unity.FPS.Gameplay
{
    /// <summary>
    /// 충전용 발사체를 발사할때 충전량에 발사체의 게임오브젝트 크기 결정
    /// </summary>
    public class ChargedProjectileEffectHandler : MonoBehaviour
    {
        #region Variables
        private ProjectileBase projectileBase;

        public GameObject charageObejct;
        public MinMaxVector3 scale;
        #endregion

        private void OnEnable()
        {
            //참조
            projectileBase = GetComponent<ProjectileBase>();
            projectileBase.OnShoot += OnShoot;
        }

        void OnShoot()
        {
            charageObejct.transform.localScale = scale.GetValueFromRatio(projectileBase.InitialCharge);
        }
    }
}