namespace PharmacySpa.Api.Models;

public sealed class PharmacyData
{
    public List<Medicine> Medicines { get; set; } = [];
    public List<SaleRecord> Sales { get; set; } = [];
}
