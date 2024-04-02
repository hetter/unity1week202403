using MoreMountains.Feedbacks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace DummyEgg.ProjectGK
{
    public class EasyAudioManager : Singleton<EasyAudioManager>
    {
        [SerializeField] public MMF_Player Bgm;
        [SerializeField] public MMF_Player HitEffect;
        [SerializeField] public MMF_Player ShootEffect;
        [SerializeField] public MMF_Player CoinEffect;

        private void Start()
        {
            Bgm.PlayFeedbacks();
        }
    }
}
