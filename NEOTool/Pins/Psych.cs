using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
namespace NEOTool.Pins
{
  public class Psych
  {
    [JsonProperty("mId")]
    public int Id { get; init; }
    [JsonProperty("mPsychicName")]
    private string NameToken { get; init; }
    [JsonProperty("mChargeMinTime")]
    public decimal MinimumChargeTime { get; init; }
    [JsonProperty("mChanceTriggerHitCount")]
    public int BeatdropHitCount { get; init; }
    [JsonProperty("mAttackComboSet")]
    private int AttackComboSetId { get; init; }
    public AttackComboSet AttackComboSet { get; private set; }
    [JsonProperty("mControllTypeName")]
    public string ControlTypeToken { get; init; }

    public void PostInit(List<AttackComboSet> attackComboSets)
    {
      AttackComboSet = attackComboSets.First(acs => acs.Id == AttackComboSetId);
    }
  }
}
