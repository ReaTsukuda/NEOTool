using System;
using System.Collections.Generic;
using System.Linq;
using NEOTool.Text;
using Newtonsoft.Json;
namespace NEOTool.Ability
{
  public class Ability
  {
    [JsonProperty("mId")]
    public int Id { get; init; }
    [JsonProperty("mName")]
    private string NameToken { get; init; }
    public string Name { get; private set; }
    [JsonProperty("mInfoText")]
    private string DescriptionToken { get; init; }
    public string GameDescription { get; private set; }
    [JsonProperty("mType")]
    public int AbilityType { get; init; }
    [JsonProperty("mTarget")]
    private int Target { get; init; }
    [JsonProperty("mCharaRestriction")]
    public int Character { get; init; }
    [JsonProperty("mSortIndex")]
    private int SortIndex { get; init; }
    [JsonProperty("mParam")]
    public List<decimal> Values { get; init; }
    [JsonProperty("mBrandParam")]
    public int Brand { get; init; }
    [JsonProperty("mElement")]
    public int Element { get; init; }
    public string Description { get; private set; }

    private static readonly Tuple<string, string, string>[] CharacterNamesAndPronouns = new[]
    {
      new Tuple<string, string, string>("Rindo", "his", "he"),
      new Tuple<string, string, string>("Shoka", "her", "she"),
      new Tuple<string, string, string>("Fret", "his", "he"),
      new Tuple<string, string, string>("Nagi", "her", "she"),
      new Tuple<string, string, string>("Beat", "his", "he"),
      new Tuple<string, string, string>("Neku", "his", "he"),
      new Tuple<string, string, string>("Minamimoto", "his", "he")
    };
    
    private static string[] BrandNames =
    {
      "Unbranded", "Top o' Topo", "Joli bécot", "Tigre PUNKS", "Cony×Cony", "RyuGu", "garagara", "IL CAVALLO DEL RE", "Shepherd House", "Jupiter of the Monkey", "MONOCROW", "NATURAL PUPPY", "HOG FANG", "Gatto Nero", "croaky panic", "BLACK HONEY CHILI COOKIE"
    };
    
    private static readonly Tuple<string, string>[] Inputs = new[]
    {
      new Tuple<string, string>("△", "X"),
      new Tuple<string, string>("□", "Y"),
      new Tuple<string, string>("L1", "L"),
      new Tuple<string, string>("R1", "R"),
      new Tuple<string, string>("L2", "ZL"),
      new Tuple<string, string>("R2", "ZR"),
    };

    private static readonly Dictionary<string, string> BrandsByShorthand = new()
    {
      { "TT", "Top o' Topo" },
      { "JB", "Joli bécot" },
      { "TP", "Tigre PUNKS" },
      { "CC", "Cony×Cony" },
      { "RG", "RyuGu" },
      { "GG", "garagara" },
      { "IL", "IL CAVALLO DEL RE" },
      { "SH", "Shepherd House" },
      { "JM", "Jupiter of the Monkey" },
      { "MC", "MONOCROW" },
      { "NP", "Natural Puppy" },
      { "HF", "HOG FANG" },
      { "GN", "Gatto Nero" },
      { "CP", "croaky panic"},
      { "BHCC", "BLACK HONEY CHILI COOKIE" }
    };
    
    private static readonly string[] Affinities = new[]
    {
      "fire",
      "ice",
      "electric",
      "wind",
      "water",
      "stone",
      "time",
      "sound",
      "darkness",
      "light",
      "kinesis",
      "burst",
      "gravity",
      "dummy",
      "dummy",
      "dummy",
      "dummy",
      "dummy",
      "dummy",
      "dummy",
      "dummy",
      "dummy",
      "poison"
    };
    
    private static readonly Dictionary<int, string> Disables = new()
    {
      { 1, "poisoned" },
      { 2, "burned" },
      { 8, "shocked" }
    };

    public void PostInit(GameText gameText, Dictionary<int, Dictionary<int, string>> descriptionFormats)
    {
      Name = gameText[NameToken].English;
      GameDescription = gameText[DescriptionToken].English;

      var abilityDescriptions = new Dictionary<int, string>();
      foreach (var entry in descriptionFormats[AbilityType])
      {
        abilityDescriptions.Add(entry.Key, entry.Value);
      }
      foreach (var key in abilityDescriptions.Keys)
      {
        var replacedDescription = abilityDescriptions[key]
          .Replace("{realValue0}", Values[0].ToString())
          .Replace("{realValue1}", Values[1].ToString())
          .Replace("{minus0}", Math.Abs(decimal.Round((Values[0] - 1) * 100)).ToString())
          .Replace("{minus1}", Math.Abs(decimal.Round((Values[1] - 1) * 100)).ToString());
        replacedDescription = (Values[0] * 100) % 1 == 0 
          ? replacedDescription.Replace("{value0}", decimal.Round(Values[0] * 100, 0).ToString()) 
          : replacedDescription.Replace("{value0}", decimal.Round(Values[0] * 100, 1).ToString());
        replacedDescription = (Values[1] * 100) % 1 == 0 
          ? replacedDescription.Replace("{value1}", decimal.Round(Values[1] * 100, 0).ToString()) 
          : replacedDescription.Replace("{value1}", decimal.Round(Values[1] * 100, 1).ToString());
        if (Character >= 1)
        {
          replacedDescription = replacedDescription
            .Replace("{character}", CharacterNamesAndPronouns[Character - 1].Item1)
            .Replace("{pronoun}", CharacterNamesAndPronouns[Character - 1].Item2)
            .Replace("{personalPronoun}", CharacterNamesAndPronouns[Character - 1].Item3);
        }
        else if (Brand >= 0)
        {
          replacedDescription = replacedDescription.Replace("{brand}", BrandNames[Brand]);
        }
        // I am going to strangle whoever at h.a.n.d. made me have to program Brand Ambassador descriptions this way.
        else if (Name.Contains("Brand Ambassador"))
        {
          var brandShorthand = Name.Split(": ")[1];
          replacedDescription = replacedDescription.Replace("{brand}", BrandsByShorthand[brandShorthand]);
        }
        else if (replacedDescription.Contains("{ps4Button}"))
        {
          replacedDescription = replacedDescription
            .Replace("{ps4Button}", Inputs[(int)Values[0] - 1].Item1)
            .Replace("{switchButton}", Inputs[(int)Values[0] - 1].Item2);
        }
        else if (Element != -1)
        {
          replacedDescription = replacedDescription
            .Replace("{affinity}", Affinities[Element - 1]);
        }
        else if (replacedDescription.Contains("{disable}"))
        {
          replacedDescription = replacedDescription
            .Replace("{disable}", Disables[(int)Values[0]]);
        }
        abilityDescriptions[key] = replacedDescription;
      }
      Description = Character >= 0 ? abilityDescriptions[1] : abilityDescriptions[0];
    }

    public override string ToString() => $"{Name}: {Description}";

    public bool IsBlank => NameToken == "Com_Blank";
  }
}
