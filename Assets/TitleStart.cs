using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DummyEgg.ProjectGK
{
    public class TitleStart : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            _doTitleProcess().Forget();
        }

        private async UniTask _doTitleProcess()
        {
            var u = UiManagerRoot.Instance;
            u.PushTitle();
            await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate);
        }
    }
}
