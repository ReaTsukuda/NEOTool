using System.Collections.Generic;
using System.Linq;
using NEOTool.Pins;
using Newtonsoft.Json;
namespace NEOTool.Enemy
{
  public class EnemyData
  {
    public enum Difficulties
    {
      Easy = 0,
      Normal = 1,
      Hard = 2,
      Ultimate = 3
    }
    
    [JsonProperty("mId")]
    public int Id { get; set; }
    [JsonProperty("mClass")]
    private int EnemyClass { get; set; }
    [JsonProperty("mType")]
    private int EnemyType { get; set; }
    [JsonProperty("mTypeVersion")]
    private int EnemyTypeVersion { get; set; }
    [JsonProperty("mSoFileName")]
    private string SoFileName { get; set; }
    [JsonProperty("mResourceData")]
    private int ResourceData { get; set; }
    [JsonProperty("mBaseParam")]
    private int BaseParam { get; set; }
    [JsonProperty("mAttack")]
    private List<int> Attacks { get; set; }
    [JsonProperty("mAttackWeightEasy")]
    private List<decimal> AttackWeightEasy { get; set; }
    [JsonProperty("mAttackWeightNormal")]
    private List<decimal> AttackWeightNormal { get; set; }
    [JsonProperty("mAttackWeightHard")]
    private List<decimal> AttackWeightHard { get; set; }
    [JsonProperty("mAttackWeightUltimate")]
    private List<decimal> AttackWeightUltimate { get; set; }
    [JsonProperty("mShacHateGaugeMax")]
    public int RevengeValue { get; set; }
    [JsonProperty("mShacTriggerLine")]
    private int RevengeValueTriggerLineId { get; set; }
    [JsonProperty("mShacAttackIndex")]
    public int RevengeValueTriggerAttackId { get; set; }
    [JsonProperty("mShacStateName")]
    private string RevengeValueTriggerStateName { get; set; }
    [JsonProperty("mSightAngle")]
    private int SightAngle { get; set; }
    [JsonProperty("mScale")]
    private decimal Scale { get; set; }
    [JsonProperty("mExp")]
    private int EXP { get; set; }
    [JsonProperty("mBp")]
    private int PP { get; set; }
    [JsonProperty("mBattleTime")]
    private decimal TargetBattleTime { get; set; }
    [JsonProperty("mParam")]
    private List<int> Params { get; set; }
    [JsonProperty("mBlowedColRadius")]
    private int BlowedColRadius { get; set; }
    [JsonProperty("mDesperateSe")]
    private int MortalPerilSoundEffectId { get; set; }
    [JsonProperty("mEscapeSe")]
    private int EscapeSoundEffectId { get; set; }
    [JsonProperty("mDesperateVoice")]
    private int MortalPerilVoiceId { get; set; }
    [JsonProperty("mDrop")]
    private List<int> PinDropIds { get; set; }
    public List<Pin> PinDrops { get; } = new List<Pin>();
    [JsonProperty("mDropRate")]
    public List<decimal> PinDropRates { get; set; }
    [JsonProperty("mDynamicBoneFps")]
    private int DynamicBoneFps { get; set; }
    [JsonProperty("mDynamicBoneDistance")]
    private int DynamicBoneDistance { get; set; }
    [JsonProperty("mDiseaseSyncroUpRate")]
    public int PlagueGrooveMultiplier { get; set; }
    [JsonProperty("mDiseaseDamageCutRate")]
    public int PlagueDamageCutMultiplier { get; set; }
    [JsonProperty("mLevel")]
    private int Level { get; set; }
    [JsonProperty("mResultCp")]
    private int ResultCp { get; set; }

    public void PostInit(List<Pin> pins)
    {
      PinDrops.Add(pins.First(pin => pin.Id == PinDropIds[0]));
      PinDrops.Add(pins.First(pin => pin.Id == PinDropIds[1]));
      PinDrops.Add(pins.First(pin => pin.Id == PinDropIds[2]));
      PinDrops.Add(pins.First(pin => pin.Id == PinDropIds[3]));
    }

    public List<Difficulties> GetDifficultiesPinIsDroppedOn(Pin pin)
    {
      // If this enemy doesn't drop the requested pin at all, bail out early with a null.
      if (PinDrops.Contains(pin) == false)
      {
        return null;
      }
      var result = new List<Difficulties>();
      for (int difficultyIndex = 0; difficultyIndex < 4; difficultyIndex += 1)
      {
        if (PinDrops[difficultyIndex] == pin)
        {
          result.Add((Difficulties)difficultyIndex);
        }
      }
      return result;
    }

    public bool IsPinDroppedOnlyOnDifficulty(Pin pin, Difficulties targetDifficulty)
    {
      // If this enemy doesn't drop the provided pin, bail early.
      if (PinDrops.Contains(pin) == false) { return false; }
      var otherDifficulties = new List<Difficulties>()
      {
        Difficulties.Easy,
        Difficulties.Normal,
        Difficulties.Hard,
        Difficulties.Ultimate
      };
      otherDifficulties.Remove(targetDifficulty);
      foreach (var difficulty in otherDifficulties)
      {
        if (PinDrops[(int)difficulty] == pin) { return false; }
      }
      return true;
    }
  }
}
