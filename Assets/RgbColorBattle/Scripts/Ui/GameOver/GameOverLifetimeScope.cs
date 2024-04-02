using Cysharp.Threading.Tasks;
using System;
using MessagePack;
using MessagePack.Resolvers;
using VContainer;
using VContainer.Unity;
using DummyEgg.MasterDataWorker;
using UnityEngine;

namespace DummyEgg.ProjectGK.GameOver
{
    public class GameOverLifetimeScope : LifetimeScope
    {
        [SerializeField] GameOverView _view;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(_view);
            builder.Register<GameOverPresenter>(Lifetime.Singleton);
            builder.RegisterBuildCallback(OnBuild);
        }

        void OnBuild(IObjectResolver container)
        {
            InitializeAsync(container).Forget();
        }

        async UniTask InitializeAsync(IObjectResolver container)
        {
            var header = container.Resolve<GameOverPresenter>();
            await header.Prepare();
        }
    }
}
