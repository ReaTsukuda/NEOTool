using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
namespace NEOTool.Pins
{
  public class PinGrowthTable : List<List<int>>
  {
    public PinGrowthTable(string masterDataLocation)
    {
      var growthArray = (JsonConvert.DeserializeObject(File.ReadAllText(Path.Combine(masterDataLocation, "BadgeLvUpTable.txt"))) as JObject)
        .GetValue("mTarget");
      foreach (JObject entry in growthArray)
      {
        Add(entry.GetValue("mExp").ToObject(typeof(List<int>)) as List<int>);
      }
    }
  }
}
