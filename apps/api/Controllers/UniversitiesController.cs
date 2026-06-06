using AbiturientDirectory.Contracts;
using AbiturientDirectory.Services;
using Microsoft.AspNetCore.Mvc;

namespace AbiturientDirectory.Controllers;

/// <summary>REST-операції над колекцією вузів.</summary>
[ApiController]
[Route("api/universities")]
public class UniversitiesController : ControllerBase
{
    private readonly DirectoryService _svc;

    /// <summary>Створює контролер поверх сервісу довідника.</summary>
    public UniversitiesController(DirectoryService svc) => _svc = svc;

    /// <summary>Список вузів з фільтром за підрядком назви/адреси.</summary>
    [HttpGet]
    public IActionResult Search([FromQuery] string? query) => Ok(_svc.SearchUniversities(query));

    /// <summary>«Все щодо обраного вузу»: вуз + його спеціальності.</summary>
    [HttpGet("{id:guid}")]
    public IActionResult Get(Guid id) =>
        Ok(new UniversityDetailsResponse(_svc.GetUniversity(id), _svc.GetUniversitySpecialties(id)));

    /// <summary>Додавання вузу.</summary>
    [HttpPost]
    public IActionResult Add([FromBody] UniversityInput input) => Ok(_svc.AddUniversity(input));

    /// <summary>Редагування вузу.</summary>
    [HttpPut("{id:guid}")]
    public IActionResult Update(Guid id, [FromBody] UniversityInput input) =>
        Ok(_svc.UpdateUniversity(id, input));

    /// <summary>Видалення вузу разом з його спеціальностями. Повертає кількість видалених спеціальностей.</summary>
    [HttpDelete("{id:guid}")]
    public IActionResult Delete(Guid id) =>
        Ok(new DeleteUniversityResponse(_svc.DeleteUniversity(id)));

    /// <summary>Додавання спеціальності до вузу.</summary>
    [HttpPost("{id:guid}/specialties")]
    public IActionResult AddSpecialty(Guid id, [FromBody] SpecialtyInput input) =>
        Ok(_svc.AddSpecialty(id, input));
}
