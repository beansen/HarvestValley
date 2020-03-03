using UnityEngine;
using Zenject;

public class ZenInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<Inventory>().AsSingle();
        Container.Bind<UiController>().FromComponentInHierarchy().AsSingle();
        Container.Bind<FarmingManager>().FromComponentInHierarchy().AsSingle();
    }
}