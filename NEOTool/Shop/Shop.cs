using System.Collections.Generic;
using Newtonsoft.Json;
namespace NEOTool.Shop
{
  public class Shop
  {
    [JsonProperty("mId")]
    public int Id { get; init; }
    [JsonProperty("mName")]
    private string NameToken { get; init; }
    [JsonProperty("mShopCategory")]
    private string CategoryToken { get; init; }
    [JsonProperty("mShopType")]
    private int ShopType { get; init; }
    [JsonProperty("mBrand")]
    public int BrandId { get; init; }
    [JsonProperty("mSkill")]
    private int SkillId { get; init; }
    [JsonProperty("mBgPath")]
    private string BgPath { get; init; }
    [JsonProperty("mRegularBuy")]
    private List<int> RegularBuy { get; init; }
    public int PurchasesForTier1Regular => RegularBuy[0];
    public int PurchasesForTier2Regular => RegularBuy[1];
    [JsonProperty("mRegularDay")]
    private List<int> RegularDay { get; init; }
    public int Tier1RegularDay => RegularDay[0];
    public int Tier2RegularDay => RegularDay[1];
    [JsonProperty("mRegularVip")]
    public List<int> RegularVip { get; init; }
    [JsonProperty("mShoptalk")]
    private List<int> ShopTalkIds { get; init; }
  }
}
