using System.Collections.Generic;
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
            EventBus<MaxHpChangedEvent>.Publish(new MaxHpChangedEvent() { OldValue = _maxHp, NewValue = value });
        }
    }
    private int _currentHp;
    public int CurrentHp
    {
        get => _currentHp;
        set => _currentHp = value;
    }
    public List<string> DeckCardIds { get; set; } = new List<string>();
    public List<string> RelicIds { get; set; } = new List<string>();

    public PlayerSaveData ExportState()
    {
        return new PlayerSaveData
        {
            Coin = Coin,
            MaxHp = MaxHp,
            CurrentHp = CurrentHp,
            DeckCardIds = new List<string>(DeckCardIds),
            RelicIds = new List<string>(RelicIds)
        };
    }

    public void ImportState(PlayerSaveData data)
    {
        _coin = data.Coin;
        _maxHp = data.MaxHp;
        _currentHp = data.CurrentHp;
        DeckCardIds = new List<string>(data.DeckCardIds);
        RelicIds = new List<string>(data.RelicIds);
    }
}