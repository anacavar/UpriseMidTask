namespace UpriseMidTask.DTOs
{
    public class AggregateProductionData
    {
        public DateTime Timestamp { get; set; }
        public decimal TotalProduction { get; set; }
        public decimal AverageProduction { get; set; }
    }
}
