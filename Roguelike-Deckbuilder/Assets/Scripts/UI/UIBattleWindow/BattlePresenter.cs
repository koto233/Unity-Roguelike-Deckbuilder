using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cysharp.Threading.Tasks;
using LitFramework;
using LitFramework.Config;
using LitFramework.EventBus;
using LitFramework.ObjectPool;
using UnityEngine;

public class BattlePresenter : BasePresenter<UIBattle>, IHasData<BattleContext>
{
    private Enemy _currentTargetEnemy;
    private Card _selectedCard;
    private BattleContext _context;
    private BattleController _controller;
    public BattlePresenter(UIBattle view) : base(view) { }
    public void SetData(BattleContext context)
    {
        _context = context;
    }
    public override void Init()
    {
        _controller = ServiceLocator.Get<BattleController>();
        SubscribeEvents();
        RefreshAllHp(_context);
        RefreshHand(_context.Player.Hand, _context.Player.Hand, ChangeType.Add);
        RefreshBlock(_context.Player.Block, EntityType.Player);
        RefreshEnergy(_context.Player.Energy, _context.Player.MaxEnergy);
        RefreshDrawPileCount(_context.Player.DrawPileCount);
        RefreshDiscardPileCount(_context.Player.DiscardPileCount);
        View.CreateEnemyViews(_context.Enemies);
    }
    private void SubscribeEvents()
    {
        EventBus<HandChangedEvent>.Subscribe(OnHandChanged);
        EventBus<HpChangedEvent>.Subscribe(OnHpChanged);
        EventBus<EnergyChangedEvent>.Subscribe(OnEnergyChanged);
        EventBus<BlockChangedEvent>.Subscribe(OnBlockChanged);
        EventBus<PlayerMaxHpChangedEvent>.Subscribe(OnPlayerMaxHpChanged);
        EventBus<BuffAppliedEvent>.Subscribe(OnBuffApplied);
        EventBus<BuffRemovedEvent>.Subscribe(OnBuffRemoved);
        EventBus<BuffStacksChangedEvent>.Subscribe(OnBuffStacksChanged);
        EventBus<TooltipShowEvent>.Subscribe(OnHoverEvent);
        EventBus<IntentEvent>.Subscribe(OnEnemyIntentChanged);
        EventBus<FloatingTextEvent>.Subscribe(OnFloatingTextEvent);
        EventBus<DrawPileChangedEvent>.Subscribe(OnDrawPileChanged);
        EventBus<DiscardPileChangedEvent>.Subscribe(OnDiscardPileChanged);
        View.OnOpenPile += OpenPile;
        View.OnEndTurn += EndTurn;
        View.OnCardDragStartRequested += OnDragStart;
        View.OnCardDragRequested += OnDragCard;
        View.OnCardDragEndRequested += OnDragEnd;
        View.OnCardPlayRequested += OnCardPlay;

    }



    private void UnSubscribeEvents()
    {
        EventBus<HandChangedEvent>.Unsubscribe(OnHandChanged);
        EventBus<HpChangedEvent>.Unsubscribe(OnHpChanged);
        EventBus<EnergyChangedEvent>.Unsubscribe(OnEnergyChanged);
        EventBus<BlockChangedEvent>.Unsubscribe(OnBlockChanged);
        EventBus<PlayerMaxHpChangedEvent>.Unsubscribe(OnPlayerMaxHpChanged);
        EventBus<BuffAppliedEvent>.Unsubscribe(OnBuffApplied);
        EventBus<BuffRemovedEvent>.Unsubscribe(OnBuffRemoved);
        EventBus<BuffStacksChangedEvent>.Unsubscribe(OnBuffStacksChanged);
        EventBus<TooltipShowEvent>.Unsubscribe(OnHoverEvent);
        EventBus<IntentEvent>.Unsubscribe(OnEnemyIntentChanged);
        EventBus<FloatingTextEvent>.Unsubscribe(OnFloatingTextEvent);
        EventBus<DrawPileChangedEvent>.Unsubscribe(OnDrawPileChanged);
        EventBus<DiscardPileChangedEvent>.Unsubscribe(OnDiscardPileChanged);
        View.OnOpenPile -= OpenPile;
        View.OnEndTurn -= EndTurn;
        View.OnCardDragStartRequested -= OnDragStart;
        View.OnCardDragRequested -= OnDragCard;
        View.OnCardDragEndRequested -= OnDragEnd;
        View.OnCardPlayRequested -= OnCardPlay;
    }


    private void EndTurn()
    {
        _controller.EndPlayerTurn();
    }


    private void OnPlayerMaxHpChanged(PlayerMaxHpChangedEvent evt)
    {

    }

    private void OnBlockChanged(BlockChangedEvent evt)
    {
        RefreshBlock(evt.NewBlock, evt.EntityType);
    }

    private void OnHandChanged(HandChangedEvent evt)
    {
        RefreshHand(evt.Cards, evt.ChangedCards, evt.Type);
    }
    private void OnHpChanged(HpChangedEvent evt)
    {
        RefreshHp(evt.NewHp, evt.MaxHp, evt.EntityType, evt.EntityId);
    }
    private void OnEnergyChanged(EnergyChangedEvent evt)
    {
        RefreshEnergy(evt.CurrentEnergy, evt.MaxEnergy);

    }
    private void OnDiscardPileChanged(DiscardPileChangedEvent @event)
    {
        RefreshDiscardPileCount(@event.CurrentCount);
    }

    private void OnDrawPileChanged(DrawPileChangedEvent @event)
    {
        RefreshDrawPileCount(@event.CurrentCount);
    }
    private void RefreshAllHp(BattleContext context)
    {
        RefreshHp(context.Player.CurrentHp, context.Player.MaxHp, EntityType.Player, -1);
        foreach (var enemy in context.Enemies)
        {
            RefreshHp(enemy.CurrentHp, enemy.MaxHp, EntityType.Enemy, enemy.Id);
        }

    }
    private void RefreshHp(int currentHp, int maxHp, EntityType entityType, int entityId)
    {
        View.RefreshHp(currentHp, maxHp, entityType, entityId);
    }

    private void RefreshBlock(int block, EntityType entityType)
    {
        View.RefreshBlock(block, entityType);
    }
    private void RefreshHand(IReadOnlyList<Card> handCards, IReadOnlyList<Card> changedCards, ChangeType type)
    {
        _controller.UpdateCardUsability();
        View.RefreshHand(handCards, changedCards, type);
    }

    private void RefreshDrawPileCount(int count)
    {
        View.RefreshDrawPileCount(count);
    }

    private void RefreshDiscardPileCount(int count)
    {
        View.RefreshDiscardPileCount(count);
    }

    private void OnDragStart(Card card, Vector2 position)
    {
        // Debug.Log("OnDragStart:" + cardId);
        _selectedCard = card;
        if (card.NeedTarget)
        {
            View.ShowArrow(position, position, Vector2.up * 150);
        }
    }
    private void OnDragCard(Enemy enemy, Vector2 position)
    {
        _currentTargetEnemy = enemy;
        View.UpdateArrow(position);
        // Debug.Log("OnDragCard:" + enemy == null);
    }


    private void OnCardPlay(Card card)
    {
        if (_selectedCard != card)
        {
            Debug.LogError("拖拽的卡牌不一致");
            return;
        }
        if (!_controller.PlayCard(_selectedCard, _currentTargetEnemy))
        {
            View.ResetCard();
        }
    }
    private void OnDragEnd(Card card)
    {
        View.HideArrow();
    }

    private void RefreshEnergy(int energy, int maxEnergy)
    {
        View.RefreshEnergy(energy, maxEnergy);
    }

    /// <summary>
    /// 打开牌组 0 : 抽牌组 1 : 弃牌组 
    /// </summary>
    /// <param name="index"></param>
    private void OpenPile(int index)
    {
        var pile = _controller.GetPile(index);
        View.ClearCardsInPileUI();
        View.SpawnCardInList(pile);
        View.OpenPilePanel();
    }
    private void OnBuffApplied(BuffAppliedEvent evt)
    {
        RefreshOwnerBuffs(evt.Owner);
    }

    private void OnBuffRemoved(BuffRemovedEvent evt)
    {
        RefreshOwnerBuffs(evt.Owner);
    }

    private void OnBuffStacksChanged(BuffStacksChangedEvent evt)
    {
        // 可以只更新单个 Slot，但简单起见全量刷新
        RefreshOwnerBuffs(evt.Owner);
    }

    private void RefreshOwnerBuffs(CharacterBase owner)
    {
        if (owner is Player)
        {
            // var playerView = View.GetPlayerView();
            // playerView.RefreshBuffs(owner.BuffManager.AllBuffs.ToList());
        }
        else if (owner is Enemy enemy)
        {
            var enemyView = View.GetEnemyView(enemy.Id);
            enemyView?.RefreshBuffs(owner.BuffManager.AllBuffs.ToList());
        }
    }
    private void OnHoverEvent(TooltipShowEvent evt)
    {
        if (evt.IsHovering)
        {
            switch (evt.Type)
            {
                case TooltipType.Buff:
                    {
                        View.ShowBuffTooltip(evt.Data, evt.Position);
                        break;
                    }
                case TooltipType.Intent:
                    {
                        View.ShowIntentToolTip(evt.Data, evt.Position);
                        break;
                    }
            }
        }
        else
        {
            View.HideAllTooltips();
        }

    }

    private void OnEnemyIntentChanged(IntentEvent evt)
    {
        View.RefreshEnemyIntent(evt.Enemy, evt.IntentConfig);
    }
    public override void Dispose()
    {
        UnSubscribeEvents();
    }

    private void OnFloatingTextEvent(FloatingTextEvent evt)
    {
        Vector3 position = View.GetEntityPosition(evt.EntityType, evt.EntityId);
        View.ShowFloatingText(evt.Text, position, evt.Color, evt.IsCritical).Forget();
    }


}
