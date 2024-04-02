using Cysharp.Threading.Tasks;
using System;
using MessagePack;
using MessagePack.Resolvers;
using VContainer;
using VContainer.Unity;
//using DummyEgg.MasterDataWorker;
using UnityEngine;

namespace DummyEgg.ProjectGK.DESystem
{
    public class BootLifetimeScope : LifetimeScope
    {
        private void Start()
        {
            //DontDestroyOnLoad(this.gameObject);
        }
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterBuildCallback(OnBuild);
        }

        private void OnBuild(IObjectResolver container)
        {
            // シーンを切り替えても破棄されないようにする
            DontDestroyOnLoad(gameObject);

            _setupMessagePack();
            _loadToTitle().Forget();
        }

        private void _setupMessagePack()
        {
            // MessagePack の初期化
            // InvalidOperationException: Register must call on startup(before use GetFormatter<T>).
            try
            {
                StaticCompositeResolver.Instance.Register(new[]{
                    DummyEgg.MasterDataWorker.MasterMemoryResolver.Instance, // set MasterMemory generated resolver
                    DummyEgg.MasterDataWorker.Resolvers.MmGeneratedResolver.Instance,    // set MessagePack generated resolver(MasterMemory用)
                    StandardResolver.Instance      // set default MessagePack resolver
                });
                var options = MessagePackSerializerOptions.Standard.WithResolver(StaticCompositeResolver.Instance);
                MessagePackSerializer.DefaultOptions = options;
            }
            catch (Exception ex)
            {
                Debug.LogWarning(ex.Message);
            }
        }

        private async UniTask _loadToTitle()
        {
            //順番確保
            await MasterData.MasterdataManager.Instance.Load();
            //TransitionManager.Instance.LoadSceneAsync("Title.unity").Forget();
            TransitionManager.Instance.LoadSceneAsync("RgbColorBattle/Scenes/Title").Forget();
        }
    }
}
