using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Unity.FPS.Game
{
    /// <summary>
    /// �߻�ü�� �⺻�� �Ǵ� �θ� Ŭ����
    /// </summary>
    public abstract class ProjectileBase : MonoBehaviour
    {
        #region Variables
        public GameObject Owner { get; private set; }   //�߻��� ��ü

        public Vector3 InitialPosition { get; private set; }

        public Vector3 InitailDirection { get; private set; }

        public Vector3 InheritedMuzzleVelocity { get; private set; }

        public float InitialCharge { get; private set; }

        public UnityAction OnShoot;                         //�߻�� ��ϵ� �Լ� ȣ��


        #endregion

        public void Shoot(WeaponController controller)
        {
            //�ʱ�ȭ
            Owner = controller.Owner;
            InitialPosition = this.transform.position;
            InitailDirection = this.transform.forward;
            InheritedMuzzleVelocity = controller.MuzzleWorldVelocity;
            InitialCharge = controller.CurrentCharge;

            OnShoot?.Invoke();
        }
    }
}