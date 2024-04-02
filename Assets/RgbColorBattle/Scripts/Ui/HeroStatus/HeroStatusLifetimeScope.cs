using Cysharp.Threading.Tasks;
using DummyEgg.ProjectGK.Title;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace DummyEgg.ProjectGK.HeroUi
{
    public class HeroStatusLifetimeScope : LifetimeScope
    {
        [SerializeField] HeroStatusView _view;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(_view);
            builder.Register<HeroStatusPresenter>(Lifetime.Singleton);
            builder.RegisterBuildCallback(OnBuild);
        }

        void OnBuild(IObjectResolver container)
        {
            InitializeAsync(container).Forget();
        }

        async UniTask InitializeAsync(IObjectResolver container)
        {
            var header = container.Resolve<HeroStatusPresenter>();
            await header.Prepare();
        }
    }
}
