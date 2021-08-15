using System;
using System.Collections.Generic;
using System.Linq;
using NEOTool.Enemy;
using NEOTool.Event;
using NEOTool.Shop;
using NEOTool.Text;
using Newtonsoft.Json;
namespace NEOTool.Pins
{
  public enum PsychInputs
  {
    TopButton = 1,
    LeftButton = 2,
    LeftShoulder = 3,
    RightShoulder = 4,
    LeftTrigger = 5,
    RightTrigger = 6
  }
  
  public class Pin
  {
    [JsonProperty("mId")]
    public int Id { get; init; }
    public string Name { get; private set; }
    [JsonProperty("mItemId")]
    public int ItemId { get; init; }
    [JsonProperty("mBrand")]
    public int BrandId { get; init; }
    public string Brand => BrandNames[BrandId];
    [JsonProperty("mNameChance")]
    public string BeatdropNameToken { get; private set; }
    [JsonProperty("mInfoChance")]
    public string BeatdropInfoToken { get; set; }
    [JsonProperty("mPsychic")]
    private int PsychId { get; init; }
    public Psych Psych { get; private set; }
    [JsonProperty("mPsychicKey")]
    public PsychInputs Input { get; init; }
    [JsonProperty("mAttack")]
    public int BaseAttack { get; init; }
    [JsonProperty("mAddAttack")]
    public int AttackGainPerLevel { get; init; }
    [JsonProperty("mMaxValue")]
    public decimal BaseUses { get; init; }
    [JsonProperty("mAddMaxValue")]
    public decimal UsesGainPerLevel { get; init; }
    [JsonProperty("mHoldBasicCost")]
    public int HoldBasicCost { get; init; }
    [JsonProperty("mChargeTime")]
    public decimal ChargeTime { get; init; }
    [JsonProperty("mComboCount")]
    public int ComboCount { get; init; }
    [JsonProperty("mRebootTime")]
    public decimal BaseRebootTime { get; init; }
    [JsonProperty("mRebootTimeDec")]
    public decimal RebootTimeLossPerLevel { get; init; }
    [JsonProperty("mBootTime")]
    public decimal BaseBootTime { get; init; }
    [JsonProperty("mBootTimeDec")]
    public decimal BootTimeLossPerLevel { get; init; }
    [JsonProperty("mAutoRecoverTime")]
    public decimal BaseAutoRecoverTime { get; init; }
    [JsonProperty("mAutoRecoverTimeDec")]
    public decimal AutoRecoverTimeLossPerLevel { get; init; }
    [JsonProperty("mMaxLevel")]
    public int MaxLevel { get; init; }
    [JsonProperty("mLevelUpType")]
    public int LevelUpType { get; init; }
    [JsonProperty("mLevelUpRate")]
    public decimal LevelUpRate { get; init; }
    public int PpToMax { get; private set; }
    [JsonProperty("mAbility")]
    public List<int> Abilities { get; init; }
    [JsonProperty("mPairAbility")]
    public int EnsembleAbilityId { get; init; }
    [JsonProperty("mComboDamage")]
    public int ComboDamage { get; init; }
    [JsonProperty("mAddSellPrice")]
    public int SellPriceAddedPerLevel { get; init; }
    [JsonProperty("mSellPrice")]
    public int BaseSellPrice { get; init; }
    [JsonProperty("mRarity")]
    public bool Uber { get; init; }
    [JsonProperty("mSortIndex")]
    public int CollectionId { get; init; }
    [JsonProperty("mSortPsychic")]
    private int PsychSortId { get; init; }
    [JsonProperty("mBadgeSpriteName")]
    public string SpriteName { get; init; }
    [JsonProperty("mBadgeSpriteAtlas")]
    private string SpriteAtlas { get; init; }
    [JsonProperty("mBadgeClass")]
    public int Class { get; init; }
    [JsonProperty("mBadgeCategory")]
    public int Category { get; init; }
    [JsonProperty("mBadgePsychicType")]
    public int InputTypeId { get; init; }
    [JsonProperty("mEvolutionLevel")]
    public int EvolutionLevel { get; init; }
    public bool HasEvolution => EvolutionTargetId != -1;
    public Pin EvolutionTarget { get; private set; }
    [JsonProperty("mEcolutionCommon")]
    private int EvolutionTargetId { get; init; }
    public bool HasMutation => Mutation.Any(mutVal => mutVal != -1);
    [JsonProperty("mEvolutionBadge")]
    public Mutation Mutation { get; private set; }
    [JsonProperty("mChancetimeType")]
    public int BeatdropType { get; init; }
    [JsonProperty("mMashupElement")]
    public int MashupElement { get; init; }
    [JsonProperty("mInfoMovie")]
    private int InfoMovie { get; init; }
    public string PsychName { get; private set; }

    // Does not include BLACK HONEY CHILI COOKIE, since no pins use that brand.
    public static string[] BrandNames =
    {
      "Unbranded", "Top o' Topo", "Joli bécot", "Tigre PUNKS", "Cony×Cony", "RyuGu", "garagara", "IL CAVALLO DEL RE", "Shepherd House", "Jupiter of the Monkey", "MONOCROW", "NATURAL PUPPY", "HOG FANG", "Gatto Nero", "croaky panic"
    };

    // This is for determining the earliest day that a mutation can be done.
    private static Dictionary<int, Days.DaysEnum> EarliestDaysCharactersJoin = new()
    {
      { 0, Days.DaysEnum.GameStart }, // Rindo
      { 1, Days.DaysEnum.W2D7 },      // Shoka
      { 2, Days.DaysEnum.GameStart }, // Fret
      { 3, Days.DaysEnum.W1D4 },      // Nagi
      { 4, Days.DaysEnum.W2D3 },      // Beat
      { 5, Days.DaysEnum.W3D5 },      // Neku
      { 6, Days.DaysEnum.AnotherDay } // Minamimoto
    };

    public void FirstPostInit(List<Pin> pins, GameText gameText, int pinOrder, Dictionary<int, string> psychNames, List<Psych> psychs)
    {
      if (HasEvolution)
      {
        EvolutionTarget = pins.First(pin => pin.Id == EvolutionTargetId);
      }
      else if (HasMutation)
      {
        Mutation.PostInit(pins);
      }
      if (HasMutation == false)
      {
        Mutation = new Mutation();
        for (int dummyIndex = 0; dummyIndex < 7; dummyIndex += 1) { Mutation.Add(-1); }
      }
      if (PsychId > -1)
      {
        Psych = psychs.First(psych => psych.Id == PsychId);
        PsychName = gameText[psychNames[PsychId]].English;
      }
    }

    public void SecondPostInit(List<AllItemsEntry> allItems, GameText gameText)
    {
      var nameStringToken = allItems.First(entry => entry.ItemId == ItemId).NameToken;
      Name = gameText[nameStringToken].English;
    }

    public void ThirdPostInit(PinGrowthTable pinGrowth)
    {
      PpToMax = (int)Math.Ceiling(pinGrowth.Select(entry => entry[LevelUpType]).Take(MaxLevel).Sum() * LevelUpRate);
    }
    
    // TODO: Scramble Slam
    public Tuple<Days.DaysEnum, string> SolveEarliestSource(
      string masterDataLocation,
      List<Pin> pins,
      Enemies enemies,
      List<ShopGood> shopGoods,
      Dictionary<int, string> shopNames,
      List<ScenarioReward> scenarioRewards,
      List<PigData> pigs,
      List<GroupData> enemyGroups,
      GameText gameText,
      List<string> mapNameTokens)
    {
      Days.DaysEnum earliestDay = Days.DaysEnum.Invalid;
      String earliestSource = string.Empty;
      // First, check for the earliest source from enemies.
      var earliestEnemyDay = enemies.GetEarliestDayForPin(enemyGroups, this);
      if (earliestEnemyDay != null && earliestEnemyDay.Item1 != Days.DaysEnum.Invalid)
      {
        earliestDay = earliestEnemyDay.Item1;
        var enemiesSourceLines = new List<string>
        {
          $"  {Days.Strings[(int)earliestDay]}, from the following enemies:"
        };
        foreach (var enemy in earliestEnemyDay.Item2)
        {
          var difficultiesDroppedOn = enemy.Data.GetDifficultiesPinIsDroppedOn(this);
          enemiesSourceLines.Add($"    {enemy.Report.Name} ({string.Join(", " , difficultiesDroppedOn.Select(diff => diff.ToString()))})");
        }
        earliestSource = string.Join('\n', enemiesSourceLines);
      }
      // ShopGoods are next. Reminder that Day = 100 Vip = 10 entries are either SN rewards or unused; we can just ignore them here,
      // as a result. Additionally, disallow Brave & Ambitious and TOKYU HANDS 1F, since those just sell things we already parse.
      var usedShopGoods = shopGoods.Where(good => good.ReleaseDay != 100 && good.RequiredVipLevel != 10
                                          && good.ShopId != 39&& good.ShopId != 11);
      var firstPinShopGood = usedShopGoods.FirstOrDefault(good => good.ItemId == ItemId);
      if (firstPinShopGood != null)
      {
        var shopGoodDay = (Days.DaysEnum)firstPinShopGood.ReleaseDay;
        if (earliestDay > shopGoodDay)
        {
          earliestDay = shopGoodDay;
          var shopName = gameText[shopNames[firstPinShopGood.ShopId]];
          var requiredVipLevel = firstPinShopGood.RequiredVipLevel == 0 ? 1 : firstPinShopGood.RequiredVipLevel;
          earliestSource = requiredVipLevel != 1 ? $"  {Days.Strings[(int)earliestDay]}, from shop {shopName}, with VIP Level {requiredVipLevel}" : $"  {Days.Strings[(int)earliestDay]}, from shop {shopName}";
        }
      }
      // Scenario rewards are easy.
      var firstScenarioRewardForPin = scenarioRewards
        .Where(sr => sr.DayId >= 0)
        .FirstOrDefault(sr => sr.FirstRewardItemId == ItemId);
      if (firstScenarioRewardForPin != null)
      {
        var scenarioRewardDay = (Days.DaysEnum)firstScenarioRewardForPin.DayId;
        if (earliestDay > scenarioRewardDay)
        {
          earliestDay = scenarioRewardDay;
          earliestSource = $"  {Days.Strings[(int)earliestDay]}, from a scenario reward";
        }
      }
      // Pigs.
      var pigThatDropsThisPin = pigs.FirstOrDefault(pig => pig.DropId == Id);
      if (pigThatDropsThisPin != null)
      {
        var pigGroup = enemyGroups.First(group => group.GroupId == pigThatDropsThisPin.EnemyGroupId);
        var pigDay = (Days.DaysEnum)pigGroup.Day;
        if (earliestDay > pigDay)
        {
          earliestDay = pigDay;
          earliestSource = $"  {Days.Strings[(int)earliestDay]}, from a pig at {gameText[mapNameTokens[pigGroup.MapId]]}";
        }
      }
      // Evolution is easy, all we need to do is get the prereq's earliest day, and then set the new source.
      var evolutionPrereqPin = pins.FirstOrDefault(pin => pin.EvolutionTarget == this);
      if (evolutionPrereqPin != null)
      {
        var earliestEvolutionDay = evolutionPrereqPin.SolveEarliestSource(masterDataLocation, pins, enemies, shopGoods, shopNames, scenarioRewards, pigs, enemyGroups, gameText, mapNameTokens).Item1;
        if (earliestDay > earliestEvolutionDay)
        {
          earliestDay = earliestEvolutionDay;
          earliestSource = $"  {Days.Strings[(int)earliestDay]}, by evolving {evolutionPrereqPin.Name}";
        }
      }
      // Checking for mutations is a bit complex. First, check when the character who causes the mutation into this pin joins. If
      // they join after the earliest day for *that* pin, then the earliest day the pin can be mutated is when that character joins.
      // If they join before the earliest day for that pin, then the earliest day is the day you can get the prerequisite pin.
      var mutationPrereqPin = pins.FirstOrDefault(pin => pin.Mutation != null && pin.Mutation.Target == this);
      if (mutationPrereqPin != null)
      {
        var prereqPinEarliestDay = mutationPrereqPin.SolveEarliestSource(masterDataLocation, pins, enemies, shopGoods, shopNames, scenarioRewards, pigs, enemyGroups, gameText, mapNameTokens).Item1;
        var targetCharacterJoinDay = EarliestDaysCharactersJoin[mutationPrereqPin.Mutation.CharacterId];
        var earliestMutationDay = Days.DaysEnum.Invalid;
        earliestMutationDay = prereqPinEarliestDay > targetCharacterJoinDay ? prereqPinEarliestDay : targetCharacterJoinDay;
        if (earliestDay > earliestMutationDay)
        {
          earliestDay = earliestMutationDay;
          earliestSource = $"  {Days.Strings[(int)earliestDay]}, by mutating {mutationPrereqPin.Name} with {mutationPrereqPin.Mutation.Character}";
        }
      }
      return new Tuple<Days.DaysEnum, string>(earliestDay, earliestSource);
    }

    public override string ToString()
    {
      if (PsychName != null)
      {
        return $"{Name} ({PsychName}) ({PpToMax} PP)";
      }
      return Name;
    }
  }

  public class Mutation : List<int>
  {
    public int CharacterId
    {
      get
      {
        if (RindoTargetId != -1) { return 0; }
        if (ShokaTargetId != -1) { return 1; }
        if (FretTargetId != -1) { return 2; }
        if (NagiTargetId != -1) { return 3; }
        if (BeatTargetId != -1) { return 4; }
        if (NekuTargetId != -1) { return 5; }
        if (MinamimotoTargetId != -1) { return 6; }
        return -1;
      }
    }

    private static string[] CharacterNames = {
      "Rindo", "Shoka", "Fret", "Nagi", "Beat", "Neku", "Minamimoto"
    };

    public string Character => CharacterNames[CharacterId];
    
    public Pin Target { get; private set; }
    private int RindoTargetId => this[0];
    private int ShokaTargetId => this[1];
    private int FretTargetId => this[2];
    private int NagiTargetId => this[3];
    private int BeatTargetId => this[4];
    private int NekuTargetId => this[5];
    private int MinamimotoTargetId => this[6];

    public void PostInit(List<Pin> pins)
    {
      Target = pins.First(pin => pin.Id == this[CharacterId]);
    }

    public override string ToString()
    {
      if (CharacterId != -1)
      {
        return $"To {Target.Name} with {Character}";
      }
      return "None";
    }
  }
}
