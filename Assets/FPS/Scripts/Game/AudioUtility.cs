using UnityEngine;

namespace Unity.FPS.Game
{
    /// <summary>
    /// 오디오 플레이 관련 기능 구현
    /// </summary>
    public class AudioUtility : MonoBehaviour
    {
        //지정된 위치에 게임오브젝트 생성하고 AudioSource 컴포넌트를 추가해서 지정된 클립을 플레이한다
        //클립 사운드 플레이가 끝나면 자동으로 킬한다 - TimeSelfDestruct 컴포넌트 이용
        public static void CreateSfx(AudioClip clip, Vector3 position, float spartialBlend, float rolloffDistanceMin = 1f)
        {
            GameObject impactSfxInstance = new GameObject();
            impactSfxInstance.transform.position = position;

            //audio clip play
            AudioSource source = impactSfxInstance.AddComponent<AudioSource>();
            source.clip = clip;
            source.spatialBlend = spartialBlend;
            source.minDistance = rolloffDistanceMin;
            source.Play();

            //오브젝트 kill
            TimeSelfDestruct timeSelfDestruct = impactSfxInstance.AddComponent<TimeSelfDestruct>();
            timeSelfDestruct.lifeTime = clip.length;
        }
    }
}