using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Unity.FPS.Game
{
    /// <summary>
    /// ü���� �����ϴ� Ŭ����
    /// </summary>
    public class Health : MonoBehaviour
    {
        #region Variables
        [SerializeField] private float maxHealth = 100f;    //�ִ� hp
        public float CurrentHealth { get; private set; }    //���� hp
        private bool isDeath = false;                       //���� üũ

        public UnityAction<float, GameObject> OnDamaged;
        public UnityAction OnDie;
        public UnityAction<float> OnHealed;

        //ü�� ���� �����
        [SerializeField] private float criticalHealRatio = 0.3f;

        //����
        public bool Invincible { get; private set; }
        #endregion

        //�� �������� ������ �ִ��� üũ
        public bool CanPickUp() => CurrentHealth < maxHealth;
        //UI HP ������ ��
        public float GetRatio() => CurrentHealth / maxHealth;
        //���� üũ
        public bool isCritical() => GetRatio() <= criticalHealRatio;

        private void Start()
        {
            //�ʱ�ȭ
            CurrentHealth = maxHealth;
        }

        public void Heal(float amount)
        {

            float beforeHealth = CurrentHealth;
            CurrentHealth += amount;
            CurrentHealth = Mathf.Clamp(CurrentHealth, 0, maxHealth);

            //realheal ���ϱ�
            float realHeal = CurrentHealth - beforeHealth;
            if (realHeal > 0f)
            {
                //�� ����
                OnHealed?.Invoke(realHeal);
            }
        }

        //damageSource: �������� �ִ� ��ü
        public void TakeDamage(float damage, GameObject damageSource)
        {
            //���� üũ
            if(Invincible)
                return;

            float beforeHealth = CurrentHealth;
            CurrentHealth -= damage;
            CurrentHealth = Mathf.Clamp(CurrentHealth, 0, maxHealth);

            //real Damage ���ϱ�
            float realDamage = beforeHealth - CurrentHealth;
            if(realDamage > 0f)
            {
                //������ ����
                OnDamaged?.Invoke(realDamage, damageSource);
            }

            //���� ó��
            HandleDeath();
        }

        //���� ó��
        void HandleDeath()
        {
            //���� üũ
            if (isDeath) 
                return;

            if(CurrentHealth <= 0f)
            {
                isDeath = true;

                //���� ����
                OnDie?.Invoke();
            }
        }

        
    }
}