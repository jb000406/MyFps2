using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.FPS.Game
{
    /// <summary>
    /// Á×¾úÀ»¶§ 
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
            //Å³
            Destroy(gameObject);
        }
    }
}