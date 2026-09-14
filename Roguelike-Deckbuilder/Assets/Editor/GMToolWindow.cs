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
using LitFramework.Config;

/// <summary>
/// GM 工具窗口 - 仅开发环境使用
/// </summary>
public class GMToolWindow : EditorWindow
{
    private Vector2 _scrollPos;
    private string _uiName = "ShopView";
    private string _addCardId = "1";
    private string _addRelicId = "1";
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
        // DrawSaveSection();
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
            AddCoin(_goldAmount);
        }
        if (GUILayout.Button("金币清零", GUILayout.Width(80)))
        {
            ClearCoin();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        _hpAmount = EditorGUILayout.IntField("HP", _hpAmount);
        if (GUILayout.Button("增加 HP", GUILayout.Width(80)))
        {
            HealPlayer(_hpAmount);
        }
        if (GUILayout.Button("满血", GUILayout.Width(60)))
        {
            FullHeal();
        }

        EditorGUILayout.EndHorizontal();
    }

    private void AddCoin(int amount)
    {
        try
        {
            var playerService = ServiceLocator.Get<PlayerDataService>();
            playerService.Coin += amount;

            Debug.Log($"金币 +{amount}");
        }
        catch (Exception e)
        {
            Debug.LogError($"修改金币失败: {e.Message}");
        }
    }
    private void ClearCoin()
    {
        try
        {
            var playerService = ServiceLocator.Get<PlayerDataService>();
            playerService.Coin = 0;
            Debug.Log($"金币 清零");
        }
        catch (Exception e)
        {
            Debug.LogError($"修改金币失败: {e.Message}");
        }
    }


    private void HealPlayer(int amount)
    {
        try
        {
            var playerService = ServiceLocator.Get<PlayerDataService>();
            playerService.CurrentHp += amount;
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
            if (playerService != null)
            {
                playerService.CurrentHp = playerService.MaxHp;
                Debug.Log("满血恢复");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"满血失败: {e.Message}");
        }
    }


    #endregion

    #region 牌库修改
    private void DrawDeckSection()
    {
        GUILayout.Label("全局牌库操作", EditorStyles.boldLabel);
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
        GUILayout.Label("局内牌库操作", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();
        _addCardId = EditorGUILayout.TextField("卡牌 ID", _addCardId);
        if (GUILayout.Button("添加卡牌至手牌", GUILayout.Width(150)))
        {
            AddCardToHand(int.Parse(_addCardId));
        }
        EditorGUILayout.EndHorizontal();
    }

    private void AddCard(int cardId)
    {
        try
        {
            var playerService = ServiceLocator.Get<PlayerDataService>();
            playerService.AddCard(cardId);

            Debug.Log($"添加卡牌 ID: {cardId}");
        }
        catch (Exception e)
        {
            Debug.LogError($"添加卡牌失败: {e.Message}");
        }
    }
    private void AddCardToHand(int cardId)
    {
        try
        {
            var battleController = ServiceLocator.Get<BattleController>();
            var cardConfig = ServiceLocator.Get<IConfigService>().GetTable<CardConfig>().Get(cardId);
            var card = new Card(cardConfig);
            battleController.Context.Player.AddCardToHand(card);
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

            var deck = playerService.DeckCardIds;
            if (deck != null && deck.Count > 0)
            {
                var last = deck.Last();
                playerService.RemoveCard(last);
                Debug.Log($"移除卡牌 ID: {last}");
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
            relicService.AddRelic(relicId);
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
            relicService.RemoveAllRelics();
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
            mapService.VisitNode(nodeId);
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
            mapService.NewMap(1);
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

    }

    private void EndBattle(bool victory)
    {
        try
        {
            var battleController = ServiceLocator.Get<BattleController>();
            if (victory)
            {
                battleController.BattleFSM.ChangeState<BattleEndState>();
            }
            else
            {
                ServiceLocator.Get<UIService>().OpenAsync<GameOverView>().Forget();
            }


        }
        catch (Exception e)
        {
            Debug.LogError($"结束战斗失败: {e.Message}");
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
        // GUILayout.Label("事件触发", EditorStyles.boldLabel);
        // if (GUILayout.Button("触发战斗开始事件"))
        // {
        //     EventBus<BattleStartEvent>.Publish(new BattleStartEvent { });
        // }
        // if (GUILayout.Button("触发回合开始事件"))
        // {
        //     EventBus<TurnStartEvent>.Publish(new TurnStartEvent());
        // }
        // if (GUILayout.Button("触发商店打开事件"))
        // {
        //     // 假设有 OpenShopEvent
        //     EventBus<OpenShopEvent>.Publish(new OpenShopEvent());
        // }
    }
    #endregion
}
#endif