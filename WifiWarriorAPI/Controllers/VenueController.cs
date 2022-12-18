using System.Dynamic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WifiWarriorAPI.Data;
using WifiWarriorAPI.Models;

namespace WifiWarriorAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VenueController : ControllerBase
{
    private readonly ApiDbContext _context;

    public VenueController(ApiDbContext context)
    {
        _context = context;
    }
    
    /// <summary>
    /// Gets all venues from database.
    /// </summary>
    /// <returns>All venues in a list.</returns>
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        if (!await _context.Venues.AnyAsync())
            return NotFound();

        return Ok(await _context.Venues.ToListAsync());
    }
    
    /// <summary>
    /// Venues from database by id.
    /// </summary>
    /// <returns>Venue by id.</returns>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        if (!await _context.Venues.AnyAsync())
            return NotFound();
        
        var result = _context.Venues.Where(x => x.Id == id).ToListAsync();

        if (result == null)
            return NotFound();

        return Ok(result);
    }
    
    /// <summary>
    /// Add a new Venue object to database.
    /// </summary>
    /// <param name="venue">Venue Model.</param>
    /// <returns>Object with new details.</returns>
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Venue venue)
    {
        venue.CreatedDate = DateTime.Now;
        
        var result = await _context.Venues.AddAsync(venue);

        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { venue.Id }, venue);
    }

    /// <summary>
    /// Update Venue.
    /// </summary>
    /// <param name="venue">The Venue Updated Object</param>
    /// <param name="id">The identifier.</param>
    /// <returns>No Content</returns>
    [HttpPut("{id:long}")]
    public async Task<IActionResult> Put([FromBody] WifiLoginDetails venue, long id)
    {
        if (id != venue.Id)
            return BadRequest();

        if (!await _context.WifiLoginDetails.AnyAsync())
            return NotFound();

        venue.UpdatedDate = DateTime.Now;
        //TODO: User updated by.
        
        _context.Entry(venue).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch(DbUpdateConcurrencyException)
        {
            if (! await _context.Venues.AnyAsync(w => w.Id == id))
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
    /// Deletes Venue row.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns>No Content</returns>
    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        if (! await _context.Venues.AnyAsync())
            return NotFound();

        var venue = await _context.Venues.FindAsync(id);

        if (venue == null)
            return NotFound();

        _context.Venues.Remove(venue);
        await _context.SaveChangesAsync();
        
        return NoContent();
    }
    
}