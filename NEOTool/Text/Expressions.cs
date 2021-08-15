using System.Collections.Generic;
using System.IO;
using System.Linq;
namespace NEOTool.Text
{
  public class Expressions
  {
    public Dictionary<string, Dictionary<long, string>> Poses { get; } = new();
    public Dictionary<string, Dictionary<string, Dictionary<long, string>>> Faces { get; } = new();

    public Expressions(StreamReader reader)
    {
      var currentLine = reader.ReadLine();
      while (currentLine != null)
      {
        var character = currentLine.Split(',')[0];
        var pose = currentLine.Split(',')[1];
        if (pose.Contains('x')) { pose = pose.Replace("x", string.Empty); }
        var face = currentLine.Split(',')[2];
        // If this pose is an x pose, then we need to check its file ID, and associate it with the standard pose. If it's a face,
        // hen we need to do the same, but, well, with the face.
        long pathId = long.Parse(currentLine.Contains("xp") ? currentLine.Split(',')[4] : currentLine.Split(',')[3]);
        if (face == "f01")
        {
          if (Poses.ContainsKey(character) == false)
          {
            Poses.Add(character, new Dictionary<long, string>());
          }
          Poses[character][pathId] = pose;
        }
        if (Faces.ContainsKey(character) == false)
        {
          Faces.Add(character, new Dictionary<string, Dictionary<long, string>>());
        }
        if (Faces[character].ContainsKey(pose) == false)
        {
          Faces[character].Add(pose, new Dictionary<long, string>());
        }
        Faces[character][pose].Add(pathId, face);
        currentLine = reader.ReadLine();
      }
    }
  }
}
