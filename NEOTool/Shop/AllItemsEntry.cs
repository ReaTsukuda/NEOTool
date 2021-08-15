using Newtonsoft.Json;
namespace NEOTool.Shop
{
  public enum ItemType
  {
    Pin = 0,
    Thread = 1,
    Food = 2,
    Fp = 3,
    Unused = 4,
    Book = 5,
    Cd = 6
  }
  
  public class AllItemsEntry
  {
    [JsonProperty("mId")]
    public int ItemId { get; init; }
    [JsonProperty("mSaveId")]
    private int SaveId { get; init; }
    [JsonProperty("mItemType")]
    public ItemType ItemType { get; init; }
    [JsonProperty("mName")]
    public string NameToken { get; init; }
    [JsonProperty("mInfo")]
    public string DescriptionToken { get; init; }

    public override string ToString() => ItemId.ToString();
  }
}
