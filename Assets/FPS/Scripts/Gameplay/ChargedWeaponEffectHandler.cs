using Unity.FPS.Game;
using UnityEngine;

namespace Unity.FPS.Gameplay
{
    public class ChargedWeaponEffectHandler : MonoBehaviour
    {
        #region Variables
        public GameObject chargingObject;               //충전하는 발사체
        public GameObject spiningFrame;                 //발사체를 감싸고 있는 회전하는 프레임
        public GameObject distOrbitParticePrefab;       //발사체를 감싸고 있는 회전하는 이펙트

        public MinMaxVector3 scale;                     //발사체의 크기 설정값

        [SerializeField] private Vector3 offset;
        public Transform parentTransform;

        public MinMaxFloat orbitY;                      //이펙트 설정값
        public MinMaxVector3 radius;                    //이펙트 설정값

        public MinMaxFloat spiningSpeed;                //회전 설정값

        //sfx
        public AudioClip chargeSound;
        public AudioClip loopChargeWeaponSfx;

        private float fadeLoopDuration = 0.5f;
        [SerializeField] public bool useProceduralPitchOnLoop;

        public float maxProceduralPitchValue = 2.0f;

        private AudioSource audioSource;
        private AudioSource audioSourceLoop;

        //
        public GameObject particleInstance {  get; private set; }
        private ParticleSystem diskOrbitParticle;
        private ParticleSystem.VelocityOverLifetimeModule velocityOverLifetimeModule;

        private WeaponController weaponController;

        private float lastChargeTriggerTimeStamp;
        private float endChargeTime;
        private float chargeRatio;                  //현재 충전률
        #endregion

        private void Awake()
        {
            //chargeSound  play
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.clip = chargeSound;
            audioSource.playOnAwake = false;

            //loopChargeWeaponSfx play
            audioSourceLoop = gameObject.AddComponent<AudioSource>();
            audioSourceLoop.clip = loopChargeWeaponSfx;
            audioSourceLoop.playOnAwake = false;
            audioSourceLoop.loop = true;
        }

        void SpawParticleSystem()
        {
            particleInstance = Instantiate(distOrbitParticePrefab, parentTransform != null ? parentTransform : transform);
            particleInstance.transform.localPosition += offset;

            FindReference();
        }

        void FindReference()
        {
            diskOrbitParticle = particleInstance.GetComponent<ParticleSystem>();
            velocityOverLifetimeModule = diskOrbitParticle.velocityOverLifetime;

            weaponController = GetComponent<WeaponController>();
        }

        private void Update()
        {
            //한번만 객체 만들기
            if (particleInstance == null)
            {
                SpawParticleSystem();
            }
                        
            diskOrbitParticle.gameObject.SetActive(weaponController.IsWeaponActive);
            chargeRatio = weaponController.CurrentCharge;
                        
            //disk, frame
            chargingObject.transform.localScale = scale.GetValueFromRatio(chargeRatio);
            if(spiningFrame)
            {
                spiningFrame.transform.localRotation *= Quaternion.Euler( 0f,
                    spiningSpeed.GetValueFromRatio(chargeRatio) * Time.deltaTime, 
                    0f );
            }
            ////VFX particle
            velocityOverLifetimeModule.orbitalY = orbitY.GetValueFromRatio(chargeRatio);
            diskOrbitParticle.transform.localScale = radius.GetValueFromRatio(chargeRatio);

            //SFX
            if(chargeRatio > 0f)
            {
                if(audioSourceLoop.isPlaying == false &&
                    weaponController.lastChargeTriggerTimeStamp > lastChargeTriggerTimeStamp)
                {
                    lastChargeTriggerTimeStamp = weaponController.lastChargeTriggerTimeStamp;
                    if(useProceduralPitchOnLoop == false)
                    {
                        endChargeTime = Time.time + chargeSound.length;
                        audioSource.Play();
                    }
                    audioSourceLoop.Play();
                }

                if (useProceduralPitchOnLoop == false)  //두개의 사운드 페이드 효과로 충전 표현
                {
                    float volumeRatio = Mathf.Clamp01((endChargeTime - Time.time - fadeLoopDuration) / fadeLoopDuration);
                    audioSource.volume = volumeRatio;
                    audioSourceLoop.volume = 1f - volumeRatio;
                }
                else  //루프사운드의 재생속도로 충전 표현
                {
                    audioSourceLoop.pitch = Mathf.Lerp(1.0f, maxProceduralPitchValue, chargeRatio);
                }

            }
            else
            {
                audioSource.Stop();
                audioSourceLoop.Stop();
            }
        }
    }
}