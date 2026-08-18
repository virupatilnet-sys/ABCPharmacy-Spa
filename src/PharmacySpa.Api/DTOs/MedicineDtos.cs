using System.ComponentModel.DataAnnotations;

namespace PharmacySpa.Api.DTOs;

public sealed class UpsertMedicineRequest
{
    [Required, StringLength(150)] public string FullName { get; init; } = string.Empty;
    [StringLength(1000)] public string Notes { get; init; } = string.Empty;
    public DateOnly ExpiryDate { get; init; }
    [Range(0, int.MaxValue)] public int Quantity { get; init; }
    [Range(typeof(decimal), "0", "999999.99")] public decimal Price { get; init; }
    [Required, StringLength(100)] public string Brand { get; init; } = string.Empty;
}

public sealed class CreateSaleRequest
{
    [Required] public Guid MedicineId { get; init; }
    [Range(1, int.MaxValue)] public int Quantity { get; init; }
}
