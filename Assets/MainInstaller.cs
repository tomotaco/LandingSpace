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

        Container.Bind<AudioClip>().WithId("SE_BreakableBlock_Destruction").FromResource("Sounds/BreakableBlock_Destruction");
        Container.Bind<AudioClip>().WithId("SE_BreakableBlock_Damage").FromResource("Sounds/BreakableBlock_Damage");
        Container.Bind<AudioClip>().WithId("SE_Thurst").FromResource("Sounds/Thurst");
        Container.Bind<AudioClip>().WithId("SE_Thurst_Head").FromResource("Sounds/Thurst_Head");
        Container.Bind<AudioClip>().WithId("SE_Thurst_Tail").FromResource("Sounds/Thurst_Tail");
        Container.Bind<AudioClip>().WithId("SE_Hit").FromResource("Sounds/HitShort");
        Container.Bind<AudioClip>().WithId("SE_Bounce").FromResource("Sounds/Bounce");
        Container.Bind<AudioClip>().WithId("SE_Connect").FromResource("Sounds/Connect");
        Container.Bind<AudioClip>().WithId("SE_Shot").FromResource("Sounds/Shot");
        Container.Bind<AudioClip>().WithId("SE_Shot2").FromResource("Sounds/Shot2");
        Container.Bind<AudioClip>().WithId("SE_Start").FromResource("Sounds/start3");
        Container.Bind<AudioClip>().WithId("SE_Goal").FromResource("Sounds/goal1");
        Container.Bind<AudioClip>().WithId("SE_Select").FromResource("Sounds/select1");

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