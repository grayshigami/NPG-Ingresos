using AplicacionNPG.Server.Data;
using AplicacionNPG.Server.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace AplicacionNPG.Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("/api/[controller]")]
    public class UsuarioController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsuarioController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Usuario>>> GetUsuarios()
        {
            try
            {
                var usuarios = await _context.usuario
                    .FromSqlRaw("exec listarTodosUsuarios")
                    .ToListAsync();
                return Ok(usuarios);
            } catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Usuario>> GetUsuario(int id)
        {
            try
            {
                var usuario = await _context.usuario
                    .FromSqlRaw("exec buscarUsuarioPorId @id", new SqlParameter("@id", id))
                    .ToListAsync();

                if (usuario == null)
                    return NotFound();

                return Ok(usuario);

            } catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpGet("usuarios-activos")]
        public async Task<IActionResult> GetUsuariosActivos()
        {
            try
            {
                var usuarios = await _context.usuario
                    .FromSqlRaw("exec listarUsuarios")
                    .ToListAsync();
                return Ok(usuarios);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult<Usuario>> CreateUsuario([FromBody] Usuario usuario)
        {
            if (usuario == null)
                return BadRequest("Datos inválidos");

            try
            {
                var result = await _context.Database.ExecuteSqlRawAsync(
                    "exec insertarUsuario @nombre, @apellido, @nombre_usuario, @correo, @contrasena, @tipo_usuario, @registro_id, @actualizacion_id",
                    new SqlParameter("@nombre", usuario.Nombre),
                    new SqlParameter("@apellido", usuario.Apellido),
                    new SqlParameter("@nombre_usuario", usuario.NombreUsuario),
                    new SqlParameter("@correo", usuario.Correo),
                    new SqlParameter("@contrasena", usuario.Contrasena),
                    new SqlParameter("@tipo_usuario", usuario.TipoUsuario),
                    new SqlParameter("@registro_id", usuario.RegistroId),
                    new SqlParameter("@actualizacion_id", usuario.ActualizacionId)
                );
                return CreatedAtAction("GetUsuario", new { id = usuario.Id }, usuario);
            } catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUsuario(int id, Usuario usuario)
        {
            var usuarioExistente = await _context.usuario.FindAsync(id);

            if (usuarioExistente == null)
                return NotFound("El usuario no existe");

            usuarioExistente.Nombre = usuario.Nombre;
            usuarioExistente.Apellido = usuario.Apellido;
            usuarioExistente.NombreUsuario = usuario.NombreUsuario;
            usuarioExistente.Correo = usuario.Correo;
            usuarioExistente.TipoUsuario = usuario.TipoUsuario;
            usuarioExistente.HoraActualizacion = DateTime.Now;

            try
            {
                var result = await _context.Database.ExecuteSqlRawAsync(
                    "exec actualizarUsuario @id, @nombre, @apellido, @nombre_usuario, @correo, @contrasena, @tipo_usuario",
                    new SqlParameter("@id", id),
                    new SqlParameter("@nombre", usuarioExistente.Nombre),
                    new SqlParameter("@apellido", usuarioExistente.Apellido),
                    new SqlParameter("@nombre_usuario", usuarioExistente.NombreUsuario),
                    new SqlParameter("@correo", usuarioExistente.Correo),
                    new SqlParameter("@contrasena", usuarioExistente.Contrasena),
                    new SqlParameter("@tipo_usuario", usuarioExistente.TipoUsuario)
                );
                return Ok(result);
            } catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("cambiar-estado/{id}")]
        public async Task<IActionResult> UpdateEstado(int id)
        {
            var usuario = await _context.usuario.FindAsync(id);

            if (usuario == null)
                return NotFound("Usuario no encontrado");

            try
            {
                if (usuario.Estado == 1)
                    await _context.Database.ExecuteSqlRawAsync("exec desactivarUsuario @id", new SqlParameter("@id", id));
                else
                    await _context.Database.ExecuteSqlRawAsync("exec reactivarUsuario @id", new SqlParameter("@id", id));

                return Ok(new { mensaje = "Estado del usuario actualizado" });
            } catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
