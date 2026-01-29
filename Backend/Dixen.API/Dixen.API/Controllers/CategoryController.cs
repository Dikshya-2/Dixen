using Dixen.Repo.DTOs.Category;
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
        public async Task<ActionResult<CategoryResponse>> Create([FromBody] CategoryDto categoryDto)
        {
            var category = new Category
            {
                Name = categoryDto.Name,
                ImageUrl = categoryDto.ImageUrl
            };

            await _categoryRepo.Create(category);
            return CreatedAtAction(nameof(GetById), new { id = category.Id },
                new CategoryResponse
                {
                    Id = category.Id,
                    Name = category.Name,
                    ImageUrl = category.ImageUrl,
                    EventCount = 0
                });
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<CategoryResponse>> Update(int id, [FromBody] CategoryDto categoryDto)
        {
            var updateData = new Category
            {
                Id = id,
                Name = categoryDto.Name,
                ImageUrl = categoryDto.ImageUrl
            };

            var updated = await _categoryRepo.Update(id, updateData);
            if (updated == null) return NotFound();

            return Ok(new CategoryResponse
            {
                Id = updated.Id,
                Name = updated.Name,
                ImageUrl = updated.ImageUrl,
                EventCount = 0
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _categoryRepo.Delete(id);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
}
