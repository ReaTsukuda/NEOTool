using System;
using Newtonsoft.Json;
namespace NEOTool.Event
{
  public class ScenarioReward
  {
    [JsonProperty("mId")]
    public int Id { get; init; }
    public int Week
    {
      get
      {
        var rawWeek = Id / (int)Math.Pow(10, 5) % 10;
        if (rawWeek > 4) { return 4; }
        return rawWeek;
      }
    }
    public int Day => Id / (int)Math.Pow(10, 4) % 10;
    public int DayId
    {
      get
      {
        if (Week <= 3) { return 7 * (Week - 1) + Day; }
        return 24; // Another Day
      }
    }
    [JsonProperty("mReward1st")]
    public int FirstRewardItemId { get; init; }
    [JsonProperty("mReward1stCount")]
    public int FirstRewardCount { get; init; }
    [JsonProperty("mReward2nd")]
    public int RepeatRewardItemId { get; init; }
    [JsonProperty("mReward2ndCount")]
    public int RepeatRewardCount { get; init; }
    [JsonProperty("mSaveIndex")]
    private int SaveIndex { get; init; }

    public override string ToString() => $"ID {Id} ({Days.Strings[DayId]})";
  }
}
