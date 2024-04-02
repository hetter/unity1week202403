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
            // �V�[����؂�ւ��Ă��j������Ȃ��悤�ɂ���
            DontDestroyOnLoad(gameObject);

            _setupMessagePack();
            _loadToTitle().Forget();
        }

        private void _setupMessagePack()
        {
            // MessagePack �̏�����
            // InvalidOperationException: Register must call on startup(before use GetFormatter<T>).
            try
            {
                StaticCompositeResolver.Instance.Register(new[]{
                    DummyEgg.MasterDataWorker.MasterMemoryResolver.Instance, // set MasterMemory generated resolver
                    DummyEgg.MasterDataWorker.Resolvers.MmGeneratedResolver.Instance,    // set MessagePack generated resolver(MasterMemory�p)
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
            //���Ԋm��
            await MasterData.MasterdataManager.Instance.Load();
            //TransitionManager.Instance.LoadSceneAsync("Title.unity").Forget();
            TransitionManager.Instance.LoadSceneAsync("RgbColorBattle/Scenes/Title").Forget();
        }
    }
}
