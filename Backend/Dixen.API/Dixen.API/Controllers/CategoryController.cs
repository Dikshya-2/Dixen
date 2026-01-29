using Dixen.Repo.Model.Entities;
using Dixen.Repo.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Dixen.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly IGRepo<Category> _categoryRepo;
        public CategoryController(IGRepo<Category> categoryRepo)
        {
            _categoryRepo = categoryRepo;
        }
        [HttpGet]
        public async Task<ActionResult<List<Category>>> GetAll()
        {
            var categories = await _categoryRepo.GetAll();
            return Ok(categories);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<Category>> GetById(int id)
        {
            var category = await _categoryRepo.GetById(id);
            if (category == null) return NotFound();
            return Ok(category);
        }
        [HttpPost]
        public async Task<ActionResult<Category>> Create(Category category)
        {
            await _categoryRepo.Create(category);
            return CreatedAtAction(nameof(GetById), new { id = category.Id }, category);
        }
        [HttpPut("{id}")]
        public async Task<ActionResult<Category>> Update(int id, Category category)
        {
            var updated = await _categoryRepo.Update(id, category);
            if (updated == null) return NotFound();
            return Ok(updated);
        }
        //[HttpDelete("{hallId}")]
        //public async Task<IActionResult> DeleteHall(int hallId, [FromQuery] int eventId)
        //{
        //    Console.WriteLine($"DEBUG: hallId={hallId}, eventId={eventId}");

        //    var success = await _hallService.DeleteHallAsync(eventId, hallId);
        //    Console.WriteLine($"DEBUG: service returned success={success}");

        //    if (!success) return NotFound();
        //    return NoContent();
        //}

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _categoryRepo.Delete(id);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
}
