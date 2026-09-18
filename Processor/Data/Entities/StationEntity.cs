using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Processor.Data.Entities;

public class StationEntity
{
    [Key]
    [MaxLength(100)]
    public string StationId { get; set; } = null!;

    [Required]
    [MaxLength(255)]
    public string Name { get; set; } = null!;

    [MaxLength(50)]
    public string? ShortName { get; set; }

    [Precision(10, 7)]
    public decimal Longitude { get; set; }

    [Precision(9, 7)]
    public decimal Latitude { get; set; }

    [MaxLength(50)]
    public string? RegionId { get; set; }
    public int Capacity { get; set; }

    [MaxLength(500)]
    public string? AndroidUri { get; set; }

    [MaxLength(500)]
    public string? IosUri { get; set; }

    [MaxLength(500)]
    public string? WebUri { get; set; }
}
