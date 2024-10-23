using AplicacionNPG.Server.Data;
using AplicacionNPG.Server.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;

namespace AplicacionNPG.Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("/api/[controller]")]
    public class IncidenciaController : ControllerBase
    {
        private readonly AppDbContext _context;

        public IncidenciaController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Incidencia>>> GetIncidencias()
        {
            try
            {
                var incidencias = await _context.incidencia
                    .FromSqlRaw("exec listarIncidencias")
                    .ToListAsync();
                return Ok(incidencias);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Incidencia>> GetIncidencia(int id)
        {
            try
            {
                var incidencia = await _context.incidencia
                    .FromSqlRaw("exec buscarIncidenciaPorId @id", new SqlParameter("@id", id))
                    .ToListAsync();

                if (incidencia == null)
                    return NotFound();

                return Ok(incidencia);
            } catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult<Incidencia>> CreateIncidencia([FromBody] Incidencia incidencia)
        {
            if (incidencia == null)
                return BadRequest("Datos inválidos");

            try
            {
                var result = await _context.Database.ExecuteSqlRawAsync(
                    "exec insertarIncidencia @nombre, @empresa, @hora_entrada, @comentario, @usuario_id_registro",
                    new SqlParameter("@nombre", incidencia.Nombre),
                    new SqlParameter("@empresa", incidencia.Empresa),
                    new SqlParameter("@hora_entrada", incidencia.HoraEntrada),
                    new SqlParameter("@comentario", incidencia.Comentario),
                    new SqlParameter("@usuario_id_registro", incidencia.UsuarioId)
                );
                return CreatedAtAction("GetIncidencia", new { id = incidencia.Id }, incidencia);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateIncidencia(int id, Incidencia incidencia)
        {
            var incidenciaExistente = await _context.incidencia.FindAsync(id);

            if (incidenciaExistente == null)
                return NotFound("La incidencia no existe");

            incidenciaExistente.Nombre = incidencia.Nombre;
            incidenciaExistente.Empresa = incidencia.Empresa;
            incidenciaExistente.HoraEntrada = incidencia.HoraEntrada;
            incidenciaExistente.HoraSalida = incidencia.HoraSalida;
            incidenciaExistente.Comentario = incidencia.Comentario;
            incidenciaExistente.ActualizacionId = incidencia.ActualizacionId;

            try
            {
                var result = await _context.Database.ExecuteSqlRawAsync(
                    "exec actualizarIncidencia @id, @nombre, @empresa, @hora_entrada, @hora_salida, @comentario, @usuario_id_actualizacion",
                    new SqlParameter("@id", id),
                    new SqlParameter("@nombre", incidenciaExistente.Nombre),
                    new SqlParameter("@empresa", incidenciaExistente.Empresa),
                    new SqlParameter("@hora_entrada", incidenciaExistente.HoraEntrada),
                    new SqlParameter("@hora_salida", incidenciaExistente.HoraSalida),
                    new SqlParameter("@comentario", incidenciaExistente.Comentario),
                    new SqlParameter("@usuario_id_actualizacion", incidenciaExistente.ActualizacionId)
                );
                return Ok(result);
            } catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteIncidencia(int id)
        {
            var incidencia = await _context.incidencia.FindAsync(id);

            if (incidencia == null)
                return NotFound();

            await _context.Database.ExecuteSqlRawAsync("exec eliminarIncidencia @id", new SqlParameter("@id", id));
            return NoContent();
        }
    }
}
