using UnityEngine;

namespace Unity.FPS.Game
{
    /// <summary>
    /// 죽었을때 Health를 가진 오브젝트를 킬하는 클래스
    /// </summary>
    public class Destructable : MonoBehaviour
    {
        #region Variables
        private Health health;
        #endregion

        private void Start()
        {
            //참조
            health = GetComponent<Health>();
            DebugUtility.HandleErrorIfNullGetComponent<Health, Destructable>(health, this, gameObject);

            //UnityAction 함수 등록
            health.OnDamaged += OnDamaged;
            health.OnDie += OnDie;
        }

        void OnDamaged(float damage, GameObject damageSource)
        {
            //TODO : 데미지 효과 구현
        }

        void OnDie()
        {
            //킬
            Destroy(gameObject);
        }
    }
}