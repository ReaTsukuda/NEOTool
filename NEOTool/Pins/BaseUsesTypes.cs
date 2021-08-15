using System.Collections.Generic;
using System.IO;
namespace NEOTool.Pins
{
  public class BaseUsesTypes : Dictionary<string, int>
  {
    public BaseUsesTypes(string csvLocation)
    {
      var lines = File.ReadAllLines(csvLocation);
      foreach (var line in lines)
      {
        var splitLine = line.Split(',');
        var psychName = splitLine[0];
        var type = int.Parse(splitLine[1]);
        Add(psychName, type);
      }
    }
  }
}
