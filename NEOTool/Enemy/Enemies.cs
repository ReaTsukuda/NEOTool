using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NEOTool.Pins;
using NEOTool.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
namespace NEOTool.Enemy
{
  public class Enemies : List<Enemy>
  {
    public Enemies(string masterDataPath, GameText gameText, List<Pin> pins)
    {
      var reports = (JsonConvert.DeserializeObject(File.ReadAllText(Path.Combine(masterDataPath, "EnemyReport.txt"))) as JObject)
        .GetValue("mTarget")
        .ToObject(typeof(List<EnemyReport>)) as List<EnemyReport>;
      reports.ForEach(report => report.PostInit(gameText));
      var enemyData = (JsonConvert.DeserializeObject(File.ReadAllText(Path.Combine(masterDataPath, "EnemyData.txt"))) as JObject)
        .GetValue("mTarget")
        .ToObject(typeof(List<EnemyData>)) as List<EnemyData>;
      enemyData.ForEach(data => data.PostInit(pins));
      foreach (var report in reports)
      {
        var targetData = enemyData.First(data => data.Id == report.EnemyDataId);
        Add(new Enemy()
        {
          Report = report,
          Data = targetData
        });
      }
    }

    public Tuple<Days.DaysEnum, List<Enemy>> GetEarliestDayForPin(List<GroupData> enemyGroups, Pin pin)
    {
      var enemiesThatDropThePin = this.Where(enemy => enemy.Data.PinDrops.Contains(pin)).ToList();
      // If no enemies drop the pin, bail early by just returning null.
      if (enemiesThatDropThePin.Count == 0) { return null; }
      var earliestDayForThisPin = Days.DaysEnum.Invalid;
      foreach (var enemy in enemiesThatDropThePin)
      {
        var earliestDayPrereq = enemyGroups
          .Where(group => group.Enemies.Contains(enemy))
          .Where(group => group.Day != -1).ToList();
        // Forgive me for not knowing why, but Phoenix Cantus somehow registers as not having any enemies in its group. I'll just have
        // to hardcode its drops.
        if (earliestDayPrereq.Count == 0)
        {
          if (enemy.Data.IsPinDroppedOnlyOnDifficulty(pin, EnemyData.Difficulties.Ultimate)
            && earliestDayForThisPin == Days.DaysEnum.Invalid)
          {
            earliestDayForThisPin = Days.DaysEnum.AnotherDay;
          }
          else if (earliestDayForThisPin == Days.DaysEnum.Invalid)
          {
            earliestDayForThisPin = Days.DaysEnum.W3D7Part3;
          }
          break;
        }
        var earliestDayForThisEnemy = (Days.DaysEnum)earliestDayPrereq.Min(group => group.Day);
        // If the pin is only dropped from this enemy on Ultimate, and the earliest day for this enemy is before Another Day, the
        // earliest day for this enemy is Another Day.
        if (enemy.Data.IsPinDroppedOnlyOnDifficulty(pin, EnemyData.Difficulties.Ultimate)
          && earliestDayForThisEnemy < Days.DaysEnum.AnotherDay)
        {
          earliestDayForThisEnemy = Days.DaysEnum.AnotherDay;
        }
        // If the pin is only dropped from this enemy on Easy or Hard, and the earliest day for this enemy is before W1D4, the
        // earliest day for this enemy is W1D4.
        else if ((enemy.Data.PinDrops[(int)EnemyData.Difficulties.Easy] == pin || enemy.Data.PinDrops[(int)EnemyData.Difficulties.Hard] == pin)
          && enemy.Data.PinDrops[(int)EnemyData.Difficulties.Normal] != pin
          && earliestDayForThisEnemy < Days.DaysEnum.W1D4)
        {
          earliestDayForThisEnemy = Days.DaysEnum.W1D4;
        }
        // If the current earliest day for this pin is later than the earliest day for this enemy, then set it to the earliest day
        // for this enemy.
        if (earliestDayForThisPin > earliestDayForThisEnemy)
        {
          earliestDayForThisPin = earliestDayForThisEnemy;
        }
      }
      // Now that we've got the earliest day this pin can be found on an enemy, we need to find all the enemies that drop this pin
      // that can be encountered on the earliest day.
      var earliestDayEnemies = enemyGroups
        .Where(group => group.Enemies.Any(groupEnemy => enemiesThatDropThePin.Any(dropEnemy => groupEnemy == dropEnemy)))
        .Where(group => group.Day == (int)earliestDayForThisPin)
        .SelectMany(group => group.Enemies)
        .Where(enemy => enemy != null)
        .Where(enemy => enemy.Data.PinDrops.Contains(pin)).Distinct().ToList();
      // Phoenix...
      if (earliestDayEnemies.Count == 0) { earliestDayEnemies = enemiesThatDropThePin; }
      // If the earliest day is Another Day, then remove enemies who only drop this pin on Ultimate.
      var enemiesToRemove = new List<Enemy>();
      foreach (var enemy in earliestDayEnemies)
      {
        if (enemy.Data.IsPinDroppedOnlyOnDifficulty(pin, EnemyData.Difficulties.Ultimate)
              && earliestDayForThisPin < Days.DaysEnum.AnotherDay)
        {
          enemiesToRemove.Add(enemy);
        }
        else if ((enemy.Data.PinDrops[(int)EnemyData.Difficulties.Easy] == pin || enemy.Data.PinDrops[(int)EnemyData.Difficulties.Hard] == pin)
          && enemy.Data.PinDrops[(int)EnemyData.Difficulties.Normal] != pin
          && earliestDayForThisPin < Days.DaysEnum.W1D4)
        {
          enemiesToRemove.Add(enemy);
        }
        else if (enemy.Report.Name.Contains("Pig"))
        {
          enemiesToRemove.Add(enemy);
        }
      }
      enemiesToRemove.ForEach(enemyToRemove => earliestDayEnemies.Remove(enemyToRemove));
      return new Tuple<Days.DaysEnum, List<Enemy>>(earliestDayForThisPin, earliestDayEnemies);
    }
  }
}
