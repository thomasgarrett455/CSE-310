using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace JournalApi.Controllers;

[ApiController]
[Route("api/hello")]
public class HelloController : ControllerBase
{
  [HttpGet]
  public IActionResult Get()
  {
    return Ok("Hello world from ASP.NET Core Web API!");
  }
}