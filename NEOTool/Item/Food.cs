using System.Collections.Generic;
using System.Linq;
using NEOTool.Shop;
using NEOTool.Text;
using Newtonsoft.Json;
namespace NEOTool.Item
{
  public class Food
  {
    [JsonProperty("mId")]
    public int Id { get; init; }
    [JsonProperty("mItemId")]
    public int ItemId { get; init; }
    [JsonProperty("mStomach")]
    public int Calories { get; init; }
    [JsonProperty("mHp")]
    public int Hp { get; init; }
    [JsonProperty("mAttack")]
    public int Atk { get; init; }
    [JsonProperty("mDefence")]
    public int Def { get; init; }
    [JsonProperty("mSense")]
    public int Style { get; init; }
    [JsonProperty("mTaste")]
    public PreferencesContainer Preferences { get; init; }
    [JsonProperty("mSortIndex")]
    private int SortIndex { get; init; }
    [JsonProperty("mUiSprite")]
    public string SpriteName { get; init; }
    public string Name { get; private set; }

    public override string ToString() => Name;

    public void PostInit(GameText gameText, List<AllItemsEntry> allItems)
    {
      var nameToken = allItems.First(entry => entry.ItemId == ItemId).NameToken;
      Name = gameText[nameToken].English;
    }

    public class PreferencesContainer : List<int>
    {
      public enum PreferenceValues
      {
        Neutral = 0,
        Hate = 1,
        Like = 2,
        Love = 3
      }

      public PreferenceValues Rindo => (PreferenceValues)this[0];
      public PreferenceValues Shoka => (PreferenceValues)this[1];
      public PreferenceValues Fret => (PreferenceValues)this[2];
      public PreferenceValues Nagi => (PreferenceValues)this[3];
      public PreferenceValues Beat => (PreferenceValues)this[4];
      public PreferenceValues Neku => (PreferenceValues)this[5];
      public PreferenceValues Minamimoto => (PreferenceValues)this[6];
    }
  }
}