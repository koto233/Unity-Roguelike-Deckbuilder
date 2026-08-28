using System;
using System.Collections.Generic;
using LitFramework;
using LitFramework.Config;
using LitFramework.EventBus;

public class PlayerDataService
{
    private int _coin;
    public int Coin
    {
        get => _coin;
        set
        {
            if (_coin == value) return;
            int old = _coin;
            _coin = value;
            EventBus<CoinChangedEvent>.Publish(new CoinChangedEvent() { OldValue = old, NewValue = value });
        }
    }
    private int _maxHp;
    public int MaxHp
    {
        get => _maxHp;
        set
        {
            if (_maxHp == value) return;
            _maxHp = value;
            EventBus<HpChangedEvent>.Publish(new HpChangedEvent() { OldHp = _maxHp, NewHp = value });
        }
    }
    private int _currentHp;
    public int CurrentHp
    {
        get => _currentHp;
        set
        {
            if (_currentHp == value) return;
            _currentHp = Math.Min(value, MaxHp);
            EventBus<HpChangedEvent>.Publish(new HpChangedEvent() { OldHp = _currentHp, NewHp = value });
        }
    }
    private List<int> _deckCardIds = new();
    private List<int> _relicIds = new();
    public IReadOnlyList<int> DeckCardIds => _deckCardIds;
    public IReadOnlyList<int> RelicIds => _relicIds;

    public void Init()
    {
        var config = ServiceLocator.Get<IConfigService>().GetTable<PlayerInitConfig>().Get(0);
        _coin = 0;
        _maxHp = config.InitialHp;
        _currentHp = config.InitialHp;
        _deckCardIds.AddRange(config.InitialDeck);
        _relicIds.Clear();
    }
    public PlayerSaveData ExportState()
    {
        return new PlayerSaveData
        {
            Coin = Coin,
            MaxHp = MaxHp,
            CurrentHp = CurrentHp,
            DeckCardIds = new List<int>(DeckCardIds),
            RelicIds = new List<int>(RelicIds)
        };
    }

    public void ImportState(PlayerSaveData data)
    {
        _coin = data.Coin;
        _maxHp = data.MaxHp;
        _currentHp = data.CurrentHp;
        _deckCardIds = new List<int>(data.DeckCardIds);
        _relicIds = new List<int>(data.RelicIds);
    }
    public void SyncHp(int currentHp, int maxHp)
    {
        CurrentHp = currentHp;
        MaxHp = maxHp;
    }
    public void UpgradeCard(int cardId, int targetId)
    {
        for (int i = 0; i < _deckCardIds.Count; i++)
        {
            if (_deckCardIds[i] == cardId)
            {
                _deckCardIds[i] = targetId;
                break;
            }
        }
    }
    public void AddCard(int cardId)
    {
        if (!_deckCardIds.Contains(cardId))
            _deckCardIds.Add(cardId);
    }
    public void RemoveCard(int cardId)
    {
        if (_deckCardIds.Contains(cardId))
            _deckCardIds.Remove(cardId);
    }
    public void AddRelic(int relicId)
    {
        if (!_relicIds.Contains(relicId))
            _relicIds.Add(relicId);
    }
    public void RemoveRelic(int relicId)
    {
        if (_relicIds.Contains(relicId))
            _relicIds.Remove(relicId);
    }
    public void AddCoin(int coin)
    {
        Coin += coin;
    }
    public void SpendCoin(int coin)
    {
        if (Coin >= coin)
            Coin -= coin;
    }
}