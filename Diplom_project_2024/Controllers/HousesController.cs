using Diplom_project_2024.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Diplom_project_2024.Controllers //TODO
{
    [Route("api/[controller]")]
    [ApiController]
    public class HousesController : ControllerBase
    {
        private readonly HousesDBContext _context;

        public HousesController(HousesDBContext context)
        {
            _context = context;
        }

        //GET: api/Houses
        [HttpGet]
        public async Task<ActionResult<IEnumerable<House>>> GetHouses()
        {
            return await _context.Houses.ToListAsync();
        }

        //GET: api/Houses/5
        [HttpGet("{id}")]
        public async Task<ActionResult<House>> GetHouse(int id) 
        {
            var house = await _context.Houses.FindAsync(id);
            if(house == null)
            {
                return NotFound();
            }

            return house;
        }

        //DELETE: api/Houses/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHouse(int id)
        {
            var house = await _context.Houses.FindAsync(id);
            if (house == null)
            {
                return NotFound();
            }

            _context.Houses.Remove(house);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
