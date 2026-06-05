using AbiturientDirectory.Contracts;
using AbiturientDirectory.Models;
using AbiturientDirectory.Services;
using Microsoft.AspNetCore.Mvc;

namespace AbiturientDirectory.Controllers;

/// <summary>REST-операції над спеціальностями та пошукові запити завдання.</summary>
[ApiController]
[Route("api/specialties")]
public class SpecialtiesController : ControllerBase
{
    private readonly DirectoryService _svc;

    /// <summary>Створює контролер поверх сервісу довідника.</summary>
    public SpecialtiesController(DirectoryService svc) => _svc = svc;

    /// <summary>Всі відомі назви спеціальностей (для випадного списку).</summary>
    [HttpGet("names")]
    public IActionResult Names() => Ok(_svc.GetSpecialtyNames());

    /// <summary>«Все щодо обраної спеціальності» + фільтр за максимальною оплатою.</summary>
    [HttpGet("offers")]
    public IActionResult Offers([FromQuery] string name, [FromQuery] decimal? maxPrice) =>
        Ok(_svc.GetOffers(name, maxPrice));

    /// <summary>Мінімальний конкурс зі спеціальності за формою навчання (404 — даних немає).</summary>
    [HttpGet("min-competition")]
    public IActionResult MinCompetition([FromQuery] string name, [FromQuery] StudyForm form)
    {
        var result = _svc.GetMinCompetition(name, form);
        return result is null ? NotFound(new { message = "За обраною формою навчання даних немає" }) : Ok(result);
    }

    /// <summary>Редагування спеціальності.</summary>
    [HttpPut("{id:guid}")]
    public IActionResult Update(Guid id, [FromBody] SpecialtyInput input)
    {
        try { return Ok(_svc.UpdateSpecialty(id, input)); }
        catch (KeyNotFoundException) { return NotFound(); }
    }

    /// <summary>Видалення спеціальності.</summary>
    [HttpDelete("{id:guid}")]
    public IActionResult Delete(Guid id)
    {
        try { _svc.DeleteSpecialty(id); return NoContent(); }
        catch (KeyNotFoundException) { return NotFound(); }
    }
}
