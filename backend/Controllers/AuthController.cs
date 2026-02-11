using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using JournalApi.Models;
using JournalApi.Models.DTOs;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
  private readonly UserManager<ApplicationUser> _userManager;
  private readonly IConfiguration _config;

  public AuthController(UserManager<ApplicationUser> userManager, IConfiguration config)
  {
    _userManager = userManager;
    _config = config;
  }

  // REGISTER
  [HttpPost("register")]
  public async Task<IActionResult> Register(RegisterDto dto)
  {
    var user = new ApplicationUser
    {
      UserName = dto.Email,
      Email = dto.Email

    };

    var result = await _userManager.CreateAsync(user, dto.Password);

    if (!result.Succeeded)
      return BadRequest(result.Errors);
    
    return Ok(new { user.Id, user.Email });
  }

  // LOGIN
  [HttpPost("login")]
  public async Task<IActionResult> Login(LoginDto dto)
  {
    var user = await _userManager.FindByEmailAsync(dto.Email);

    if (user == null)
      return Unauthorized("Invalid credentials");
    
    var valid = await _userManager.CheckPasswordAsync(user, dto.Password);

    if (!valid)
      return Unauthorized("Invalid credentials");
    
    var token = GenerateToken(user);

    return Ok(new { token });
  }

  private string GenerateToken(ApplicationUser user)
  {
    var claims = new[]
    {
      new Claim(ClaimTypes.NameIdentifier, user.Id),
      new Claim(ClaimTypes.Email, user.Email!)
    };

    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));

    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    var token = new JwtSecurityToken(
      issuer: _config["Jwt:Issuer"],
      audience: _config["Jwt:Audience"],
      claims: claims,
      expires: DateTime.UtcNow.AddHours(
        int.Parse(_config["Jwt:ExpireHours"]!)),
      signingCredentials : creds);

    return new JwtSecurityTokenHandler().WriteToken(token);
  }
}