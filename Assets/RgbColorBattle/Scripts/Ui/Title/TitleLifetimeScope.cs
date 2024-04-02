using Cysharp.Threading.Tasks;
using System;
using MessagePack;
using MessagePack.Resolvers;
using VContainer;
using VContainer.Unity;
using DummyEgg.MasterDataWorker;
using UnityEngine;

namespace DummyEgg.ProjectGK.Title
{
    public class TitleLifetimeScope : LifetimeScope
    {
        [SerializeField] TitleView _view;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(_view);
            builder.Register<TitlePresenter>(Lifetime.Singleton);
            builder.RegisterBuildCallback(OnBuild);
        }

        void OnBuild(IObjectResolver container)
        {
            InitializeAsync(container).Forget();
        }

        async UniTask InitializeAsync(IObjectResolver container)
        {
            var header = container.Resolve<TitlePresenter>();
            await header.Prepare();
        }
    }
}
