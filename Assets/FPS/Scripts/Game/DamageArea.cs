using System.Collections.Generic;
using UnityEngine;

namespace Unity.FPS.Game
{
    /// <summary>
    /// 일정 범위 안에 있는 콜라이더 오브젝트 데미지 주기
    /// </summary>
    public class DamageArea : MonoBehaviour
    {
        #region Variables
        [SerializeField] private float areaOfEffectDistance = 10f;
        [SerializeField] private AnimationCurve damageRatioOverDistance;
        #endregion

        public void InflictDamageArea(float damage, Vector3 center, LayerMask layers, QueryTriggerInteraction interaction, GameObject owner)
        {
            Dictionary<Health, Damageable> uniqueDamagedHealth = new Dictionary<Health, Damageable>();

            Collider[] affectedColliers = Physics.OverlapSphere(center, areaOfEffectDistance, layers, interaction);
            foreach (Collider collider in affectedColliers)
            {
                Damageable damageable = collider.GetComponent<Damageable>();
                if (damageable)
                {
                    Health health = damageable.GetComponentInParent<Health>();
                    if (health != null && uniqueDamagedHealth.ContainsKey(health) == false)
                    {
                        uniqueDamagedHealth.Add(health, damageable);
                    }
                }
            }

            //데미지 주기
            foreach(var uniqueDamageable in uniqueDamagedHealth.Values)
            {
                float distance = Vector3.Distance(uniqueDamageable.transform.position, center);
                float curveDamage = damage * damageRatioOverDistance.Evaluate(distance/ areaOfEffectDistance);
                Debug.Log($"distance: {distance} - curveDamage: {curveDamage}");
                uniqueDamageable.InflictDamage(curveDamage, true, owner);
            }
        }
    }
}