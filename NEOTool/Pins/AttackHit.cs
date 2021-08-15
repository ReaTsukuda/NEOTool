using System.Collections.Generic;
using Newtonsoft.Json;
namespace NEOTool.Pins
{
  public class AttackHit
  {
    [JsonProperty("mId")]
    public int Id { get; init; }
    [JsonProperty("mBullet")]
    private int BulletId { get; init; }
    [JsonProperty("mPower")]
    public decimal Efficiency { get; init; }
    [JsonProperty("mGainHate")]
    public int RevengeValue { get; init; }
    [JsonProperty("mHitTargetType")]
    private int HitTargetType { get; init; }
    [JsonProperty("mHitAirGroundType")]
    private int HitAirGroundType { get; init; }
    [JsonProperty("mHitCount")]
    public int HitCount { get; init; }
    [JsonProperty("mColAttach")]
    private int ColAttach { get; init; }
    [JsonProperty("mColAttachTarget")]
    private string ColAttachTarget { get; init; }
    [JsonProperty("mColType")]
    private int ColType { get; init; }
    [JsonProperty("mColRadius")]
    private int ColRadius { get; init; }
    [JsonProperty("mColParam")]
    private List<decimal> ColParam { get; init; }
    [JsonProperty("mHitStopDelay")]
    private decimal HitStopDelay { get; init; }
    [JsonProperty("mHitStopTime")]
    private decimal HitStopTime { get; init; }
    [JsonProperty("mHitStopTarget")]
    private int HitStopTarget { get; init; }
    [JsonProperty("mBlowType")]
    private int BlowType { get; init; }
    [JsonProperty("mBlowSpeed")]
    private int BlowSpeed { get; init; }
    [JsonProperty("mBlowAngle")]
    private decimal BlowAngle { get; init; }
    [JsonProperty("mCameraShake")]
    private int CameraShake { get; init; }
    [JsonProperty("mHitVibration")]
    private int HitVibration { get; init; }
    [JsonProperty("mHitEffect")]
    private int HitEffect { get; init; }
    [JsonProperty("mCrackEffect")]
    private int CrackEffect { get; init; }
    [JsonProperty("mCrackEffectScale")]
    private int CrackEffectScale { get; init; }
    [JsonProperty("mHitSe")]
    private int HitSoundEffect { get; init; }
    [JsonProperty("mEnchant")]
    public List<int> Disables { get; init; }
    [JsonProperty("mEnchantInfectionRate")]
    public List<decimal> DisableInflictionChances { get; init; }
    [JsonProperty("mElement")]
    public List<int> Elements { get; init; }
    [JsonProperty("mChanceType")]
    public int BeatdropTypeId { get; init; }
    [JsonProperty("mDiseaseSyncroDamageRate")]
    private int PlagueGrooveDamageRate { get; init; }
    [JsonProperty("mIsReleasedBySpecialAttack")]
    private bool IsReleasedBySpecialAttack { get; init; }
    [JsonProperty("mAttackIgnoreInvincibilityType")]
    private int IgnoreInvincibilityType { get; init; }
    [JsonProperty("mPsychicShieldVaidityRate")]
    private int ShieldValidityRate { get; init; }
  }
}
