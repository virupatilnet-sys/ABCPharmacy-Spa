using Microsoft.AspNetCore.Mvc;
using PharmacySpa.Api.DTOs;
using PharmacySpa.Api.Models;
using PharmacySpa.Api.Services;

namespace PharmacySpa.Api.Controllers;

[ApiController, Route("api/sales")]
public sealed class SalesController(MedicineService service) : ControllerBase
{
    [HttpGet] public Task<IEnumerable<SaleRecord>> List(CancellationToken ct) => service.SalesAsync(ct);
    [HttpPost] public async Task<ActionResult<SaleRecord>> Create(CreateSaleRequest request, CancellationToken ct) =>
        (await service.SellAsync(request, ct)) is { } sale ? Ok(sale) : BadRequest(new { message = "Medicine was not found or there is insufficient stock." });
}
