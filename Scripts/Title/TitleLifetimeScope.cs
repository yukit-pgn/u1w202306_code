using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Main.Title
{
    public class TitleLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<TitleModel>(Lifetime.Scoped);
            builder.RegisterEntryPoint<TitlePresenter>(Lifetime.Scoped);
            builder.RegisterComponentInHierarchy<MatchingView>();
            builder.RegisterComponentInHierarchy<TitleUIView>();
        }
    }
}
