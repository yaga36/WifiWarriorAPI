using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WifiWarriorAPI.Data;
using WifiWarriorAPI.Models;

namespace WifiWarriorAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly ApiDbContext _context;

    public UsersController(ApiDbContext context)
    {
        _context = context;
    }
    
    /// <summary>
    /// Gets all users from database.
    /// </summary>
    /// <returns>All users in a list.</returns>
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        if (!await _context.Users.AnyAsync())
            return NotFound();

        return Ok(await _context.Users.ToListAsync());
    }
    
    /// <summary>
    /// Gets user from database by Id.
    /// </summary>
    /// <returns>User by Id.</returns>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(string id)
    {
        if (!await _context.Users.AnyAsync())
            return NotFound();

        var result = await _context.Users.Where(x => x.Id == id).ToListAsync();

        if (result == null)
            return NotFound();
        
        return Ok(result);
    }
    
    /// <summary>
    /// Add a new Users object to database.
    /// </summary>
    /// <param name="users">Users Model.</param>
    /// <returns>Object with new details.</returns>
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Users users)
    {
        var result = await _context.Users.AddAsync(users);

        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { users.Id }, users);
    }

    /// <summary>
    /// Update Users .
    /// </summary>
    /// <param name="users">The Users Updated Object</param>
    /// <param name="id">The identifier.</param>
    /// <returns>No Content</returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> Put([FromBody] Users users, string id)
    {
        if (id != users.Id)
            return BadRequest();

        if (!await _context.Users.AnyAsync())
            return NotFound();

        _context.Entry(users).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch(DbUpdateConcurrencyException)
        {
            if (! await _context.Users.AnyAsync(w => w.Id == id))
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
    /// Deletes Users row.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns>No Content</returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        if (! await _context.Users.AnyAsync())
            return NotFound();

        var users = await _context.Users.FindAsync(id);

        if (users == null)
            return NotFound();

        _context.Users.Remove(users);
        await _context.SaveChangesAsync();
        
        return NoContent();
    }
    
}