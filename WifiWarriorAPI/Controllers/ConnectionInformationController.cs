using System.Dynamic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WifiWarriorAPI.Data;
using WifiWarriorAPI.Models;

namespace WifiWarriorAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ConnectionInformationController : ControllerBase
{
    private readonly ApiDbContext _context;

    public ConnectionInformationController(ApiDbContext context)
    {
        _context = context;
    }
    
    /// <summary>
    /// Gets all connection information from database.
    /// </summary>
    /// <returns>All connection information in a list.</returns>
    [HttpGet]
    public async Task<IActionResult> Get()
    {

        if (!await _context.ConnectionInformation.AnyAsync())
            return NotFound();
        
        return Ok(await _context.ConnectionInformation
            .Include(x => x.ConnectionType)
            .Include(x => x.WifiLoginDetails).ToListAsync());
         
    }
    
    /// <summary>
    /// Gets connection information by Id from database.
    /// </summary>
    /// <returns>Connection information by Id..</returns>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        if (!await _context.ConnectionInformation.AnyAsync())
            return NotFound();
        
        var result = await _context.ConnectionInformation
            .Include(x => x.ConnectionType)
            .Include(x => x.WifiLoginDetails)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (result is null)
            return NotFound();

        return Ok(result);
    }
    
    /// <summary>
    /// Add a new Connection Information object to database.
    /// </summary>
    /// <param name="connectionInformation">Connection Information Model.</param>
    /// <returns>Object with new details.</returns>
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] ConnectionInformation connectionInformation)
    {
        connectionInformation.CreatedDate = DateTime.UtcNow;
        
        var result = await _context.ConnectionInformation.AddAsync(connectionInformation);

        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { connectionInformation.Id }, connectionInformation);
    }

    /// <summary>
    /// Update Connection Information .
    /// </summary>
    /// <param name="connectionInformation">The Connection Information Updated Object</param>
    /// <param name="id">The identifier.</param>
    /// <returns>No Content</returns>
    [HttpPut("{id:long}")]
    public async Task<IActionResult> Put([FromBody] ConnectionInformation connectionInformation, long id)
    {
        if (id != connectionInformation.Id)
            return BadRequest();

        if (!await _context.ConnectionInformation.AnyAsync())
            return NotFound();

        connectionInformation.UpdatedDate = DateTime.UtcNow;
        //TODO: User updated by.
        
        _context.Entry(connectionInformation).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch(DbUpdateConcurrencyException)
        {
            if (! await _context.ConnectionInformation.AnyAsync(w => w.Id == id))
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
    /// Deletes Connection Information row.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns>No Content</returns>
    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        if (! await _context.ConnectionInformation.AnyAsync())
            return NotFound();

        var connectionInformation = await _context.ConnectionInformation.FindAsync(id);

        if (connectionInformation == null)
            return NotFound();

        _context.ConnectionInformation.Remove(connectionInformation);
        await _context.SaveChangesAsync();
        
        return NoContent();
    }
    
}