using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
namespace NEOTool.Text
{
  public class GameText : Dictionary<string, TextEntry>
  {
    enum Languages
    {
      Japanese,
      English,
      Spanish,
      French,
      German,
      Italian
    }
    
    public GameText(string? jpLocation, string enLocation, string? spLocation, string? frLocation, string? deLocation, string? itLocation)
    {
      if (jpLocation != null) { doLanguage(jpLocation, Languages.Japanese); }
      doLanguage(enLocation, Languages.English);
      if (spLocation != null) { doLanguage(spLocation, Languages.Spanish); }
      if (frLocation != null) { doLanguage(frLocation, Languages.French); }
      if (deLocation != null) { doLanguage(deLocation, Languages.German); }
      if (itLocation != null) { doLanguage(itLocation, Languages.Italian); }
    }

    private void doLanguage(string location, Languages lang)
    {
      foreach (var path in Directory.GetFiles(location))
      {
        var textFileContents = (JsonConvert.DeserializeObject(File.ReadAllText(path)) as JObject)
          .GetValue("columns") as JArray;
        foreach (JObject entry in textFileContents)
        {
          var entryName = entry.GetValue("name").ToString();
          if (ContainsKey(entryName) == false)
          {
            Add(entryName, new TextEntry());
          }
          this[entryName].Speaker = entry.GetValue("speaker").ToString();
          this[entryName].Listener = entry.GetValue("listener").ToString();
          switch (lang)
          {
            case Languages.Japanese:
              this[entryName].Comment = entry.GetValue("comment").ToString()
                .Replace("\n", string.Empty)
                .Replace("\\n", string.Empty);
              this[entryName].Annotation = entry.GetValue("annotation").ToString();
              this[entryName].Japanese = entry.GetValue("content").ToString()
                .Replace("\n", string.Empty)
                .Replace("\\n", string.Empty);
              break;
            case Languages.English:
              this[entryName].English = entry.GetValue("content").ToString()
                .Replace("<MK_11>", "\"")
                .Replace("</MK_11>", "\"")
                .Replace("<NBSP>", " ")
                .Replace("<BR>", " ")
                .Replace("<CRE>", "<b>")
                .Replace("</C>", "</b>")
                .Replace("<size=74%>", string.Empty)
                .Replace("</size>", string.Empty);
              break;
            case Languages.Spanish:
              this[entryName].Spanish = entry.GetValue("content").ToString();
              break;
            case Languages.French:
              this[entryName].French = entry.GetValue("content").ToString();
              break;
            case Languages.German:
              this[entryName].German = entry.GetValue("content").ToString();
              break;
            case Languages.Italian:
              this[entryName].Italian = entry.GetValue("content").ToString();
              break;
          }
        }
      }
    }
  }

  public class TextEntry
  {
    public string Speaker { get; set; }
    public string FriendlySpeaker { get; set; }
    public string Listener { get; set; }
    public string Comment { get; set; }
    public string Annotation { get; set; }
    public string Japanese { get; set; }
    public string English { get; set; }
    public string Spanish { get; set; }
    public string French { get; set; }
    public string German { get; set; }
    public string Italian { get; set; }

    public override string ToString() => English;
  }
}
