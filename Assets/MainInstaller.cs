using UnityEngine;
using Zenject;
using NCMBRest;

public class MainInstaller : MonoInstaller<MainInstaller>
{

    private const string applicationKey = "(Your NCMB application key)";
    private const string clientKey = "(Your NCMB client key)";

    public override void InstallBindings()
    {
        Container.Bind<GameMain>().AsSingle().NonLazy();
        Container.Bind<IInitializable>().To<GameMain>().AsSingle();
        Container.Bind<IFixedTickable>().To<GameMain>().AsSingle();

        Container.Bind<NCMBRestController>().AsSingle().WithArguments(applicationKey, clientKey).NonLazy();
        Container.Bind<LeaderboardManager>().FromNewComponentOnNewGameObject().WithGameObjectName("LeaderBoardGameObject").AsSingle();

        // Factories
        Container.BindFactory<PlayerController, PlayerController.Factory>().FromComponentInNewPrefabResource("Prefabs/Player");
        Container.BindFactory<ConnectableBlockController, ConnectableBlockController.Factory>().WithId("connectableCubeFactory").FromComponentInNewPrefabResource("Prefabs/ConnectableCube");
        Container.BindFactory<ConnectableBlockController, ConnectableBlockController.Factory>().WithId("connectableCylinderFactory").FromComponentInNewPrefabResource("Prefabs/ConnectableCylinder");
        Container.BindFactory<BreakableBlockController, BreakableBlockController.Factory>().WithId("breakableCubeFactory").FromComponentInNewPrefabResource("Prefabs/BreakableCube");
        Container.BindFactory<BreakableBlockController, BreakableBlockController.Factory>().WithId("breakableCylinderFactory").FromComponentInNewPrefabResource("Prefabs/BreakableCylinder");

        // MemoryPools
        Container.BindMemoryPool<ConnectableBlockController, ConnectableBlockController.Pool>().WithInitialSize(20).WithId("connectableCubePool").FromComponentInNewPrefabResource("Prefabs/ConnectableCube").UnderTransformGroup("ConnectableCubes");
        Container.BindMemoryPool<ConnectableBlockController, ConnectableBlockController.Pool>().WithInitialSize(20).WithId("connectableCylinderPool").FromComponentInNewPrefabResource("Prefabs/ConnectableCylinder").UnderTransformGroup("ConnectableCylinders");
        Container.BindMemoryPool<BreakableBlockController, BreakableBlockController.Pool>().WithInitialSize(20).WithId("breakableCubePool").FromComponentInNewPrefabResource("Prefabs/BreakableCube").UnderTransformGroup("BreakableCubes");
        Container.BindMemoryPool<BreakableBlockController, BreakableBlockController.Pool>().WithInitialSize(20).WithId("breakableCylinderPool").FromComponentInNewPrefabResource("Prefabs/BreakableCylinder").UnderTransformGroup("BreakableCylinders");

        Container.BindMemoryPool<UIRankRecordController, UIRankRecordController.Pool>().WithInitialSize(20).WithId("rankRecordPool").FromComponentInNewPrefabResource("Prefabs/RankRecord").UnderTransformGroup("RankingRecords");
    }
}