using System;
using Cysharp.Threading.Tasks;
using LitFramework.Asset;
using LitFramework.Config;
using UnityEngine;
using YooAsset;

namespace LitFramework.FSM.Procedure
{
    public class ProcedureInit : ProcedureBase
    {
        public override string Name => "Init";
        private IResourceUpdater _updater;

        public ProcedureInit(ProcedureManager manager) : base(manager) { }

        public override void OnEnter()
        {
            InitAsync().Forget();
        }

        private async UniTask InitAsync()
        {
            // 1. 资源更新

            _updater = new YooAssetUpdater(playMode: GameRoot.Instance.PlayMode);

            await _updater.StartUpdate();

            // 2. 注册 AssetService
            var asset = new YooAssetAssetService(YooAssets.GetPackage("DefaultPackage"));
            ServiceLocator.Register<IAssetService>(asset);
            var cardIconService = ServiceLocator.Get<CardIconService>();
            await cardIconService.PreLoadCardIcons();
            // 3. 并行加载所有配置
            await LoadConfigsParallelAsync();

            // 4. 进入标题
            _procedureManager.ChangeProcedure<ProcedureTitle>();

        }

        private static async UniTask LoadConfigsParallelAsync()
        {
            var svc = ServiceLocator.Get<IConfigService>();

            var tasks = new UniTask[]
            {
                svc.LoadDictTableAsync<CardConfig>(ConfigPaths.Card),
                svc.LoadDictTableAsync<CardEffectsConfig>(ConfigPaths.CardEffects),
                svc.LoadDictTableAsync<EnemyConfig>(ConfigPaths.Enemy),
                svc.LoadDictTableAsync<BuffConfig>(ConfigPaths.Buff),
                svc.LoadDictTableAsync<IntentConfig>(ConfigPaths.Intent),
                svc.LoadListTableAsync<MapConfig>(ConfigPaths.Map),
                svc.LoadDictTableAsync<EncounterConfig>(ConfigPaths.Encounter),
                svc.LoadDictTableAsync<PlayerInitConfig>(ConfigPaths.PlayerInit),
                svc.LoadDictTableAsync<RelicConfig>(ConfigPaths.Relic),
                svc.LoadDictTableAsync<ShopConfig>(ConfigPaths.Shop),
                svc.LoadDictTableAsync<RewardConfig>(ConfigPaths.Reward),
                 svc.LoadDictTableAsync<ActionConfig>(ConfigPaths.ActionConfig)

            };

            await UniTask.WhenAll(tasks);
            Debug.Log("[Init] 所有配置加载完成");
        }
    }

    // 路径集中管理
    public static class ConfigPaths
    {
        public const string Card = "Assets/Config/Json/CardConfig.json";
        public const string CardEffects = "Assets/Config/Json/CardEffects.json";
        public const string Enemy = "Assets/Config/Json/EnemyConfig.json";
        public const string Buff = "Assets/Config/Json/BuffConfig.json";
        public const string Intent = "Assets/Config/Json/IntentConfig.json";
        public const string Map = "Assets/Config/Json/MapConfig.json";
        public const string Encounter = "Assets/Config/Json/EncounterConfig.json";
        public const string PlayerInit = "Assets/Config/Json/PlayerInitConfig.json";
        public const string Relic = "Assets/Config/Json/RelicConfig.json";
        public const string Shop = "Assets/Config/Json/ShopConfig.json";
        public const string Reward = "Assets/Config/Json/RewardConfig.json";
        public const string ActionConfig = "Assets/Config/Json/ActionConfig.json";
    }
}