using AbiturientDirectory.Contracts;
using AbiturientDirectory.Models;
using AbiturientDirectory.Services;
using Microsoft.AspNetCore.Mvc;

namespace AbiturientDirectory.Controllers;

[ApiController]
[Route("api/specialties")]
public class SpecialtiesController : ControllerBase
{
    private readonly DirectoryService _svc;

    public SpecialtiesController(DirectoryService svc) => _svc = svc;

    [HttpGet("names")]
    public IActionResult Names() => Ok(_svc.GetSpecialtyNames());

    [HttpGet("offers")]
    public IActionResult Offers([FromQuery] string name, [FromQuery] decimal? maxPrice) =>
        Ok(_svc.GetOffers(name, maxPrice));

    [HttpGet("min-competition")]
    public IActionResult MinCompetition([FromQuery] string name, [FromQuery] StudyForm form) =>
        Ok(_svc.GetMinCompetition(name, form));

    [HttpPut("{id:guid}")]
    public IActionResult Update(Guid id, [FromBody] SpecialtyInput input) =>
        Ok(_svc.UpdateSpecialty(id, input));

    [HttpDelete("{id:guid}")]
    public IActionResult Delete(Guid id)
    {
        _svc.DeleteSpecialty(id);
        return NoContent();
    }
}
