using UnityEngine;
using UnityEngine.Events;

namespace Unity.FPS.Game
{
    /// <summary>
    /// 체력을 관리하는 클래스
    /// </summary>
    public class Health : MonoBehaviour
    {
        #region Variables
        [SerializeField] private float maxHealth = 100f;    //최대 Hp
        public float CurrentHealth { get; private set; }    //현재 Hp
        private bool isDeath = false;                       //죽음 체크

        public UnityAction<float, GameObject> OnDamaged;
        public UnityAction OnDie;
        public UnityAction<float> OnHeal;

        //체력 위험 경계율
        [SerializeField] private float criticalHealRatio = 0.3f;

        //무적
        public bool Invincible { get; private set; }
        #endregion

        //힐 아이템을 먹을수 있는지 체크
        public bool CanPickUp() => CurrentHealth < maxHealth;
        //UI HP 게이지 값
        public float GetRatio() => CurrentHealth / maxHealth;
        //위험 체크
        public bool IsCritical() => GetRatio() <= criticalHealRatio;


        private void Start()
        {
            //초기화
            CurrentHealth = maxHealth;
            Invincible = false;
        }

        //힐
        public void Heal(float amount)
        {
            float beforeHealth = CurrentHealth;
            CurrentHealth += amount;
            CurrentHealth = Mathf.Clamp(CurrentHealth, 0f, maxHealth);

            //real Heal 구하기
            float realHeal = CurrentHealth - beforeHealth;
            if (realHeal > 0f)
            {
                //힐 구현
                OnHeal?.Invoke(realHeal);
            }
        }

        //damageSource: 데미지를 주는 주체
        public void TakeDamage(float damage, GameObject damageSource)
        {
            //무적 체크
            if (Invincible)
                return;

            float beforeHealth = CurrentHealth;
            CurrentHealth -= damage;
            CurrentHealth = Mathf.Clamp(CurrentHealth, 0f, maxHealth);
            //Debug.Log($"{gameObject.name} CurrentHealth: {CurrentHealth}");

            //real Damage 구하기
            float realDamage = beforeHealth - CurrentHealth;
            if (realDamage > 0f)
            {
                //데미지 구현                
                OnDamaged?.Invoke(realDamage, damageSource);
            }

            //죽음 처리
            HandleDeath();
        }

        //죽음 처리 관리
        void HandleDeath()
        {
            //죽음 체크
            if (isDeath)
                return;

            if(CurrentHealth <= 0f)
            {
                isDeath = true;

                //죽음 구현
                OnDie?.Invoke();
            }
        }
    }
}