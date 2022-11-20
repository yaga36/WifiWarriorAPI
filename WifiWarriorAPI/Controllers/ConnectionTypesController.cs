using System.Dynamic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WifiWarriorAPI.Data;
using WifiWarriorAPI.Models;

namespace WifiWarriorAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class ConnectionTypesController : ControllerBase
{
    private readonly ApiDbContext _context;

    public ConnectionTypesController(ApiDbContext context)
    {
        _context = context;
    }
    
    /// <summary>
    /// Gets all connection types from database.
    /// </summary>
    /// <returns>All connection types in a list.</returns>
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        if (!await _context.ConnectionTypes.AnyAsync())
            return NotFound();
        
        return Ok(await _context.ConnectionTypes.ToListAsync());
    }
    
    /// <summary>
    /// Connection types from database by Id.
    /// </summary>
    /// <returns>Connection types by Id.</returns>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        if (!await _context.ConnectionTypes.AnyAsync())
            return NotFound();

        var result = await _context.ConnectionTypes
            .Where(x => x.Id == id).ToListAsync();

        if (result == null)
            return NotFound();
        
        return Ok(result);
    }
    
    /// <summary>
    /// Add a new Connection Type object to database.
    /// </summary>
    /// <param name="connectionType">Connection Type Model.</param>
    /// <returns>Object with new details.</returns>
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] ConnectionType connectionType)
    {
        connectionType.CreatedDate = DateTime.Now;
        
        var result = await _context.ConnectionTypes.AddAsync(connectionType);

        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { connectionType.Id }, connectionType);
    }

    /// <summary>
    /// Update Connection Type .
    /// </summary>
    /// <param name="connectionType">The Connection Type Updated Object</param>
    /// <param name="id">The identifier.</param>
    /// <returns>No Content</returns>
    [HttpPut("{id:long}")]
    public async Task<IActionResult> Put([FromBody] ConnectionType connectionType, long id)
    {
        if (id != connectionType.Id)
            return BadRequest();

        if (!await _context.ConnectionTypes.AnyAsync())
            return NotFound();

        connectionType.UpdatedDate = DateTime.Now;
        //TODO: User updated by.
        
        _context.Entry(connectionType).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch(DbUpdateConcurrencyException)
        {
            if (! await _context.ConnectionTypes.AnyAsync(w => w.Id == id))
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

    /// <summary>
    /// Deletes Connection Type row.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns>No Content</returns>
    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        if (! await _context.ConnectionTypes.AnyAsync())
            return NotFound();

        var connectionType = await _context.ConnectionTypes.FindAsync(id);

        if (connectionType == null)
            return NotFound();

        _context.ConnectionTypes.Remove(connectionType);
        await _context.SaveChangesAsync();
        
        return NoContent();
    }
    
}