using System.Collections;
using System.Collections.Generic;
using Unity.FPS.Game;
using Unity.FPS.Gameplay;
using UnityEngine;
using UnityEngine.UI;

namespace Unity.FPS.UI
{ 

public class PlayerHealthBar : MonoBehaviour
    {
        #region Variables
        private Health playerHealth;
        public Image healthFillinImage;
        #endregion


        private void Start()
        {
            PlayerCharacterController playerCharacterController 
                = GameObject.FindObjectOfType<PlayerCharacterController>(); 

            playerHealth = playerCharacterController.GetComponent<Health>();
        }

        private void Update()
        {
            healthFillinImage.fillAmount = playerHealth.GetRatio();
        }
    }
}