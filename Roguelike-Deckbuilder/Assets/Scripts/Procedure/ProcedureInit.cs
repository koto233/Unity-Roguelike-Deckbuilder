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
        private IResourceUpdater _updater;

        public ProcedureInit(ProcedureManager manager) : base(manager) { }

        public override void OnEnter()
        {
            InitAsync().Forget();
        }

        private async UniTask InitAsync()
        {
            try
            {
                // 1. 资源更新
                _updater = new YooAssetUpdater();
                await _updater.StartUpdate();

                // 2. 注册 AssetService
                var asset = new YooAssetAssetService(YooAssets.GetPackage("DefaultPackage"));
                ServiceLocator.Register<IAssetService>(asset);

                // 3. 并行加载所有配置
                await LoadConfigsParallelAsync();

                // 4. 进入标题
                _procedureManager.ChangeProcedure<ProcedureTitle>();
            }
            catch (Exception ex)
            {
                Debug.LogError($"[Init] 失败: {ex}");
                // 可以在这里打开错误弹窗，或重试
            }
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
    }
}