
using System;
using System.Collections.Generic;
using Unity.FPS.Game;
using UnityEngine.Events;
using UnityEngine;

namespace Unity.FPS.Gameplay
{
    /// <summary>
    /// ���� ��ü ����
    /// </summary>
    public enum WeaponSwithState
    {
        Up,
        Down,
        PutDownPrivious,
        PutUpNew,

    }

    /// <summary>
    /// �÷��̾ ���� ������� �����ϴ� Ŭ����
    /// </summary>
    public class PlayerWeaponsManager : MonoBehaviour
    {
        #region Variables
        //���� ���� - ������ �����Ҷ� ó�� �������� ���޵Ǵ� ���� ����Ʈ
        public List<WeaponController> startingWeapons = new List<WeaponController>();

        //���� ����
        //���⸦ �����ϴ� ������Ʈ
        public Transform weaponParentSocket;

        //�÷��̾ �����߿� ��� �ٴϴ� ���� ����Ʈ
        private WeaponController[] weaponSlots = new WeaponController[9];

        //���� ����Ʈ(����)�� Ȱ��ȭ�� ���⸦ �����ϴ� �ε���
        public int ActiveWeaponIndex {  get; private set; }

        //���� ��ü
        public UnityAction<WeaponController> OnSwitchToWeapon;  //���� ��ü�� ��ϵ��Լ� ȣ��

        private WeaponSwithState weaponSwitchState;             //���� ��ü�� ����

        private PlayerInputHandler playerInputHandler;

        //���� ��ü�� ���Ǵ� ��ġ
        private Vector3 weaponMainLocalPosition;

        public Transform defaultWeaponPosition;
        public Transform downWeaponPosition;

        private int weaponSwitchNewIndex;           //���� �ٲ�� ���� �ε���

        private float weaponSwitchTimeStarted = 0f; //
        [SerializeField] private float weaponSwitchDelay = 1f;
        #endregion


        private void Start()
        {
            //����
            playerInputHandler = GetComponent<PlayerInputHandler>();

            //�ʱ�ȭ
            ActiveWeaponIndex = -1;
            weaponSwitchState = WeaponSwithState.Down;

            //
            OnSwitchToWeapon += OnWeaponSwitched;


            //���� ���� ���� ����
            foreach ( var weapon in startingWeapons )
            {
                AddWeapon(weapon);
            }
            SwitchWeapon(true);
        }

        private void Update()
        {
            if (weaponSwitchState == WeaponSwithState.Up || weaponSwitchState == WeaponSwithState.Down)
            {
                int switchWeaponInput = playerInputHandler.GetSwitchWeaponInput();
                if (switchWeaponInput != 0)
                {
                    bool switchUp = switchWeaponInput > 0;
                    SwitchWeapon(switchUp);
                }
            }
        }

        private void LateUpdate()
        {
            UpdateWeaponSwitching();

            //���� ���� ��ġ
            weaponParentSocket.localPosition = weaponMainLocalPosition;
        }

        //���¿� ���� ���� ����
        void UpdateWeaponSwitching()
        {
            //Lerp ����
            float switchingTimeFactor = 0f;
            if(weaponSwitchDelay == 0f)
            {
                switchingTimeFactor = 1f;
            }
            else
            {
                switchingTimeFactor = Mathf.Clamp01((Time.time - weaponSwitchTimeStarted) / weaponSwitchDelay);
            }

            //�����ð� ���� ���� ���� �ٲٱ�
            if(switchingTimeFactor >= 1f)
            {
                if(weaponSwitchState == WeaponSwithState.PutDownPrivious)
                {
                    //���� ���� false, ���ο� ���� true
                    WeaponController oldWeapon = GetActiveWeapon();
                    if (oldWeapon != null)
                    {
                        oldWeapon.ShowWeapon(false);
                    }

                    ActiveWeaponIndex = weaponSwitchNewIndex;
                    WeaponController newWeapon = GetActiveWeapon();
                    OnSwitchToWeapon?.Invoke(newWeapon);

                    switchingTimeFactor = 0f;
                    if(newWeapon != null)
                    {
                        weaponSwitchTimeStarted = Time.time;
                        weaponSwitchState = WeaponSwithState.PutUpNew;
                    }
                    else
                    {
                        weaponSwitchState = WeaponSwithState.Down;
                    }

                }
                else if(weaponSwitchState == WeaponSwithState.PutUpNew)
                {
                    weaponSwitchState = WeaponSwithState.Up;
                }
            }

            //�����ð����� ������ ��ġ �̵�
            if (weaponSwitchState == WeaponSwithState.PutDownPrivious)
            {
                weaponMainLocalPosition = Vector3.Lerp(defaultWeaponPosition.localPosition, downWeaponPosition.localPosition, switchingTimeFactor);
            }
            else
            {
                weaponMainLocalPosition = Vector3.Lerp(downWeaponPosition.localPosition, defaultWeaponPosition.localPosition, switchingTimeFactor);
            }
        }



        //weaponSlots�� ���� ���������� ������ WeaponController ������Ʈ �߰�
        public bool AddWeapon(WeaponController weaponPrefab)
        {
            //�߰��ϴ� ���� ���� ���� üũ - �ߺ� �˻�
            if (HasWeapon(weaponPrefab) != null)
            {
                Debug.Log("Has Same Weapon");

                return false;
            }


            for (int i = 0; i< weaponSlots.Length; i++)
            {
                if (weaponSlots[i] == null)
                {
                    WeaponController weaponInstance = Instantiate(weaponPrefab, weaponParentSocket);
                    weaponInstance.transform.localPosition = Vector3.zero;
                    weaponInstance.transform.localRotation = Quaternion.identity;

                    weaponInstance.Owner = this.gameObject;
                    weaponInstance.SourcePrefab = weaponPrefab.gameObject;
                    weaponInstance.ShowWeapon(false);

                    weaponSlots[i] = weaponInstance;

                    return true;
                }
            }

            Debug.Log("weaponSlots full");
            return false;

        }

        //�Ű������� ����
        private WeaponController HasWeapon(WeaponController weaponPrefab)
        {
            for(int i = 0; i < weaponSlots.Length; i++)
            {
                if(weaponSlots[i] != null && weaponSlots[i].SourcePrefab == weaponPrefab)
                {
                    return weaponSlots[i];
                }
            }

            return null;
        }

        public WeaponController GetActiveWeapon()
        {
            return GetWeaponAtSlotIndex(ActiveWeaponIndex);
        }

        //������ ���Կ� ���Ⱑ �ִ��� ����
        public WeaponController GetWeaponAtSlotIndex(int index)
        {
            if(index >= 0 && index < weaponSlots.Length)
            {
                return weaponSlots[index];
            }

            return null;
        }

        //0~9 0,1,2
        //���� �ٲٱ�, ���� ��� �ִ� ���� false, ���ο� ���� true
        public void SwitchWeapon(bool ascendingOrder)
        {
            int newWeaponIndex = -1;        //���� ��Ƽ���� ���� �ε���
            int closestSlotDistance = weaponSlots.Length;
            for(int i = 0;i < weaponSlots.Length;i++)
            {
                if(i != ActiveWeaponIndex && GetWeaponAtSlotIndex(i) != null)
                {
                    int distanceToActiveIndex = GetDistanceBetWeenWeaponSlot(ActiveWeaponIndex, i, ascendingOrder);
                    if(distanceToActiveIndex < closestSlotDistance)
                    {
                        closestSlotDistance = distanceToActiveIndex;
                        newWeaponIndex = i;
                    }
                }    
            }

            //���� ��Ƽ���� ���� �ε����� ���� ��ü
            SwitchToWeaponIndex(newWeaponIndex);
        }

        private void SwitchToWeaponIndex(int newWeaponIndex)
        {
            //newWeaponIndex �� üũ
            if (newWeaponIndex >= 0 && newWeaponIndex != ActiveWeaponIndex)
            {
                weaponSwitchNewIndex = newWeaponIndex;
                weaponSwitchTimeStarted = Time.time;

                //���� ��Ƽ���� ���Ⱑ �ִ���?
                if(GetActiveWeapon() == null)
                {
                    weaponMainLocalPosition = downWeaponPosition.position;
                    weaponSwitchState = WeaponSwithState.PutUpNew;
                    ActiveWeaponIndex = newWeaponIndex;

                    WeaponController weaponController = GetWeaponAtSlotIndex(newWeaponIndex);
                    OnSwitchToWeapon?.Invoke(weaponController);
                }
                else
                {
                    weaponSwitchState = WeaponSwithState.PutDownPrivious;
                }

                /*if (ActiveWeaponIndex >= 0)
                {
                    WeaponController nowWeapon = GetWeaponAtSlotIndex(ActiveWeaponIndex);
                    nowWeapon.ShowWeapon(false);
                }
                WeaponController newWeapon = GetWeaponAtSlotIndex(newWeaponIndex);
                newWeapon.ShowWeapon(true);

                ActiveWeaponIndex = newWeaponIndex;*/

            }
        }

        //���԰� �Ÿ�
        private int GetDistanceBetWeenWeaponSlot(int fromSlotIndex, int toSlotIndex, bool ascendingOrder)
        {
            int distanceBetweenSlots = 0;

            if(ascendingOrder)
            {
                distanceBetweenSlots = toSlotIndex - fromSlotIndex;
            }
            else
            {
                distanceBetweenSlots = fromSlotIndex - toSlotIndex;
            }

            if(distanceBetweenSlots < 0)
            {
                distanceBetweenSlots = distanceBetweenSlots + weaponSlots.Length;
            }

            return distanceBetweenSlots;
        }

        void OnWeaponSwitched(WeaponController newWeapon)
        {
            if(newWeapon != null)
            {
                newWeapon.ShowWeapon(true);
            }
        }
    }
}