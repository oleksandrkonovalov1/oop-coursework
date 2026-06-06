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

    /// <summary>Мінімальний конкурс зі спеціальності за формою навчання (200 з тілом null — даних немає).</summary>
    [HttpGet("min-competition")]
    public IActionResult MinCompetition([FromQuery] string name, [FromQuery] StudyForm form) =>
        Ok(_svc.GetMinCompetition(name, form));

    /// <summary>Редагування спеціальності.</summary>
    [HttpPut("{id:guid}")]
    public IActionResult Update(Guid id, [FromBody] SpecialtyInput input) =>
        Ok(_svc.UpdateSpecialty(id, input));

    /// <summary>Видалення спеціальності.</summary>
    [HttpDelete("{id:guid}")]
    public IActionResult Delete(Guid id)
    {
        _svc.DeleteSpecialty(id);
        return NoContent();
    }
}
