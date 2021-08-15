using System.Collections.Generic;
using NEOTool.Text;
using Newtonsoft.Json;
namespace NEOTool.Enemy
{
  public class EnemyReport
  {
    [JsonProperty("mId")]
    public int Id { get; set; }
    [JsonProperty("mSortIndex")]
    private int SortIndex { get; set; }
    [JsonProperty("mCharacter")]
    public int BattleCharacterId { get; set; }
    [JsonProperty("mGroupCharacter")]
    private List<int> GroupCharacter { get; set; }
    [JsonProperty("mEnemydata")]
    public int EnemyDataId { get; set; }
    [JsonProperty("mIcon")]
    private string Icon { get; set; }
    [JsonProperty("mSymbolType")]
    private int SymbolId { get; set; }
    [JsonProperty("mName")]
    private string NameToken { get; set; }
    public string Name { get; set; }
    [JsonProperty("mInfo")]
    private string InfoToken { get; set; }
    [JsonProperty("mIsBoss")]
    private bool Boss { get; set; }
    [JsonProperty("mWeak")]
    public List<int> Weaknesses { get; set; }
    [JsonProperty("mNoiseImagePath")]
    public string ImagePath { get; set; }

    public void PostInit(GameText gameText)
    {
      Name = gameText[NameToken].English;
    }
  }
}
