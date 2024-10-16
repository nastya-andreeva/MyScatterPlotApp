using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyScatterPlotApp.Models
{
    public class ChartData
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public string XValues { get; set; } // Хранение значений X (например, JSON или CSV)

        [Required]
        public string YValues { get; set; } // Хранение значений Y

        [Required]
        public string ChartImagePath { get; set; } // Путь к изображению диаграммы

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }
    }
}
