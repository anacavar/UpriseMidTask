using System.ComponentModel.DataAnnotations;

namespace UpriseMidTask.Models
{
    public class SolarPlant
    {
        [Key]
        public int Id { get; set; }
        public string Uuid { get; set; } = Guid.NewGuid().ToString();
        public DateTime DeletedAt { get; set; }
        required public string Name { get; set; }
        public decimal PowerInstalled { get; set; }
        public DateTime DateInstalled { get; set; }
        
        [Range(-90, 90)]
        public double Latitude { get; set; }
        [Range(-180, 180)]
        public double Longitude { get; set; }

        public ICollection<ProductionData>? ProductionData { get; set; } // "many" side of the relationship
    }
}
