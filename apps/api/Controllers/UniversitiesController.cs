using AbiturientDirectory.Contracts;
using AbiturientDirectory.Services;
using Microsoft.AspNetCore.Mvc;

namespace AbiturientDirectory.Controllers;

[ApiController]
[Route("api/universities")]
public class UniversitiesController : ControllerBase
{
    private readonly DirectoryService _svc;

    public UniversitiesController(DirectoryService svc) => _svc = svc;

    [HttpGet]
    public IActionResult Search([FromQuery] string? query) => Ok(_svc.SearchUniversities(query));

    [HttpGet("{id:guid}")]
    public IActionResult Get(Guid id) =>
        Ok(new UniversityDetailsResponse(_svc.GetUniversity(id), _svc.GetUniversitySpecialties(id)));

    [HttpPost]
    public IActionResult Add([FromBody] UniversityInput input) => Ok(_svc.AddUniversity(input));

    [HttpPut("{id:guid}")]
    public IActionResult Update(Guid id, [FromBody] UniversityInput input) =>
        Ok(_svc.UpdateUniversity(id, input));

    [HttpDelete("{id:guid}")]
    public IActionResult Delete(Guid id) =>
        Ok(new DeleteUniversityResponse(_svc.DeleteUniversity(id)));

    [HttpPost("{id:guid}/specialties")]
    public IActionResult AddSpecialty(Guid id, [FromBody] SpecialtyInput input) =>
        Ok(_svc.AddSpecialty(id, input));
}
