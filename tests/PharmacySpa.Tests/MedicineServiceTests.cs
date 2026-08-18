using PharmacySpa.Api.DTOs;
using PharmacySpa.Api.Models;
using PharmacySpa.Api.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;
using Xunit;

namespace PharmacySpa.Tests;

public sealed class MedicineServiceTests
{
    [Fact]
    public async Task Sale_reduces_stock_and_creates_record()
    {
        var service = CreateService(out var medicine);
        var sale = await service.SellAsync(new CreateSaleRequest { MedicineId = medicine.Id, Quantity = 3 }, default);
        Assert.NotNull(sale); Assert.Equal(7, (await service.GetAsync(medicine.Id, default))!.Quantity);
    }

    [Fact]
    public async Task Sale_with_insufficient_stock_is_rejected_without_change()
    {
        var service = CreateService(out var medicine);
        Assert.Null(await service.SellAsync(new CreateSaleRequest { MedicineId = medicine.Id, Quantity = 11 }, default));
        Assert.Equal(10, (await service.GetAsync(medicine.Id, default))!.Quantity);
    }

    private static MedicineService CreateService(out Medicine medicine)
    {
        var root = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString()); Directory.CreateDirectory(root);
        var environment = new FakeEnvironment { ContentRootPath = root, WebRootPath = root };
        var repository = new MedicineRepository(environment);
        medicine = repository.MutateAsync(data => { var m = new Medicine { FullName = "Test", Brand = "Brand", Quantity = 10, Price = 1, ExpiryDate = DateOnly.FromDateTime(DateTime.Today.AddYears(1)) }; data.Medicines.Add(m); return m; }).GetAwaiter().GetResult();
        return new MedicineService(repository);
    }

    private sealed class FakeEnvironment : IWebHostEnvironment
    {
        public string ApplicationName { get; set; } = "Tests"; public IFileProvider WebRootFileProvider { get; set; } = null!; public string WebRootPath { get; set; } = ""; public string EnvironmentName { get; set; } = "Test"; public string ContentRootPath { get; set; } = ""; public IFileProvider ContentRootFileProvider { get; set; } = null!;
    }
}
