using System.Dynamic;
using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WifiWarriorAPI.Data;
using WifiWarriorAPI.Models;

namespace WifiWarriorAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class AddressController : ControllerBase
{
    private readonly ApiDbContext _context;

    public AddressController(ApiDbContext context)
    {
        _context = context;
    }
    
    /// <summary>
    /// Gets all addresses from database.
    /// </summary>
    /// <returns>All addresses in a list.</returns>
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Get()
    {

        if (!await _context.Addresses.AnyAsync())
            return NotFound();
        
        var addresses = await _context.Addresses
            .Include(r => r.Venue)
            .Include(r => r.ConnectionInformation)
            .Include(r => r.ConnectionInformation.ConnectionType)
            .Include(r => r.ConnectionInformation.WifiLoginDetails).ToListAsync();
        
        return Ok(addresses);
    }
    
    /// <summary>
    /// Gets address by Id from database.
    /// </summary>
    /// <returns>Addresses by Id.</returns>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        if (!await _context.Addresses.AnyAsync())
            return NotFound();

        var result = await _context.Addresses
            .Include(r => r.Venue)
            .Include(r => r.ConnectionInformation)
            .Include(r => r.ConnectionInformation.ConnectionType)
            .Include(r => r.ConnectionInformation.WifiLoginDetails)
            .Where(x => x.Id == id).ToListAsync();

        if (result == null)
            return NotFound();

        return Ok(result);

    }
    
    /// <summary>
    /// Add a new Address object to database.
    /// </summary>
    /// <param name="addresses">Address Model.</param>
    /// <returns>Object with new details.</returns>
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Address addresses)
    {
        addresses.CreatedDate = DateTime.Now;
        
        var result = await _context.Addresses.AddAsync(addresses);

        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { addresses.Id }, addresses);
    }

    /// <summary>
    /// Update Address .
    /// </summary>
    /// <param name="addresses">The Address Updated Object</param>
    /// <param name="id">The identifier.</param>
    /// <returns>No Content</returns>
    [HttpPut("{id:long}")]
    public async Task<IActionResult> Put([FromBody] Address addresses, long id)
    {
        if (id != addresses.Id)
            return BadRequest();

        if (!await _context.Addresses.AnyAsync())
            return NotFound();

        addresses.UpdatedDate = DateTime.Now;
        //TODO: User updated by.
        
        _context.Entry(addresses).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch(DbUpdateConcurrencyException)
        {
            if (! await _context.Addresses.AnyAsync(w => w.Id == id))
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
    /// Deletes Address row.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns>No Content</returns>
    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        if (! await _context.Addresses.AnyAsync())
            return NotFound();

        var addresses = await _context.Addresses.FindAsync(id);

        if (addresses == null)
            return NotFound();

        _context.Addresses.Remove(addresses);
        await _context.SaveChangesAsync();
        
        return NoContent();
    }
}