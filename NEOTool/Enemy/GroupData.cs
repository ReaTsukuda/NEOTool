using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
namespace NEOTool.Enemy
{
  public class GroupData
  {
    [JsonProperty("mId")]
    public int GroupId { get; init; }
    [JsonProperty("mWeek")]
    private int Week { get; init; }
    [JsonProperty("mDayLabel")]
    public int Day { get; init; }
    [JsonProperty("mSeriesOfBattleLabel")]
    private int SeriesOfBattlesLabel { get; init; }
    [JsonProperty("mOverwriteDevastationRate")]
    private decimal OverwriteDevastationRate { get; init; }
    [JsonProperty("mMapId")]
    public int MapId { get; init; }
    [JsonProperty("mGroupBgm")]
    private int GroupBgmId { get; init; }
    [JsonProperty("mEnvironmentalSe")]
    private int EnvironmentalSe { get; init; }
    [JsonProperty("mPositionTag")]
    private string PositionTag { get; init; }
    [JsonProperty("mOverrideMapEnvParam")]
    private string OverrideMapEnvParam { get; init; }
    [JsonProperty("mThunderstormParam")]
    private string ThunderstormParam { get; init; }
    [JsonProperty("mProcessType")]
    private int ProcessType { get; init; }
    [JsonProperty("mNotEscape")]
    public bool CannotEscape { get; init; }
    [JsonProperty("mBattleStartCameraLabel")]
    private int BattleStartCameraLabel { get; init; }
    [JsonProperty("mBattleStartTelopStartWaitTime")]
    private decimal BattleStartTelopWaitTime { get; init; }
    [JsonProperty("mCameraOverrideSo")]
    private string CameraOverrideSo { get; init; }
    [JsonProperty("mSymbolType")]
    private int SymbolType { get; init; }
    public string Symbol => Symbols[IconType];
    [JsonProperty("mIconType")]
    private int IconType { get; init; }
    [JsonProperty("mParallelAttackLimit")]
    public int ParallelAttackLimit { get; init; }
    [JsonProperty("mInitialSpawnNum")]
    private int InitialSpawnNum { get; init; }
    [JsonProperty("mGroupEnemyType")]
    private int GroupEnemyType { get; init; }
    [JsonProperty("mEnemy")]
    private List<int> SpawnIds { get; init; }
    public List<Enemy> Enemies { get; init; } = new();
    [JsonProperty("mParamInt")]
    private List<int> ParamInts { get; init; }
    [JsonProperty("mParamStr")]
    private List<string> ParamStrs { get; init; }

    private static readonly Dictionary<int, string> Symbols = new()
    {
      { 0, "Grizzly" },
      { 1, "Frog" },
      { 2, "Raven" },
      { 3, "Jelly" },
      { 4, "Wolf" },
      { 5, "Rhino" },
      { 6, "Shark" },
      { 7, "Wooly" },
      { 8, "Penguin" },
      { 9, "Puffer" },
      { 10, "Scorpion" },
      { 11, "Leon" },
      { 12, "T-Rex" },
      { 15, "Soul Pulvis" },
      { 17, "Pig" },
      { 18, "Turf Noise" },
      { 19, "Beringei"},
      { 20, "Fuya" },
      { 21, "Susukichi" },
      { 22, "Mr. Mew" },
      { 23, "Grus" },
      { 24, "Iris" },
      { 25, "Leo Armo" },
      { 26, "Cervus" },
      { 27, "Motoi" },
      { 28, "Shiba" },
      { 29, "Phoenix" },
      { 30, "Deep Rivers Society" },
      { 31, "Purehearts" },
      { 32, "Variabeauties" },
      { 33, "Ruinbringers" }
    };

    public void PostInit(Enemies enemies, List<SpawnData> spawnData)
    {
      foreach (var spawnId in SpawnIds)
      {
        var targetSpawn = spawnData.First(spawn => spawn.Id == spawnId);
        var targetEnemy = enemies.FirstOrDefault(enemy => enemy.Data.Id == targetSpawn.EnemyId);
        Enemies.Add(targetEnemy);
      }
    }
  }
}
