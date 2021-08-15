using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using NEOTool.Enemy;
using NEOTool.Event;
using NEOTool.Pins;
using NEOTool.Portraits;
using NEOTool.Shop;
using NEOTool.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NEOTool
{
  class Program
  {
    static void Main(string[] args)
    {
      if (args.Contains("portraits"))
      {
        PortraitStitching.StitchCharacter(args[1]);
        return;
      }
      var masterDataLocation = args[6];
      var gameText = new GameText(args[0], args[1], args[2], args[3], args[4], args[5]);
      var psychNames = GetPsychNameTokens(masterDataLocation);
      var shopNames = GetShopNameTokens(masterDataLocation);
      var attacks = (JsonConvert.DeserializeObject(File.ReadAllText(Path.Combine(masterDataLocation, "Attack.txt"))) as JObject)
        .GetValue("mTarget")
        .ToObject(typeof(List<Attack>)) as List<Attack>;
      var attackHits = (JsonConvert.DeserializeObject(File.ReadAllText(Path.Combine(masterDataLocation, "AttackHit.txt"))) as JObject)
        .GetValue("mTarget")
        .ToObject(typeof(List<AttackHit>)) as List<AttackHit>;
      var attackComboSets = (JsonConvert.DeserializeObject(File.ReadAllText(Path.Combine(masterDataLocation, "AttackComboSet.txt"))) as JObject)
        .GetValue("mTarget")
        .ToObject(typeof(List<AttackComboSet>)) as List<AttackComboSet>;
      var psychs = (JsonConvert.DeserializeObject(File.ReadAllText(Path.Combine(masterDataLocation, "Psychic.txt"))) as JObject)
        .GetValue("mTarget")
        .ToObject(typeof(List<Psych>)) as List<Psych>;
      attacks.ForEach(attack => attack.PostInit(attackHits));
      attackComboSets.ForEach(acs => acs.PostInit(attacks));
      psychs.ForEach(psych => psych.PostInit(attackComboSets));
      var pins = Pins(Path.Combine(masterDataLocation, "Badge.txt"), gameText, psychNames, psychs);
      var shopResult = Shop(gameText, masterDataLocation, pins);
      var allItems = shopResult.Item1;
      var shopGoods = shopResult.Item2;
      var enemies = new Enemies(masterDataLocation, gameText, pins);
      pins.ForEach(pin => pin.SecondPostInit(allItems, gameText));
      var scenarioRewards = (JsonConvert.DeserializeObject(File.ReadAllText(Path.Combine(masterDataLocation, "ScenarioRewards.txt"))) as JObject)
        .GetValue("mTarget")
        .ToObject(typeof(List<ScenarioReward>)) as List<ScenarioReward>;
      var pigData = ((JsonConvert.DeserializeObject(File.ReadAllText(Path.Combine(masterDataLocation, "PigData.txt"))) as JObject)
        .GetValue("mTarget")
        .ToObject(typeof(List<PigData>)) as List<PigData>).Where(pig => pig.BattleTime != 999).ToList();
      var spawnData = ((JsonConvert.DeserializeObject(File.ReadAllText(Path.Combine(masterDataLocation, "SpawnData.txt"))) as JObject)
        .GetValue("mTarget")
        .ToObject(typeof(List<SpawnData>)) as List<SpawnData>).ToList();
      var enemyGroups = (JsonConvert.DeserializeObject(File.ReadAllText(Path.Combine(masterDataLocation, "GroupData.txt"))) as JObject)
        .GetValue("mTarget")
        .ToObject(typeof(List<GroupData>)) as List<GroupData>;
      enemyGroups.ForEach(group => group.PostInit(enemies, spawnData));
      RemovePlagueNoiseGhostFightsFromEnemyGroups(enemies, enemyGroups, "F:/NEO/PlagueNoiseDayOverride.csv");
      var abilities = ((JsonConvert.DeserializeObject(File.ReadAllText(Path.Combine(masterDataLocation, "Ability.txt"))) as JObject)
        .GetValue("mTarget")
        .ToObject(typeof(List<Ability.Ability>)) as List<Ability.Ability>).Where(ability => ability.IsBlank == false).ToList();
      var abilityDescriptions = GetAbilityDescriptionFormats("F:/NEO/AbilityDescriptions.txt");
      abilities.ForEach(ability => ability.PostInit(gameText, abilityDescriptions));
      var mapNameTokens = ((JsonConvert.DeserializeObject(File.ReadAllText(Path.Combine(masterDataLocation, "FieldMap.txt"))) as JObject)
        .GetValue("mTarget") as JArray).Select(entry => (entry as JObject).GetValue("mLocationName").ToString()).ToList();
      var baseUsesTypes = new BaseUsesTypes("F:/NEO/BaseUsesTypes.csv");
      var pinGrowth = new PinGrowthTable(masterDataLocation);
      pins.ForEach(pin => pin.ThirdPostInit(pinGrowth));
      
      var wikiLines = new List<string>();
      wikiLines.Add("==Pins==");
      for (int brandIndex = 1; brandIndex < 15; brandIndex += 1)
      {
        wikiLines.AddRange(WikiPinFormatting.GetRowsForBrand(brandIndex, pins, gameText, baseUsesTypes, pinGrowth));
      }
      wikiLines.AddRange(WikiPinFormatting.GetRowsForBrand(0, pins, gameText, baseUsesTypes, pinGrowth));
      File.WriteAllLines("F:/NEO/Tables.txt", wikiLines);
    }

    /// <summary>
    /// Groups that are for "ghost" Plague Noise fights need to be removed, as they can cause, among other things, the earliest pin source solver to get
    /// confused. Plague Grizzlies most certainly cannot be defeated as early as W2D4.
    /// </summary>
    static void RemovePlagueNoiseGhostFightsFromEnemyGroups(Enemies enemies, List<GroupData> enemyGroups, string csvPath)
    {
      var csvLines = File.ReadAllLines(csvPath);
      foreach (var line in csvLines)
      {
        var splitLine = line.Split(',');
        var enemyId = int.Parse(splitLine[0]);
        var dayOverride = int.Parse(splitLine[1]);
        var targetEnemy = enemies[enemyId];
        var targetEnemyGhostGroups = enemyGroups
          .Where(group => group.Enemies.Contains(targetEnemy))
          .Where(group => group.Day < dayOverride).ToList();
        targetEnemyGhostGroups.ForEach(group => enemyGroups.Remove(group));
      }
    }

    static void WriteEnemyInfo(Enemies enemies)
    {
      var result = new List<string>();
      foreach (var enemy in enemies)
      {
        result.Add($"{enemy.Report.Name},{enemy.Data.PinDropRates[0] * 100},{enemy.Data.PinDropRates[1] * 100},{enemy.Data.PinDropRates[2] * 100},{enemy.Data.PinDropRates[3] * 100}");
        // result.Add(enemy.Report.Name);
        // for (int dropIndex = 0; dropIndex < enemy.Data.PinDrops.Count; dropIndex += 1)
        // {
        //   result.Add($"  {enemy.Data.PinDrops[dropIndex].Name}");
        //   result.Add($"    Base:        {Decimal.Round(enemy.Data.PinDropRates[dropIndex] * 100, 2)}%");
        //   result.Add($"    DR for 100%: {Decimal.Round(100 / (enemy.Data.PinDropRates[dropIndex] * 100))}");
        // }
        // result.Add("");
      }
      File.WriteAllLines("F:/NEO/EnemyDrops.csv", result);
    }

    static void WritePinSources(
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
      var result = new List<string>();
      var pinNumber = 1;
      foreach (var pin in pins)
      {
        result.Add($"{pin.Name} ({pin.PsychName}) (No. {pinNumber.ToString("D3")})");
        // Check enemy drops first.
        var enemiesThatDropThisPin = enemies.Where(enemy => enemy.Data.PinDrops.Contains(pin)).ToList();
        if (enemiesThatDropThisPin.Count > 0)
        {
          foreach (var enemy in enemiesThatDropThisPin)
          {
            if (enemy.Report.Name.Contains("Pig")) { continue; }
            result.Add($"  Enemy: {enemy.Report.Name}");
            if (enemy.Data.PinDrops[0] == pin) { result.Add($"    Easy:     {Decimal.Round(enemy.Data.PinDropRates[0] * 100, 2)}%"); }
            if (enemy.Data.PinDrops[1] == pin) { result.Add($"    Normal:   {Decimal.Round(enemy.Data.PinDropRates[1] * 100, 2)}%"); }
            if (enemy.Data.PinDrops[2] == pin) { result.Add($"    Hard:     {Decimal.Round(enemy.Data.PinDropRates[2] * 100, 2)}%"); }
            if (enemy.Data.PinDrops[3] == pin) { result.Add($"    Ultimate: {Decimal.Round(enemy.Data.PinDropRates[3] * 100, 2)}%"); }
          }
        }
        // After enemies, check ShopGoods.
        var shopGoodsForThisPin = shopGoods.Where(good => good.ItemId == pin.ItemId).ToList();
        if (shopGoodsForThisPin.Count > 0)
        {
          foreach (var shopGood in shopGoodsForThisPin)
          {
            if (shopGood.RequiredVipLevel != 10 && shopGood.ReleaseDay != 100)
            {
              result.Add($"  {gameText[shopNames[shopGood.ShopId]]}");
              if (shopGood.Exchange != null)
              {
                foreach (var exchangePinEntry in shopGood.Exchange.Pins)
                {
                  var exchangePin = exchangePinEntry.Item1;
                  var amount = exchangePinEntry.Item2;
                  result.Add($"    {exchangePin.Name} x{amount}");
                }
              }
              else { result.Add($"    Price: {shopGood.Price}"); }
              result.Add($"    Available on {shopGood.ReleaseDayString}");
              if (shopGood.RequiredVipLevel > 1)
              {
                result.Add($"    Requires VIP Level {shopGood.RequiredVipLevel}");
              }
            }
          }
        }
        // After ShopGoods, check if the pin evolves from something.
        var pinEvolvesFromThisPin = pins.FirstOrDefault(evolvePin => evolvePin.HasEvolution && evolvePin.EvolutionTarget == pin);
        if (pinEvolvesFromThisPin != null)
        {
          result.Add($"  Evolves from {pinEvolvesFromThisPin.Name}");
        }
        // After evolution, check if the pin mutates from something.
        var pinMutatesFromThisPin = pins.FirstOrDefault(mutatePin => mutatePin.HasMutation && mutatePin.Mutation.Target == pin);
        if (pinMutatesFromThisPin != null)
        {
          result.Add($"  Mutates from {pinMutatesFromThisPin.Name} when equipped by {pinMutatesFromThisPin.Mutation.Character}");
        }
        // After mutation, check if the pin is given from any story rewards.
        var pinGivenFromTheseScenarioRewards = scenarioRewards.Where(
          reward => reward.FirstRewardItemId == pin.ItemId || reward.RepeatRewardItemId == pin.ItemId);
        if (pinGivenFromTheseScenarioRewards != null)
        {
          int countFromScenarioRewards = 0;
          foreach (var scenarioReward in pinGivenFromTheseScenarioRewards)
          {
            //countFromScenarioRewards += scenarioReward.FirstRewardCount;
            if (scenarioReward.FirstRewardItemId == pin.ItemId)
            {
              result.Add($"  Scenario Reward ID {scenarioReward.Id} First (x{scenarioReward.FirstRewardCount})");
            }
            if (scenarioReward.RepeatRewardItemId == pin.ItemId)
            {
              result.Add($"  Scenario Reward ID {scenarioReward.Id} Repeat (x{scenarioReward.RepeatRewardCount})");
            }
          }
          if (countFromScenarioRewards > 0)
          {
            result.Add($"  Scenario Reward (x{countFromScenarioRewards})");
          }
          pinNumber += 1;
        }
        // After story rewards, check for pig drops.
        var pigsThatDropThisPin = pigs.Where(pig => pig.DropId == pin.Id);
        foreach (var pig in pigsThatDropThisPin)
        {
          var enemyGroupForPig = enemyGroups.FirstOrDefault(group => group.GroupId == pig.EnemyGroupId);
          result.Add($"  Pig on {Days.Strings[enemyGroupForPig.Day]}, at {gameText[mapNameTokens[enemyGroupForPig.MapId]].English}");
        }
        result.Add("");
      }
      File.WriteAllLines("F:/NEO/PinSources.txt", result);
    }

    static void WriteEarliestPinSources(
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
      var result = new List<string>();
      foreach (var pin in pins)
      {
        var earliestPinSource = pin.SolveEarliestSource(masterDataLocation, pins, enemies, shopGoods, shopNames, scenarioRewards, pigs, enemyGroups, gameText, mapNameTokens);
        result.Add($"{pin.Name}");
        result.Add(earliestPinSource.Item2);
        result.Add(string.Empty);
      }
      File.WriteAllLines("F:/NEO/EarliestPinSources.txt", result);
    }

    static void WriteEarliestPinSourcesSortedByDay(
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
      var result = new List<string>();
      var daysAndPins = new SortedDictionary<Days.DaysEnum, List<List<string>>>();
      foreach (var pin in pins)
      {
        var earliestPinSource = pin.SolveEarliestSource(masterDataLocation, pins, enemies, shopGoods, shopNames, scenarioRewards, pigs, enemyGroups, gameText, mapNameTokens);
        if (daysAndPins.ContainsKey(earliestPinSource.Item1) == false) { daysAndPins.Add(earliestPinSource.Item1, new List<List<string>>()); }
        var pinLines = new List<string>();
        pinLines.Add($"{pin.Name}");
        pinLines.Add(earliestPinSource.Item2);
        pinLines.Add(string.Empty);
        daysAndPins[earliestPinSource.Item1].Add(pinLines);
      }
      foreach (var daysAndPinsEntry in daysAndPins)
      {
        foreach (var entry in daysAndPinsEntry.Value)
        {
          result.AddRange(entry);
        }
      }
      File.WriteAllLines("F:/NEO/EarliestPinSourcesByDay.txt", result);
    }

    static void WriteEnemyLocations(Enemies enemies, List<GroupData> enemyGroups, List<string> mapNameTokens, GameText gameText)
    {
      var lines = new List<string>();
      var noisepediaIndex = 1;
      foreach (var enemy in enemies)
      {
        // Getting the days and locations.
        var groupsWithEnemy = enemyGroups.Where(group => group.Enemies.Contains(enemy));
        var daysAndLocations = new Dictionary<int, List<int>>();
        foreach (var group in groupsWithEnemy)
        {
          if (group.Day == -1) { continue; }
          if (daysAndLocations.ContainsKey(group.Day) == false) { daysAndLocations.Add(group.Day, new List<int>()); }
          if (daysAndLocations[group.Day].Contains(group.MapId) == false) { daysAndLocations[group.Day].Add(group.MapId); }
        }
        // Writing that info to the output.
        lines.Add($"{enemy.Report.Name} (No. {noisepediaIndex})");
        foreach (var day in daysAndLocations.Keys)
        {
          daysAndLocations[day].Sort();
          lines.Add($"  {Days.Strings[day]}");
          foreach (var locationId in daysAndLocations[day])
          {
            var location = SpecialLocations.Names.ContainsKey(locationId) ? SpecialLocations.Names[locationId] : gameText[mapNameTokens[locationId]].English;
            lines.Add($"    {location}");
          }
        }
        lines.Add(string.Empty);
        noisepediaIndex += 1;
      }
      File.WriteAllLines("F:/NEO/ReadableEnemyLocations.txt", lines);
    }

    static List<Pin> Pins(string pinDataLocation, GameText gameText, Dictionary<int, string> psychNames, List<Psych> psychs)
    {
      var pins = ((JsonConvert.DeserializeObject(File.ReadAllText(pinDataLocation)) as JObject)
        .GetValue("mTarget")
        .ToObject(typeof(List<Pin>)) as List<Pin>);
      var pinsIterated = 0;
      pins.ForEach(pin => { pin.FirstPostInit(pins, gameText, pinsIterated += 1, psychNames, psychs); });
      pins.Sort(delegate(Pin x, Pin y)
      {
        if (x.CollectionId > y.CollectionId) { return 1; }
        if (x.CollectionId < y.CollectionId) { return -1; }
        return 0;
      });
      return pins;
    }

    static Tuple<Expressions, Speakers, List<MessageClip>> Event(string targetDirectory, GameText gameText)
    {
      var clips = new List<MessageClip>();
      using var expressionsReader = new StreamReader(new FileStream("F:/NEO/PoseFaceFileIDs.txt", FileMode.Open));
      var expressions = new Expressions(expressionsReader);
      using var speakersReader = new StreamReader(new FileStream("F:/NEO/UniqueSpeakers.txt", FileMode.Open));
      var speakers = new Speakers(speakersReader);
      foreach (var clipPath in Directory.GetFiles(targetDirectory))
      {
        using var clipReader = new BinaryReader(new FileStream(clipPath, FileMode.Open));
        var clipCandidate = new MessageClip(clipPath, clipReader, expressions, gameText, speakers);
        clips.Add(clipCandidate);
        //if (clipCandidate.TextId != string.Empty && gameText.ContainsKey(clipCandidate.TextId)) { clips.Add(clipCandidate); }
      }
      clips = clips.Distinct().ToList();
      clips.Sort(new MessageClipComparer());
      return new Tuple<Expressions, Speakers, List<MessageClip>>(expressions, speakers, clips);
    }

    static Tuple<List<AllItemsEntry>, List<ShopGood>> Shop(GameText gameText, string masterDataLocation, List<Pin> pins)
    {
      var allItems = ((JsonConvert.DeserializeObject(File.ReadAllText(Path.Combine(masterDataLocation, "AllItems.txt"))) as JObject)
          .GetValue("mTarget") as JArray)
        .ToObject<List<AllItemsEntry>>().ToList();
      var shopGoods = ((JsonConvert.DeserializeObject(File.ReadAllText(Path.Combine(masterDataLocation, "ShopGoods.txt"))) as JObject)
          .GetValue("mTarget") as JArray)
        .ToObject<List<ShopGood>>().Skip(3).ToList();
      shopGoods.ForEach(good => good.Name = gameText[allItems.First(entry => entry.ItemId == good.ItemId).NameToken].English);
      shopGoods.Sort(delegate(ShopGood x, ShopGood y)
      {
        if (x.ReleaseDay > y.ReleaseDay) { return 1; }
        if (x.ReleaseDay == y.ReleaseDay) { return 0; }
        return -1;
      });
      var exchanges = ((JsonConvert.DeserializeObject(File.ReadAllText(Path.Combine(masterDataLocation, "GoodsExchange.txt"))) as JObject)
        .GetValue("mTarget") as JArray).ToObject<List<Exchange>>();
      exchanges.ForEach(entry => entry.PostInit(pins));
      shopGoods.ForEach(entry => entry.PostInit(exchanges));
      return new Tuple<List<AllItemsEntry>, List<ShopGood>>(allItems, shopGoods);
    }

    static Dictionary<int, string> GetPsychNameTokens(string masterDataLocation)
    {
      var result = new Dictionary<int, string>();
      var psychNameArray = ((JsonConvert.DeserializeObject(File.ReadAllText(Path.Combine(masterDataLocation, "Psychic.txt"))) as JObject)
        .GetValue("mTarget") as JArray);
      foreach (JObject entry in psychNameArray)
      {
        var key = int.Parse(entry.GetValue("mId").ToString());
        var value = entry.GetValue("mPsychicName").ToString();
        result.Add(key, value);
      }
      return result;
    }

    static Dictionary<int, string> GetShopNameTokens(string masterDataLocation)
    {
      var result = new Dictionary<int, string>();
      var psychNameArray = ((JsonConvert.DeserializeObject(File.ReadAllText(Path.Combine(masterDataLocation, "Shop.txt"))) as JObject)
        .GetValue("mTarget") as JArray);
      foreach (JObject entry in psychNameArray)
      {
        var key = int.Parse(entry.GetValue("mId").ToString());
        var value = entry.GetValue("mName").ToString();
        result.Add(key, value);
      }
      return result;
    }

    static Dictionary<int, Dictionary<int, string>> GetAbilityDescriptionFormats(string csvLocation)
    {
      var result = new Dictionary<int, Dictionary<int, string>>();
      foreach (var line in File.ReadAllLines(csvLocation))
      {
        var splitLine = line.Split(',');
        var abilityId = int.Parse(splitLine[0]);
        var variantId = int.Parse(splitLine[1]);
        var description = splitLine.Length > 2 ? string.Join(",", splitLine.Skip(2)) : splitLine[2];
        if (result.ContainsKey(abilityId) == false) { result.Add(abilityId, new Dictionary<int, string>()); }
        result[abilityId].Add(variantId, description);
      }
      return result;
    }
  }
}
