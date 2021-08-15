using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Processors.Transforms;
namespace NEOTool.Portraits
{
  public static class PortraitStitching
  {
    public static void StitchCharacter(string path)
    {
      var attachPoints = GetAttachPoints(Path.Combine(path, "AttachPoints.csv"));
      var attachOverrides = GetAttachPoints(Path.Combine(path, "AttachOverrides.csv"));
      var cropPoints = GetCropPoints(Path.Combine(path, "CropPoints.csv"));
      
      var rawImages = Directory.GetFiles(Path.Combine(path, "Raw"));
      // Stitching.
      foreach (var entry in attachPoints)
      {
        var posePrefix = entry.Key;
        var x = entry.Value.Item1;
        var y = entry.Value.Item2;
        string basePoseFilename = Path.Combine(path, $"Raw/{posePrefix}_f01.png");
        var basePose = Image.Load(basePoseFilename);
        foreach (var matchingFace in rawImages.Where
        (rawImagePath => Path.GetFileName(rawImagePath).StartsWith(posePrefix)
           && rawImagePath.Contains("f01") == false))
        {
          var face = Image.Load(matchingFace);
          using var portrait = new Image<Rgba32>(basePose.Width, basePose.Height);
          if (attachOverrides != null && attachOverrides.ContainsKey(Path.GetFileNameWithoutExtension(matchingFace)))
          {
            var key = Path.GetFileNameWithoutExtension(matchingFace);
            portrait.Mutate(port => port
                              .DrawImage(basePose, new Point(0, 0), 1f)
                              .DrawImage(face, new Point(attachOverrides[key].Item1, attachOverrides[key].Item2), 1f));
          }
          else
          {
            portrait.Mutate(port => port
                              .DrawImage(basePose, new Point(0, 0), 1f)
                              .DrawImage(face, new Point(x, y), 1f));
          }
          var finalFilename = Path.GetFileName(matchingFace);
          if (Directory.Exists(Path.Combine(path, "StitchTemp")) == false) { Directory.CreateDirectory(Path.Combine(path, "StitchTemp")); }
          portrait.Save(Path.Combine(path, "StitchTemp", finalFilename));
        }
      }
      // Cropping.
      foreach (var cropEntry in cropPoints)
      {
        string basePoseFilename = Path.Combine(path, $"Raw/{cropEntry.Key}_f01.png");
        var basePose = Image.Load(basePoseFilename);
        var cropRectangle = new Rectangle(cropEntry.Value.Item1, cropEntry.Value.Item2, 267, 267);
        // If the initial pose has a face, we need to process it.
        if (cropEntry.Value.Item1 != 0 && cropEntry.Value.Item2 != 0)
        {
          if (cropEntry.Value.Item3 == false)
          {
            basePose.Mutate(port => port.Crop(cropRectangle));
            basePose.Mutate(port => port.Resize((int)(basePose.Width * 0.3), (int)(basePose.Width * 0.3), new TriangleResampler()));
            basePose.Mutate(port => port.Flip(FlipMode.Horizontal));
            if (Directory.Exists(Path.Combine(path, "Result")) == false) { Directory.CreateDirectory(Path.Combine(path, "Result")); }
            basePose.Save(Path.Combine(path, "Result", Path.GetFileName(basePoseFilename)));
          }
          foreach (var stitchedImage in Directory.GetFiles(Path.Combine(path, "StitchTemp")).Where(filename => filename.Contains(cropEntry.Key) && filename.Contains("L01") == false && filename.Contains("R01") == false))
          {
            using var portraitStream = new FileStream(stitchedImage, FileMode.Open);
            using var portrait = Image.Load(portraitStream);
            portrait.Mutate(port => port.Crop(cropRectangle));
            portrait.Mutate(port => port.Resize((int)(portrait.Width * 0.3), (int)(portrait.Width * 0.3), new TriangleResampler()));
            portrait.Mutate(port => port.Flip(FlipMode.Horizontal));
            if (Directory.Exists(Path.Combine(path, "Result")) == false) { Directory.CreateDirectory(Path.Combine(path, "Result")); }
            portrait.Save(Path.Combine(path, "Result", Path.GetFileName(stitchedImage)));
          }
        }
      }
      // Delete temp files.
      foreach (var tempFilename in Directory.GetFiles(Path.Combine(path, "StitchTemp")))
      {
        File.Delete(tempFilename);
      }
      Directory.Delete(Path.Combine(path, "StitchTemp"));
    }

    private static Dictionary<string, Tuple<int, int, bool>> GetCropPoints(string csvPath)
    {
      var result = new Dictionary<string, Tuple<int, int, bool>>();
      using var reader = new StreamReader(new FileStream(csvPath, FileMode.Open));
      string currentLine = reader.ReadLine();
      while (currentLine != null)
      {
        var posePrefix = currentLine.Split(',')[0];
        var x = int.Parse(currentLine.Split(',')[1]);
        var y = int.Parse(currentLine.Split(',')[2]);
        var poseHasMissingFace = bool.Parse(currentLine.Split(',')[3]);
        result.Add(posePrefix, new Tuple<int, int, bool>(x, y, poseHasMissingFace));
        currentLine = reader.ReadLine();
      }
      return result;
    }

    private static Dictionary<string, Tuple<int, int>> GetAttachPoints(string csvPath)
    {
      if (File.Exists(csvPath) == false) { return null; }
      var result = new Dictionary<string, Tuple<int, int>>();
      using var reader = new StreamReader(new FileStream(csvPath, FileMode.Open));
      string currentLine = reader.ReadLine();
      while (currentLine != null)
      {
        var posePrefix = currentLine.Split(',')[0];
        var x = int.Parse(currentLine.Split(',')[1]);
        var y = int.Parse(currentLine.Split(',')[2]);
        result.Add(posePrefix, new Tuple<int, int>(x, y));
        currentLine = reader.ReadLine();
      }
      return result;
    }
  }
}
