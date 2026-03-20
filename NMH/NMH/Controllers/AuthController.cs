using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NMH.Shared.DTOs;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace NMH.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public AuthController(UserManager<IdentityUser> userManager,
                              SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // 🔹 REGISTER
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var user = new IdentityUser
            {
                UserName = dto.UserName,
                Email = dto.Email
            };

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            // 🔥 Connexion directe après inscription (cookie)
            await _signInManager.SignInAsync(user, isPersistent: false);

            return Ok();
        }

        // 🔹 LOGIN (JWT)
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var user = await _userManager.FindByNameAsync(dto.UserName);
            if (user == null)
                return Unauthorized("Nom d'utilisateur incorrect");

            var result = await _signInManager.PasswordSignInAsync(
                user,
                dto.Password,
                isPersistent: false,
                lockoutOnFailure: false);

            if (!result.Succeeded)
                return Unauthorized("Mot de passe incorrect");

            // 🔥 Génération du JWT
            var jwtSettings = HttpContext.RequestServices
                .GetRequiredService<IConfiguration>()
                .GetSection("Jwt");

            var key = jwtSettings["Key"];
            var issuer = jwtSettings["Issuer"];
            var audience = jwtSettings["Audience"];
            var expireHours = int.Parse(jwtSettings["ExpireHours"]);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName),
                new Claim(JwtRegisteredClaimNames.Sub, user.Id)
            };

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer,
                audience,
                claims,
                expires: DateTime.UtcNow.AddHours(expireHours), // ✅ utilise appsettings
                signingCredentials: credentials
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(new
            {
                token = tokenString
            });
        }

        // 🔹 LOGIN FORM (AJOUTÉ POUR BLAZOR SERVER FORM HTML)
        [HttpPost("login-form")]
        public async Task<IActionResult> LoginForm([FromForm] LoginDto dto)
        {
            var user = await _userManager.FindByNameAsync(dto.UserName);

            if (user == null)
                return Redirect("/login");

            var result = await _signInManager.PasswordSignInAsync(
                user,
                dto.Password,
                isPersistent: false,
                lockoutOnFailure: false);

            if (!result.Succeeded)
                return Redirect("/login");

            return Redirect("/user/home");
        }

        // 🔹 LOGOUT
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok();
        }
    }
}