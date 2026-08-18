using Microsoft.AspNetCore.Mvc;
using PharmacySpa.Api.DTOs;
using PharmacySpa.Api.Models;
using PharmacySpa.Api.Services;

namespace PharmacySpa.Api.Controllers;

[ApiController, Route("api/medicines")]
public sealed class MedicinesController(MedicineService service) : ControllerBase
{
    [HttpGet] public Task<IEnumerable<Medicine>> List([FromQuery] string? search, CancellationToken ct) => service.ListAsync(search, ct);
    [HttpGet("{id:guid}")] public async Task<ActionResult<Medicine>> Get(Guid id, CancellationToken ct) => (await service.GetAsync(id, ct)) is { } m ? Ok(m) : NotFound();
    [HttpPost] public async Task<ActionResult<Medicine>> Create(UpsertMedicineRequest request, CancellationToken ct) { var m = await service.CreateAsync(request, ct); return CreatedAtAction(nameof(Get), new { m.Id }, m); }
    [HttpPut("{id:guid}")] public async Task<ActionResult<Medicine>> Update(Guid id, UpsertMedicineRequest request, CancellationToken ct) => (await service.UpdateAsync(id, request, ct)) is { } m ? Ok(m) : NotFound();
    [HttpDelete("{id:guid}")] public async Task<IActionResult> Delete(Guid id, CancellationToken ct) => await service.DeleteAsync(id, ct) ? NoContent() : NotFound();
}
