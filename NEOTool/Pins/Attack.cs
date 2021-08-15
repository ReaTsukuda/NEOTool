using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
namespace NEOTool.Pins
{
  public class Attack
  {
    [JsonProperty("mId")]
    public int Id { get; init; }
    [JsonProperty("mHit")]
    private List<int> AttackHitIds { get; init; }
    public List<AttackHit> AttackHits { get; } = new();

    public override string ToString() => $"({string.Join(", ", AttackHits.Select(ah => $"{decimal.Round(ah.Efficiency * 100, 2)}% x{ah.HitCount}"))})";

    public void PostInit(List<AttackHit> attackHits)
    {
      foreach (var id in AttackHitIds)
      {
        AttackHits.Add(attackHits.Find(hit => hit.Id == id));
      }
    }
  }
}
