using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
namespace NEOTool.Shop
{
  public class ShopGood
  {
    [JsonProperty("mId")]
    public int Id { get; init; }
    public string Name { get; set; }
    public string Description { get; set; }
    [JsonProperty("mShop")]
    public int ShopId { get; init; }
    [JsonProperty("mItem")]
    public int ItemId { get; init; }
    [JsonProperty("mItemCount")]
    public ShopGoodItemCount ShopGoodItemCount { get; init; }
    [JsonProperty("mPrice")]
    public int Price { get; init; }
    [JsonProperty("mExchange")]
    private int ExchangeId { get; init; }
    public Exchange Exchange { get; private set; }
    [JsonProperty("mReleaseVip")]
    public int RequiredVipLevel { get; init; }
    [JsonProperty("mReleaseDay")]
    public int ReleaseDay { get; init; }
    public string ReleaseDayString => Days.Strings[ReleaseDay];
    [JsonProperty("mReleaseSkill")]
    public bool UnlockedThroughSocialNetwork { get; init; }
    [JsonProperty("mSortIndex")]
    private int SortIndex { get; init; }
    [JsonProperty("mSaveIndex")]
    private int SaveIndex { get; init; }

    public void PostInit(List<Exchange> exchanges)
    {
      if (ExchangeId != -1)
      {
        Exchange = exchanges[ExchangeId];
      }
    }

    public override string ToString() => $"{Name} (Releases on {ReleaseDayString})";
  }

  public class ShopGoodItemCount : List<int>
  {
    public int Standard => this[0];
    public int VIP => this[1];
    public int Regular => this[2];
    public int Both => this[3];
  }
}
