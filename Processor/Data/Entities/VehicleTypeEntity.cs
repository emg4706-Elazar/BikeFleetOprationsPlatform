using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

using System.Diagnostics.Contracts;

namespace Processor.Data.Entities;

public class VehicleTypeEntity
{
    [Key]
    [MaxLength(100)]
    public string VehicleTypeId { get; set; } = null!;

    [Required]
    [MaxLength(30)]
    public string FormFactor { get; set; } = null!;

    [Required]
    [MaxLength(30)]
    public string PropulsionType { get; set; } = null!;

    [Precision(12, 2)]
    public decimal? MaxRangeMeters { get; set; }
}
