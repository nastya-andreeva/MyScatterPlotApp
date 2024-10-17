public class ChartData
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public string XValues { get; set; }
    public string YValues { get; set; }
    public string ChartImagePath { get; set; }
    public DateTime CreatedAt { get; set; }

    // Связь с пользователем
    public virtual ApplicationUser ApplicationUser { get; set; }
}
