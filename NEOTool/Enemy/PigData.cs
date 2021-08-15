using Newtonsoft.Json;
namespace NEOTool.Enemy
{
  public class PigData
  {
    [JsonProperty("mId")]
    public int Id { get; init; }
    [JsonProperty("mGroupId")]
    public int EnemyGroupId { get; init; }
    [JsonProperty("mExp")]
    public int EXP { get; init; }
    [JsonProperty("mBp")]
    public int PP { get; init; }
    [JsonProperty("mBattleTime")]
    public int BattleTime { get; init; }
    [JsonProperty("mDrop")]
    public int DropId { get; init; }
    [JsonProperty("mRankTimeTable")]
    private int RankTimeTable { get; init; }
    [JsonProperty("mColorReaderMinimum")]
    private int ColorReaderMinimum { get; init; }
    [JsonProperty("mColorReaderFluctuation")]
    private int ColorReaderFluctuation { get; init; }
    [JsonProperty("mColorExplosionDamageType")]
    private int ColorExplosionDamageType { get; init; }
    [JsonProperty("mDivisionTable")]
    private int DivisionTable { get; init; }
    [JsonProperty("mSametimeTable")]
    private int SavetimeTable { get; init; }
    [JsonProperty("mWeakReaderMinimum")]
    private int WeakReaderMinimum { get; init; }
    [JsonProperty("mWeakReaderFluctuation")]
    private int WeakReaderFluctuation { get; init; }
    [JsonProperty("mGroupType")]
    private string GroupType { get; init; }
  }
}
