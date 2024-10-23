using AplicacionNPG.Server.Data;
using AplicacionNPG.Server.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AplicacionNPG.Server.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public AuthController(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginModel login)
        {
            var usuario = await _context.usuario.FirstOrDefaultAsync(u => u.NombreUsuario == login.NombreUsuario);

            if (usuario == null || !BCrypt.Net.BCrypt.Verify(login.Contrasena, usuario.Contrasena))
                return Unauthorized("Usuario o contraseña incorrectos");

            var token = GenerateJwtToken(usuario);
            return Ok(new
            {
                access_token = token,
                user_type = usuario.TipoUsuario,
                name = usuario.Nombre,
                user_id = usuario.Id,
                register_id = usuario.RegistroId
            });
        }

        private string GenerateJwtToken(Usuario usuario)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("ppppppppppppppppppppppppppppppppppppp"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, usuario.Id.ToString())
            };
            var token = new JwtSecurityToken(
                issuer: "https://localhost:7207",
                audience: "https://localhost:7207",
                claims: claims,
                expires: DateTime.Now.AddHours(3),
                signingCredentials: creds
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [HttpPut("cambiar-contrasena/{id}")]
        public async Task<IActionResult> UpdateContrasena(int id, [FromBody] ChangePasswordModel model)
        {
            if (string.IsNullOrWhiteSpace(model.ContrasenaNueva))
                return BadRequest("La nueva contraseña no puede ser vacía");

            var usuario = await _context.usuario.FindAsync(id);

            if (usuario == null)
                return NotFound("Usuario no encontrado");

            if (!BCrypt.Net.BCrypt.Verify(model.ContrasenaActual, usuario.Contrasena))
                return BadRequest("La contraseña actual es incorrecta");

            usuario.Contrasena = BCrypt.Net.BCrypt.HashPassword(model.ContrasenaNueva);

            try
            {
                _context.usuario.Update(usuario);
                await _context.SaveChangesAsync();
                return Ok("Contraseña actualizada correctamente");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error al actualizar contraseña" + ex.Message);
            }
        }
    }
}
