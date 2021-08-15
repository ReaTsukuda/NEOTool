using Newtonsoft.Json;
namespace NEOTool.Enemy
{
  public class SpawnData
  {
    [JsonProperty("mId")]
    public int Id { get; init; }
    [JsonProperty("mEnemy")]
    public int EnemyId { get; init; }
  }
}
