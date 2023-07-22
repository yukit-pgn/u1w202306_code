using VContainer;
using VContainer.Unity;

namespace Main.Mobile
{
    public class MobileLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<MobileModel>(Lifetime.Scoped);
            builder.RegisterEntryPoint<MobilePresenter>(Lifetime.Scoped);
            builder.RegisterComponentInHierarchy<MatchingView>();
            builder.RegisterComponentInHierarchy<MobileUIView>();
            builder.RegisterComponentInHierarchy<InputObserver>();
            builder.RegisterComponentInHierarchy<Bottle>();
        }
    }
}
