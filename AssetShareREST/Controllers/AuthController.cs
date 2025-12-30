using AssetShareLib;
using AssetShareREST.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AssetShareREST.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserRepository _users;
        private readonly PasswordService _pw;
        private readonly JwtTokenService _jwt;

        public AuthController(UserRepository users, PasswordService pw, JwtTokenService jwt)
        {
            _users = users;
            _pw = pw;
            _jwt = jwt;
        }

        public record RegisterRequest(string Email, string Password, string FirstName, string LastName);
        public record LoginRequest(string Email, string Password);
        public record AuthResponse(string Token, DateTime ExpiresAt, int UserId, string Email, List<string> Roles);

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest req)
        {
            try
            {
                // validate email (library rules)
                new User { Email = req.Email }.ValidateEmail();

                // validate plain password (REST rules)
                PasswordHelper.ValidatePlainPassword(req.Password);

                var existing = await _users.GetByEmailAsync(req.Email);
                if (existing != null)
                    return Conflict("Email is already registered.");

                var user = new User
                {
                    Email = req.Email,
                    FirstName = req.FirstName,
                    LastName = req.LastName,
                    Roles = new List<string> { "normal" } // default
                };

                // hash password into PasswordHash
                user.PasswordHash = _pw.Hash(user, req.Password);

                // save via repo (repo assigns Id)
                var created = await _users.AddAsync(user);

                var (token, expiresAt) = _jwt.CreateToken(created);

                return Ok(new AuthResponse(token, expiresAt, created.Id, created.Email ?? "", created.Roles ?? new()));
            }
            catch (Exception ex) when (
                ex is ArgumentNullException ||
                ex is ArgumentOutOfRangeException ||
                ex is ArgumentException)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest req)
        {
            var user = await _users.GetByEmailAsync(req.Email);
            if (user == null)
                return Unauthorized("Invalid credentials.");

            if (!_pw.Verify(user, req.Password, user.PasswordHash))
                return Unauthorized("Invalid credentials.");

            var (token, expiresAt) = _jwt.CreateToken(user);

            return Ok(new AuthResponse(token, expiresAt, user.Id, user.Email ?? "", user.Roles ?? new()));
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<ActionResult<object>> Me()
        {
            var idStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(idStr, out var userId))
                return Unauthorized();

            var user = await _users.GetByIdAsync(userId);
            if (user == null)
                return Unauthorized();

            return Ok(new
            {
                user.Id,
                user.Email,
                user.FirstName,
                user.LastName,
                Roles = user.Roles ?? new List<string>()
            });
        }

        [HttpPost("logout")]
        public IActionResult Logout() => NoContent();
    }
}
