using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using NEOTool.Pins;
namespace NEOTool.Shop
{
  public class Exchange
  {
    private List<Tuple<int, int>> PinsById { get; } = new();
    public List<Tuple<Pin, int>> Pins { get; } = new();
    
    [JsonConstructor]
    public Exchange(List<int> mItem, List<int> mItemCount)
    {
      for (int pinIndex = 0; pinIndex < mItem.Count; pinIndex += 1)
      {
        PinsById.Add(new Tuple<int, int>(mItem[pinIndex], mItemCount[pinIndex]));
      }
    }

    public void PostInit(List<Pin> pins)
    {
      foreach (var entry in PinsById)
      {
        Pins.Add(new Tuple<Pin, int>(pins.First(pin => pin.ItemId == entry.Item1), entry.Item2));
      }
    }
  }
}
