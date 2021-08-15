using System.Collections.Generic;
using System.IO;
namespace NEOTool.Text
{
  public class Speakers : Dictionary<string, string>
  {
    public Speakers(StreamReader reader)
    {
      var iteration = 0;
      var currentLine = reader.ReadLine();
      while (currentLine != null)
      {
        var key = currentLine.Split(',')[0].Replace(",", string.Empty);
        string value;
        value = currentLine.Contains(',') ? currentLine.Split(',')[1] : $"Character{iteration.ToString("D4")}";
        Add(key, value);
        iteration += 1;
        currentLine = reader.ReadLine();
      }
    }
  }
}
