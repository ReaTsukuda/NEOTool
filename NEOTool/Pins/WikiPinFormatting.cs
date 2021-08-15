using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NEOTool.Text;
namespace NEOTool.Pins
{
  public static class WikiPinFormatting
  {
    private static readonly string[] Affinities = {
      "None",
      "Fire",
      "Ice",
      "Electric",
      "Wind",
      "Water",
      "Stone",
      "Time",
      "Sound",
      "Darkness",
      "Light",
      "Kinesis",
      "Burst",
      "Gravity",
      "dummy",
      "dummy",
      "dummy",
      "dummy",
      "dummy",
      "dummy",
      "dummy",
      "dummy",
      "dummy",
      "Poison"
    };
    
    public static List<string> GetRowsForBrand(int brandId, List<Pin> pins, GameText gameText, BaseUsesTypes baseUsesTypes, PinGrowthTable pinGrowth)
    {
      var result = new List<string>();
      var pinResult = new List<string>();
      var brandPins = pins.Where(pin => pin.BrandId == brandId);
      var anyPinHasEnsemble = brandPins.Any(pin => pin.EnsembleAbilityId != -1);
      string ensembleHeader = anyPinHasEnsemble ? "|| Ensemble" : string.Empty;
      result.Add($"==={Pin.BrandNames[brandId]}===");
      result.Add("{| {{table}}\n|- style=\"background: #F2F2F2;\"\n! Icon || # || Name || Affinity || Evolution || Psych || Beatdrop || Max Level || Command || Power || Limit || Reboot || PP to Max || Extra || Value || Uber || Sources");
      foreach (var pin in brandPins)
      {
        string baseUsesTypeString = string.Empty;
        if (pin.Psych != null)
        {
          var baseUsesType = baseUsesTypes[pin.PsychName];
          switch (baseUsesType)
          {
            case 0:
              var twoThirdsUses = Math.Floor(pin.BaseUses * 0.67m);
              if (twoThirdsUses == 0) { twoThirdsUses = 1; }
              baseUsesTypeString = $"{twoThirdsUses.ToString()}";
              break;
            case 1:
              baseUsesTypeString = "1";
              break;
            case 2:
              baseUsesTypeString = $"{pin.BaseUses - 1}s";
              break;
            case 3:
              var thirdUses = Math.Floor(pin.BaseUses * 0.33m);
              if (thirdUses == 0) { thirdUses = 1; }
              baseUsesTypeString = $"{thirdUses.ToString()}";
              break;
            case 4:
              baseUsesTypeString = $"{pin.BaseUses}+";
              break;
            case 5:
              if (pin.Name == "UFO Rescue"
                || pin.Name == "Runaway Rocket"
                || pin.Name == "HOG Healer")
              {
                baseUsesTypeString = $"{pin.BaseUses - 1}s";
              }
              else
              {
                baseUsesTypeString = $"{pin.BaseUses}";
              }
              break;
            case 6:
              var halfUses = Math.Floor(pin.BaseUses * 0.5m);
              if (halfUses == 0) { halfUses = 1; }
              baseUsesTypeString = $"{halfUses.ToString()}";
              break;
            case 7:
              baseUsesTypeString = $"{pin.BaseUses}s";
              break;
          }
        }
        pinResult.Add("|- style=\"text-align:center; vertical-align:middle;\"");
        pinResult.Add($"|[[File:NEO_Pin_{pin.CollectionId.ToString("D3")}_{pin.Name.Replace(" ", string.Empty).Replace("・", string.Empty).Replace("\'", string.Empty).Replace(",", string.Empty).Replace("!", string.Empty).Replace("-", string.Empty)}.png|50px]]");
        pinResult.Add($"|{pin.CollectionId.ToString("D3")}");
        pinResult.Add($"|[[{pin.Name}]]");
        if (pin.Psych != null)
        {
          pinResult.Add($"|[[File:{Affinities[pin.Psych.AttackComboSet.Attacks[0].AttackHits[0].Elements[0]]}.png|50px]]");
          if (pin.HasEvolution)
          {
            pinResult.Add($"|[[File:NEO_Pin_{pin.EvolutionTarget.CollectionId.ToString("D3")}_{pin.EvolutionTarget.Name.Replace(" ", string.Empty).Replace("・", string.Empty).Replace("\'", string.Empty).Replace(",", string.Empty).Replace("!", string.Empty).Replace("-", string.Empty)}.png|50px|link={pin.EvolutionTarget.Name}]]");
          }
          else if (pin.HasMutation)
          {
            pinResult.Add($"|[[File:NEO_Pin_{pin.Mutation.Target.CollectionId.ToString("D3")}_{pin.Mutation.Target.Name.Replace(" ", string.Empty).Replace("・", string.Empty).Replace("\'", string.Empty).Replace(",", string.Empty).Replace("!", string.Empty).Replace("-", string.Empty)}.png|50px|link={pin.Mutation.Target.Name}]] [[File:PinMutation{pin.Mutation.Character}.png|50px]]");
          }
          else
          {
            pinResult.Add("|");
          }
          pinResult.Add($"|{pin.PsychName}");
          pinResult.Add($"|{gameText[pin.BeatdropNameToken]}");
          pinResult.Add($"|{pin.MaxLevel}");
          pinResult.Add($"|{gameText[pin.Psych.ControlTypeToken]}");
          pinResult.Add($"|{pin.BaseAttack}");
          pinResult.Add($"|{baseUsesTypeString}");
          if (pin.BaseRebootTime == -1)
          {
           pinResult.Add("|"); 
          }
          else
          {
            pinResult.Add($"|{pin.BaseRebootTime}s");
          }
          if (pin.MaxLevel > 1)
          {
            pinResult.Add($"|{pin.PpToMax}");
          }
          else
          {
            pinResult.Add("|");
          }
          pinResult.Add($"|");
          if (pin.BaseSellPrice != 0)
          {
            pinResult.Add($"|¥{pin.BaseSellPrice}");
          }
          else
          {
            pinResult.Add("|");
          }
          var uberString = pin.Uber ? "✓" : string.Empty;
          pinResult.Add($"|{uberString}");
        }
        else
        {          
          pinResult.Add($"|");
          pinResult.Add($"|");
          pinResult.Add($"|");
          pinResult.Add($"|");
          pinResult.Add($"|");
          pinResult.Add($"|");
          pinResult.Add($"|");
          pinResult.Add($"|");
          pinResult.Add($"|");
          pinResult.Add("|");
          pinResult.Add($"|");
          if (pin.BaseSellPrice != 0)
          {
            pinResult.Add($"|¥{pin.BaseSellPrice}");
          }
          else
          {
            pinResult.Add("|");
          }
          pinResult.Add($"|");
        }
        pinResult.Add($"|");
        result.AddRange(pinResult);
        pinResult.Clear();
      }
      result.Add("|-");
      result.Add("|}");
      return result;
    }
  }
}
