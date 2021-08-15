using System.Collections.Generic;
namespace NEOTool
{
  public class Days
  {
    public enum DaysEnum
    {
      GameStart = 0,
      W1D1 = 1,
      W1D2 = 2,
      W1D3 = 3,
      W1D4 = 4,
      W1D5 = 5,
      W1D6 = 6,
      W1D7 = 7,
      W2D1 = 8,
      W2D2 = 9,
      W2D3 = 10,
      W2D4 = 11,
      W2D5 = 12,
      W2D6 = 13,
      W2D7 = 14,
      W3D1 = 15,
      W3D2 = 16,
      W3D3 = 17,
      W3D4 = 18,
      W3D5 = 19,
      W3D6 = 20,
      W3D7 = 21,
      W3D7Part2 = 22,
      W3D7Part3 = 23,
      AnotherDay = 24,
      AnotherDayCleared = 25,
      Invalid = 100
    }
    public static readonly Dictionary<int, string> Strings = new()
    {
      { 0,  "Immediately" },
      { 1,  "W1D1" },
      { 2,  "W1D2" },
      { 3,  "W1D3" },
      { 4,  "W1D4" },
      { 5,  "W1D5" },
      { 6,  "W1D6" },
      { 7,  "W1D7" },
      { 8,  "W2D1" },
      { 9,  "W2D2" },
      { 10, "W2D3" },
      { 11, "W2D4" },
      { 12, "W2D5" },
      { 13, "W2D6" },
      { 14, "W2D7" },
      { 15, "W3D1" },
      { 16, "W3D2" },
      { 17, "W3D3" },
      { 18, "W3D4" },
      { 19, "W3D5" },
      { 20, "W3D6" },
      { 21, "W3D7" },
      { 22, "W3D7`" },
      { 23, "W3D7``" },
      { 24, "Another Day" },
      { 25, "Finish Another Day" },
      { 100, "Does Not Unlock Naturally" }
    };
  }
}
