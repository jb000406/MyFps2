using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.FPS.AI
{

    /// <summary>
    /// Enemy ����Ʈ�� �����ϴ� Ŭ����
    /// </summary>

    public class EnemyManager : MonoBehaviour
    {
        #region Variables
        public List<EnemyController> Enemies {  get; private set; }

        public int NumberOfEnemiesTotal { get; private set; }       //�� ����� enemy ���� ��

        public int NumberOfEneiesRemaining => Enemies.Count;        //���� ����ִ� enemy ���� ��
        #endregion

        private void Awake()
        {
            Enemies = new List<EnemyController>();
        }

        //���
        public void RegisterEnemy(EnemyController newEnemy)
        {
            Enemies.Add(newEnemy);

            NumberOfEnemiesTotal++;
        }

        //����
        public void RemoveEnemy(EnemyController killedEnemy)
        {
            Enemies.Remove(killedEnemy);
        }
    }
}