using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MySqlLogin.Data;
using MySqlLogin.Models;

namespace MySqlLogin.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class CiudadesController : ControllerBase
    {
        private readonly DataContext _context;

        public CiudadesController(DataContext context)
        {
            _context = context;
        }

        // GET: api/Ciudades
        [HttpGet]
        public IEnumerable<Ciudad> GetCIUDAD()
        {
            return _context.CIUDAD;
        }

        // GET: api/Ciudades/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCiudad([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var ciudad = await _context.CIUDAD.FindAsync(id);

            if (ciudad == null)
            {
                return NotFound();
            }

            return Ok(ciudad);
        }

        // PUT: api/Ciudades/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCiudad([FromRoute] int id, [FromBody] Ciudad ciudad)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != ciudad.CIU_ID)
            {
                return BadRequest();
            }

            _context.Entry(ciudad).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CiudadExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Ciudades
        [HttpPost]
        public async Task<IActionResult> PostCiudad([FromBody] Ciudad ciudad)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.CIUDAD.Add(ciudad);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCiudad", new { id = ciudad.CIU_ID }, ciudad);
        }

        // DELETE: api/Ciudades/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCiudad([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var ciudad = await _context.CIUDAD.FindAsync(id);
            if (ciudad == null)
            {
                return NotFound();
            }

            _context.CIUDAD.Remove(ciudad);
            await _context.SaveChangesAsync();

            return Ok(ciudad);
        }

        private bool CiudadExists(int id)
        {
            return _context.CIUDAD.Any(e => e.CIU_ID == id);
        }
    }
}