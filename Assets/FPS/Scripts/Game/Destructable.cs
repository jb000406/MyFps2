using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.FPS.Game
{
    /// <summary>
    /// �׾����� 
    /// </summary>
    public class Destructable : MonoBehaviour
    {
        #region Variables
        private Health health;

        #endregion

        private void Start()
        {
            health = GetComponent<Health>();
            DebugUtility.HandleErrorIfNullGetComponent<Health, Destructable>(health, this, gameObject);

            health.OnDie += OnDie;
        }

        void OnDie()
        {
            //ų
            Destroy(gameObject);
        }
    }
}