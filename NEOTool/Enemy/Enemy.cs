namespace NEOTool.Enemy
{
  public class Enemy
  {
    public EnemyReport Report { get; init; }
    public EnemyData Data { get; init; }
    public override string ToString() => Report.Name;
  }
}
