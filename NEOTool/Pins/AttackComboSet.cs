using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
namespace NEOTool.Pins
{
  public class AttackComboSet
  {
    [JsonProperty("mId")]
    public int Id { get; init; }
    [JsonProperty("mAttack")]
    private List<int> AttackIds { get; init; }
    public List<Attack> Attacks { get; } = new();

    public override string ToString()
    {
      var efficiencyStrings = new List<string>();
      Attacks.ForEach(atk => efficiencyStrings.Add(atk.ToString()));
      return string.Join(", ", efficiencyStrings);
    }

    public void PostInit(List<Attack> attacks)
    {
      AttackIds.ForEach(id => Attacks.Add(attacks.First(atk => atk.Id == id)));
    }
  }
}
