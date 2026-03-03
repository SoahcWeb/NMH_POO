using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NMH.Shared.DTOs;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace NMH.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IConfiguration _configuration; // 🔹 Injecter IConfiguration

        public AuthController(UserManager<IdentityUser> userManager,
                              SignInManager<IdentityUser> signInManager,
                              IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var user = new IdentityUser { UserName = dto.UserName, Email = dto.Email };
            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            // Connecte automatiquement après inscription
            await _signInManager.SignInAsync(user, isPersistent: false);

            return Ok("Utilisateur créé ✅");
        }

        [HttpPost("login")]
public async Task<IActionResult> Login([FromBody] LoginDto dto)
{
    var user = await _userManager.FindByNameAsync(dto.UserName);
    if (user == null) return Unauthorized("Nom d'utilisateur incorrect");

    var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, false);
    if (!result.Succeeded) return Unauthorized("Mot de passe incorrect");

    // 🔹 JWT depuis appsettings
    var jwtKey = _configuration["Jwt:Key"];
    var jwtIssuer = _configuration["Jwt:Issuer"];
    var jwtAudience = _configuration["Jwt:Audience"];
    var expireHours = int.Parse(_configuration["Jwt:ExpireHours"]);

    var tokenHandler = new JwtSecurityTokenHandler();
    var key = Encoding.UTF8.GetBytes(jwtKey);
    var tokenDescriptor = new SecurityTokenDescriptor
    {
        Subject = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, user.UserName) }),
        Expires = DateTime.UtcNow.AddHours(expireHours),
        Issuer = jwtIssuer,
        Audience = jwtAudience,
        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
    };

    var token = tokenHandler.CreateToken(tokenDescriptor);
    return Ok(new { token = tokenHandler.WriteToken(token) });
}

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok("Déconnecté ✅");
        }
    }
}