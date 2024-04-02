using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace DummyEgg.ProjectGK.Home
{
    public class HomeLifetimeScope : LifetimeScope
    {
        [SerializeField] HomeView _view;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(_view);
            builder.Register<HomePresenter>(Lifetime.Singleton);
            builder.RegisterBuildCallback(OnBuild);
        }

        void OnBuild(IObjectResolver container)
        {
            InitializeAsync(container).Forget();
        }

        async UniTask InitializeAsync(IObjectResolver container)
        {
            var header = container.Resolve<HomePresenter>();
            await header.Prepare();
        }
    }
}
