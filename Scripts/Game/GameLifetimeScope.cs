using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Main.Game
{
    public class GameLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<GameModel>(Lifetime.Scoped);
            builder.RegisterEntryPoint<GamePresenter>(Lifetime.Scoped);
            builder.RegisterComponentInHierarchy<Player>();
            builder.RegisterComponentInHierarchy<FieldManager>();
            builder.RegisterComponentInHierarchy<GameOverDialog>();
            builder.RegisterComponentInHierarchy<GameUIView>();
        }
    }
}
