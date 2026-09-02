#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using LitFramework;
using LitFramework.UI.Core.Service;
using LitFramework.FSM.Procedure;
using LitFramework.EventBus;
using Cysharp.Threading.Tasks;

/// <summary>
/// GM 工具窗口 - 仅开发环境使用
/// </summary>
public class GMToolWindow : EditorWindow
{
    private Vector2 _scrollPos;
    private string _uiName = "ShopView";
    private string _addCardId = "101";
    private string _addRelicId = "201";
    private string _nodeId = "0_0";
    private int _goldAmount = 100;
    private int _hpAmount = 10;
    private static readonly Dictionary<string, Func<UniTask>> UIOpenMap = new()
    {
        ["ShopView"] = () => ServiceLocator.Get<UIService>().OpenAsync<ShopView>(),
        ["DeckView"] = () => ServiceLocator.Get<UIService>().OpenAsync<DeckView>(),
        ["MapView"] = () => ServiceLocator.Get<UIService>().OpenAsync<MapView>(),
        ["RestView"] = () => ServiceLocator.Get<UIService>().OpenAsync<RestView>(),

    };
    [MenuItem("Tools/GM Tool")]
    public static void ShowWindow()
    {
        GetWindow<GMToolWindow>("GM Tool");
    }

    private void OnGUI()
    {
        if (!Application.isPlaying)
        {
            EditorGUILayout.HelpBox("请进入 Play Mode 以使用 GM 功能", MessageType.Warning);
            return;
        }

        _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);

        DrawUISection();
        DrawPlayerSection();
        DrawDeckSection();
        DrawRelicSection();
        DrawMapSection();
        DrawBattleSection();
        DrawSaveSection();
        DrawEventSection();

        EditorGUILayout.EndScrollView();
    }

    #region UI 操作
    private void DrawUISection()
    {
        GUILayout.Label("UI 控制", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();
        _uiName = EditorGUILayout.TextField("UI 名称", _uiName);
        if (GUILayout.Button("打开 UI", GUILayout.Width(80)))
        {
            OpenUI(_uiName);
        }
        if (GUILayout.Button("关闭所有 UI", GUILayout.Width(100)))
        {
            CloseAllUI();
        }
        EditorGUILayout.EndHorizontal();
    }

    private async void OpenUI(string uiName)
    {
        if (UIOpenMap.TryGetValue(uiName, out var openFunc))
        {
            await openFunc();
            Debug.Log($"打开 UI: {uiName}");
        }
        else
        {
            Debug.LogError($"未注册的 UI: {uiName}");
        }
    }

    private void CloseAllUI()
    {
        try
        {
            var uiService = ServiceLocator.Get<UIService>();
            var method = typeof(UIService).GetMethod("CloseAll");
            method?.Invoke(uiService, null);
            Debug.Log("已关闭所有 UI");
        }
        catch (Exception e)
        {
            Debug.LogError($"关闭 UI 失败: {e.Message}");
        }
    }
    #endregion

    #region 玩家数据修改
    private void DrawPlayerSection()
    {
        GUILayout.Label("玩家数据", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();
        _goldAmount = EditorGUILayout.IntField("金币", _goldAmount);
        if (GUILayout.Button("增加金币", GUILayout.Width(80)))
        {
            ModifyGold(_goldAmount);
        }
        if (GUILayout.Button("设置金币为 0", GUILayout.Width(100)))
        {
            SetGold(0);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        _hpAmount = EditorGUILayout.IntField("HP", _hpAmount);
        if (GUILayout.Button("恢复 HP", GUILayout.Width(80)))
        {
            HealPlayer(_hpAmount);
        }
        if (GUILayout.Button("满血", GUILayout.Width(60)))
        {
            FullHeal();
        }
        if (GUILayout.Button("受到伤害", GUILayout.Width(80)))
        {
            DamagePlayer(_hpAmount);
        }
        EditorGUILayout.EndHorizontal();
    }

    private void ModifyGold(int amount)
    {
        try
        {
            var playerService = ServiceLocator.Get<PlayerDataService>();
            var method = typeof(PlayerDataService).GetMethod("AddCoin");
            method?.Invoke(playerService, new object[] { amount });
            Debug.Log($"金币 +{amount}");
        }
        catch (Exception e)
        {
            Debug.LogError($"修改金币失败: {e.Message}");
        }
    }

    private void SetGold(int amount)
    {
        try
        {
            var playerService = ServiceLocator.Get<PlayerDataService>();
            var field = typeof(PlayerDataService).GetField("_coin", BindingFlags.NonPublic | BindingFlags.Instance);
            field?.SetValue(playerService, amount);
            Debug.Log($"金币设置为 {amount}");
        }
        catch (Exception e)
        {
            Debug.LogError($"设置金币失败: {e.Message}");
        }
    }

    private void HealPlayer(int amount)
    {
        try
        {
            var executor = ServiceLocator.Get<EffectExecutor>();
            var playerService = ServiceLocator.Get<PlayerDataService>();
            var character = playerService.GetType().GetProperty("Character")?.GetValue(playerService) as CharacterBase;
            if (character != null)
            {
                executor.Heal(amount, character);
                Debug.Log($"治疗 {amount} HP");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"治疗失败: {e.Message}");
        }
    }

    private void FullHeal()
    {
        try
        {
            var playerService = ServiceLocator.Get<PlayerDataService>();
            var character = playerService.GetType().GetProperty("Character")?.GetValue(playerService) as CharacterBase;
            if (character != null)
            {
                character.CurrentHp = character.MaxHp;
                Debug.Log("满血恢复");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"满血失败: {e.Message}");
        }
    }

    private void DamagePlayer(int amount)
    {
        // try
        // {
        //     var executor = ServiceLocator.Get<EffectExecutor>();
        //     var playerService = ServiceLocator.Get<PlayerDataService>();
        //     var character = playerService.GetType().GetProperty("Character")?.GetValue(playerService) as CharacterBase;
        //     if (character != null)
        //     {
        //         executor.Damage(amount, character);
        //         Debug.Log($"受到 {amount} 伤害");
        //     }
        // }
        // catch (Exception e)
        // {
        //     Debug.LogError($"伤害失败: {e.Message}");
        // }
    }
    #endregion

    #region 牌库修改
    private void DrawDeckSection()
    {
        GUILayout.Label("牌库操作", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();
        _addCardId = EditorGUILayout.TextField("卡牌 ID", _addCardId);
        if (GUILayout.Button("添加卡牌", GUILayout.Width(80)))
        {
            AddCard(int.Parse(_addCardId));
        }
        if (GUILayout.Button("移除最后一张", GUILayout.Width(100)))
        {
            RemoveLastCard();
        }
        EditorGUILayout.EndHorizontal();
    }

    private void AddCard(int cardId)
    {
        try
        {
            var playerService = ServiceLocator.Get<PlayerDataService>();
            var method = typeof(PlayerDataService).GetMethod("AddCard");
            method?.Invoke(playerService, new object[] { cardId });
            Debug.Log($"添加卡牌 ID: {cardId}");
        }
        catch (Exception e)
        {
            Debug.LogError($"添加卡牌失败: {e.Message}");
        }
    }

    private void RemoveLastCard()
    {
        try
        {
            var playerService = ServiceLocator.Get<PlayerDataService>();
            var deck = playerService.GetType().GetMethod("GetDeck")?.Invoke(playerService, null) as IReadOnlyList<Card>;
            if (deck != null && deck.Count > 0)
            {
                var last = deck.Last();
                var method = typeof(PlayerDataService).GetMethod("RemoveCard");
                method?.Invoke(playerService, new object[] { last.Config.Id });
                Debug.Log($"移除卡牌 ID: {last.Config.Id}");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"移除卡牌失败: {e.Message}");
        }
    }
    #endregion

    #region 遗物操作
    private void DrawRelicSection()
    {
        GUILayout.Label("遗物操作", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();
        _addRelicId = EditorGUILayout.TextField("遗物 ID", _addRelicId);
        if (GUILayout.Button("添加遗物", GUILayout.Width(80)))
        {
            AddRelic(int.Parse(_addRelicId));
        }
        if (GUILayout.Button("移除所有遗物", GUILayout.Width(100)))
        {
            ClearRelics();
        }
        EditorGUILayout.EndHorizontal();
    }

    private void AddRelic(int relicId)
    {
        try
        {
            var relicService = ServiceLocator.Get<RelicService>();
            var method = typeof(RelicService).GetMethod("AddRelic");
            method?.Invoke(relicService, new object[] { relicId });
            Debug.Log($"添加遗物 ID: {relicId}");
        }
        catch (Exception e)
        {
            Debug.LogError($"添加遗物失败: {e.Message}");
        }
    }

    private void ClearRelics()
    {
        try
        {
            var relicService = ServiceLocator.Get<RelicService>();
            var relics = relicService.Relics;
            foreach (var relic in relics.ToList())
            {
                var method = typeof(RelicService).GetMethod("RemoveRelic");
                method?.Invoke(relicService, new object[] { relic.Value.Config.Id });
            }
            Debug.Log("已清除所有遗物");
        }
        catch (Exception e)
        {
            Debug.LogError($"清除遗物失败: {e.Message}");
        }
    }
    #endregion

    #region 地图控制
    private void DrawMapSection()
    {
        GUILayout.Label("地图操作", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();
        _nodeId = EditorGUILayout.TextField("节点 ID", _nodeId);
        if (GUILayout.Button("跳转节点", GUILayout.Width(80)))
        {
            JumpToNode(_nodeId);
        }
        if (GUILayout.Button("重置地图", GUILayout.Width(80)))
        {
            ResetMap();
        }
        EditorGUILayout.EndHorizontal();
    }

    private void JumpToNode(string nodeId)
    {
        try
        {
            var mapService = ServiceLocator.Get<MapService>();
            var method = typeof(MapService).GetMethod("VisitNode");
            method?.Invoke(mapService, new object[] { nodeId });
            Debug.Log($"跳转到节点: {nodeId}");
        }
        catch (Exception e)
        {
            Debug.LogError($"跳转节点失败: {e.Message}");
        }
    }

    private void ResetMap()
    {
        try
        {
            var mapService = ServiceLocator.Get<MapService>();
            var method = typeof(MapService).GetMethod("ResetMap");
            method?.Invoke(mapService, null);
            // 重新生成地图
            var genMethod = typeof(MapService).GetMethod("GenerateMap");
            genMethod?.Invoke(mapService, new object[] { 1 });
            Debug.Log("地图已重置");
        }
        catch (Exception e)
        {
            Debug.LogError($"重置地图失败: {e.Message}");
        }
    }
    #endregion

    #region 战斗控制
    private void DrawBattleSection()
    {
        GUILayout.Label("战斗控制", EditorStyles.boldLabel);
        if (GUILayout.Button("立即结束战斗（胜利）"))
        {
            EndBattle(true);
        }
        if (GUILayout.Button("立即结束战斗（失败）"))
        {
            EndBattle(false);
        }
        if (GUILayout.Button("跳过回合"))
        {
            SkipTurn();
        }
    }

    private void EndBattle(bool victory)
    {
        try
        {
            var procedureManager = ServiceLocator.Get<ProcedureManager>();
            // 查找当前战斗流程
            var current = procedureManager.GetType().GetField("_currentProcedure", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(procedureManager);
            if (current != null && current.GetType().Name == "ProcedureBattle")
            {
                var battleController = current.GetType().GetField("_battleController", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(current);
                if (battleController != null)
                {
                    var method = battleController.GetType().GetMethod("ForceEndBattle");
                    method?.Invoke(battleController, new object[] { victory });
                    Debug.Log($"强制结束战斗，结果: {(victory ? "胜利" : "失败")}");
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"结束战斗失败: {e.Message}");
        }
    }

    private void SkipTurn()
    {
        try
        {
            var procedureManager = ServiceLocator.Get<ProcedureManager>();
            var current = procedureManager.GetType().GetField("_currentProcedure", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(procedureManager);
            if (current != null && current.GetType().Name == "ProcedureBattle")
            {
                var battleController = current.GetType().GetField("_battleController", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(current);
                if (battleController != null)
                {
                    var method = battleController.GetType().GetMethod("SkipTurn");
                    method?.Invoke(battleController, null);
                    Debug.Log("跳过回合");
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"跳过回合失败: {e.Message}");
        }
    }
    #endregion

    #region 存档操作
    private void DrawSaveSection()
    {
        GUILayout.Label("存档管理", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("保存游戏"))
        {
            SaveGame();
        }
        if (GUILayout.Button("加载游戏"))
        {
            LoadGame();
        }
        if (GUILayout.Button("删除存档"))
        {
            DeleteSave();
        }
        EditorGUILayout.EndHorizontal();
    }

    private void SaveGame()
    {
        try
        {
            var saveService = ServiceLocator.Get<SaveService>();
            var method = typeof(SaveService).GetMethod("SaveGame");
            method?.Invoke(saveService, new object[] { "Map" });
            Debug.Log("游戏已保存");
        }
        catch (Exception e)
        {
            Debug.LogError($"保存失败: {e.Message}");
        }
    }

    private void LoadGame()
    {
        try
        {
            var saveService = ServiceLocator.Get<SaveService>();
            var method = typeof(SaveService).GetMethod("LoadGame");
            var result = method?.Invoke(saveService, null);
            // Debug.Log($"读档结果: {result ? "成功" : "失败"}");
        }
        catch (Exception e)
        {
            Debug.LogError($"读档失败: {e.Message}");
        }
    }

    private void DeleteSave()
    {
        try
        {
            var saveService = ServiceLocator.Get<SaveService>();
            var method = typeof(SaveService).GetMethod("DeleteSave");
            method?.Invoke(saveService, null);
            Debug.Log("存档已删除");
        }
        catch (Exception e)
        {
            Debug.LogError($"删除存档失败: {e.Message}");
        }
    }
    #endregion

    #region 事件触发
    private void DrawEventSection()
    {
        GUILayout.Label("事件触发", EditorStyles.boldLabel);
        if (GUILayout.Button("触发战斗开始事件"))
        {
            EventBus<BattleStartEvent>.Publish(new BattleStartEvent { EnemyIds = new List<int> { 1, 2 } });
        }
        if (GUILayout.Button("触发回合开始事件"))
        {
            // EventBus<TurnStartEvent>.Publish(new TurnStartEvent());
        }
        if (GUILayout.Button("触发商店打开事件"))
        {
            // 假设有 OpenShopEvent
            // EventBus<OpenShopEvent>.Publish(new OpenShopEvent());
        }
    }
    #endregion
}
#endif