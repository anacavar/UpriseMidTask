using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace UpriseMidTask.Models
{
    public class ProductionData
    {
        [Key]
        public int Id { get; set; }
        public string Uuid { get; set; } = Guid.NewGuid().ToString();
        public DateTime? DeletedAt { get; set; }
        public int SolarPowerPlantId { get; set; } // Foreign key to SolarPowerPlant
        public DateTime Timestamp { get; set; }
        public decimal Production { get; set; }
        public bool IsForecast { get; set; } // true or false

        [JsonIgnore]
        public SolarPlant? SolarPlant { get; set; } // Reference back to the parent SolarPowerPlant

    }
}
