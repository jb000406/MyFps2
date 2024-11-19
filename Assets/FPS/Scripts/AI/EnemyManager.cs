using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.FPS.AI
{

    /// <summary>
    /// Enemy 리스트를 관리하는 클래스
    /// </summary>

    public class EnemyManager : MonoBehaviour
    {
        #region Variables
        public List<EnemyController> Enemies {  get; private set; }

        public int NumberOfEnemiesTotal { get; private set; }       //총 생산된 enemy 수의 합

        public int NumberOfEneiesRemaining => Enemies.Count;        //현재 살아있는 enemy 수의 합
        #endregion

        private void Awake()
        {
            Enemies = new List<EnemyController>();
        }

        //등록
        public void RegisterEnemy(EnemyController newEnemy)
        {
            Enemies.Add(newEnemy);

            NumberOfEnemiesTotal++;
        }

        //제거
        public void RemoveEnemy(EnemyController killedEnemy)
        {
            Enemies.Remove(killedEnemy);
        }
    }
}