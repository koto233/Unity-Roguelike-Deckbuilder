using System.Collections.Generic;

public class PlayerDataService
{
    public int Gold { get; set; }
    public int MaxHp { get; set; }
    public int CurrentHp { get; set; }
    public List<string> DeckCardIds { get; set; } = new List<string>();
    public List<string> RelicIds { get; set; } = new List<string>();

    public PlayerSaveData ExportState()
    {
        return new PlayerSaveData
        {
            Gold = Gold,
            MaxHp = MaxHp,
            CurrentHp = CurrentHp,
            DeckCardIds = new List<string>(DeckCardIds),
            RelicIds = new List<string>(RelicIds)
        };
    }

    public void ImportState(PlayerSaveData data)
    {
        Gold = data.Gold;
        MaxHp = data.MaxHp;
        CurrentHp = data.CurrentHp;
        DeckCardIds = new List<string>(data.DeckCardIds);
        RelicIds = new List<string>(data.RelicIds);
    }
}