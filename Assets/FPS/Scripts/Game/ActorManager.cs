using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.FPS.Game
{
    /// <summary>
    /// 게임에 등장하는 Actor들을 관리하는 클래스
    /// </summary>
    public class ActorManager : MonoBehaviour
    {
        #region Variables
        public List<Actor> Actors {  get; private set; }

        public GameObject Player {  get; private set; }
        #endregion

        //플레이어 셋팅
        public void SetPlayer(GameObject player) => Player = player;

        private void Awake()
        {
            Actors = new List<Actor>();
        }
    }
}
