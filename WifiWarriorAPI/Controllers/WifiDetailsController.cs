using System.Reflection.Metadata.Ecma335;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WifiWarriorAPI.Data;
using WifiWarriorAPI.Models;

namespace WifiWarriorAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WifiDetailsController : ControllerBase
{
    private readonly ApiDbContext _context;
    
    public WifiDetailsController(ApiDbContext context)
    {
        _context = context;
    }
    
    /// <summary>
    /// Gets all Wi-Fi details.
    /// </summary>
    /// <returns>All Wi-Fi details in a list.</returns>
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        if (!await _context.WifiLoginDetails.AnyAsync())
            return NotFound();
                
        return Ok(_context.WifiLoginDetails.ToList());
    }
    
    /// <summary>
    /// Gets Wi-Fi details by Id.
    /// </summary>
    /// <returns>Wi-Fi details by Id.</returns>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        if (!await _context.WifiLoginDetails.AnyAsync())
            return NotFound();
        
        var result = await _context.WifiLoginDetails.Where(x => x.Id == id).ToListAsync();

        if (result == null)
            return NotFound();

        return Ok(result);
    }

    /// <summary>
    /// Add a new Wifi Details object to database.
    /// </summary>
    /// <param name="wifiDetails">Wifi Details Model.</param>
    /// <returns>Object with new details.</returns>
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] WifiLoginDetails wifiDetails)
    {
        wifiDetails.CreatedDate = DateTime.Now;
        
        var result = await _context.WifiLoginDetails.AddAsync(wifiDetails);

        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { wifiDetails.Id }, wifiDetails);
    }

    /// <summary>
    /// Update Wifi Details .
    /// </summary>
    /// <param name="wifiDetails">The Wifi Details Updated Object</param>
    /// <param name="id">The identifier.</param>
    /// <returns>No Content</returns>
    [HttpPut("{id:long}")]
    public async Task<IActionResult> Put([FromBody] WifiLoginDetails wifiDetails, long id)
    {
        if (id != wifiDetails.Id)
            return BadRequest();

        if (!await _context.WifiLoginDetails.AnyAsync())
            return NotFound();

        wifiDetails.UpdatedDate = DateTime.Now;
        //TODO: User updated by.
        
        _context.Entry(wifiDetails).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch(DbUpdateConcurrencyException)
        {
            if (! await _context.WifiLoginDetails.AnyAsync(w => w.Id == id))
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
    /// Deletes wifi details row.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns>No Content</returns>
    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        if (! await _context.WifiLoginDetails.AnyAsync())
            return NotFound();

        var wifiDetails = await _context.WifiLoginDetails.FindAsync(id);

        if (wifiDetails == null)
            return NotFound();

        _context.WifiLoginDetails.Remove(wifiDetails);
        await _context.SaveChangesAsync();
        
        return NoContent();
    }
}