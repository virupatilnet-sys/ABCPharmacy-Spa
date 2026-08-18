using PharmacySpa.Api.DTOs;
using PharmacySpa.Api.Models;

namespace PharmacySpa.Api.Services;

public sealed class MedicineService(MedicineRepository repository)
{
    public async Task<IEnumerable<Medicine>> ListAsync(string? search, CancellationToken ct) =>
        (await repository.ReadAsync(ct)).Medicines
            .Where(m => string.IsNullOrWhiteSpace(search) || m.FullName.Contains(search, StringComparison.OrdinalIgnoreCase))
            .OrderBy(m => m.FullName).ToList();

    public async Task<Medicine?> GetAsync(Guid id, CancellationToken ct) =>
        (await repository.ReadAsync(ct)).Medicines.SingleOrDefault(m => m.Id == id);

    public Task<Medicine> CreateAsync(UpsertMedicineRequest request, CancellationToken ct) => repository.MutateAsync(data =>
    {
        var medicine = FromRequest(request, new Medicine());
        data.Medicines.Add(medicine);
        return medicine;
    }, ct);

    public Task<Medicine?> UpdateAsync(Guid id, UpsertMedicineRequest request, CancellationToken ct) => repository.MutateAsync(data =>
    {
        var medicine = data.Medicines.SingleOrDefault(m => m.Id == id);
        return medicine is null ? null : FromRequest(request, medicine);
    }, ct);

    public Task<bool> DeleteAsync(Guid id, CancellationToken ct) => repository.MutateAsync(data =>
    {
        var medicine = data.Medicines.SingleOrDefault(m => m.Id == id);
        if (medicine is null) return false;
        data.Medicines.Remove(medicine);
        return true;
    }, ct);

    public Task<SaleRecord?> SellAsync(CreateSaleRequest request, CancellationToken ct) => repository.MutateAsync(data =>
    {
        var medicine = data.Medicines.SingleOrDefault(m => m.Id == request.MedicineId);
        if (medicine is null || medicine.Quantity < request.Quantity) return null;
        medicine.Quantity -= request.Quantity;
        var sale = new SaleRecord { MedicineId = medicine.Id, MedicineName = medicine.FullName, Quantity = request.Quantity, UnitPrice = medicine.Price };
        data.Sales.Add(sale);
        return sale;
    }, ct);

    public async Task<IEnumerable<SaleRecord>> SalesAsync(CancellationToken ct) =>
        (await repository.ReadAsync(ct)).Sales.OrderByDescending(s => s.SoldAtUtc).ToList();

    private static Medicine FromRequest(UpsertMedicineRequest r, Medicine m)
    {
        m.FullName = r.FullName.Trim(); m.Notes = r.Notes.Trim(); m.ExpiryDate = r.ExpiryDate;
        m.Quantity = r.Quantity; m.Price = decimal.Round(r.Price, 2); m.Brand = r.Brand.Trim();
        return m;
    }
}
