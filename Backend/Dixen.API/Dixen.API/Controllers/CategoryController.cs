using Dixen.Repo.DTOs.Category;
using Dixen.Repo.Model.Entities;
using Dixen.Repo.Repositories;
using Dixen.Repo.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dixen.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly IGRepo<Category> _categoryRepo;
        private readonly IGRepo<Evnt> _eventRepo;

        public CategoryController(IGRepo<Category> categoryRepo, IGRepo<Evnt> eventRepo)
        {
            _categoryRepo = categoryRepo;
            _eventRepo = eventRepo;

        }
        //[Authorize(Roles = "Admin,User")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _categoryRepo.GetAll(q => q.Include(c => c.Events));
            var response = categories.Select(c => new CategoryResponse
            {
                Id = c.Id,
                Name = c.Name,
                ImageUrl = c.ImageUrl,
                Events = c.Events.Select(e => new OnlyEvent
                {
                    Id = e.Id,
                    Title = e.Title,
                    ImageUrl = e.ImageUrl,
                    StartTime = e.StartTime
                }).ToList()

            });

            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var categoryList = await _categoryRepo.Find(
                c => c.Id == id,
                q => q.Include(c => c.Events) // only include events
            );

            var category = categoryList.FirstOrDefault();
            if (category == null) return NotFound();

            var response = new CategoryResponse
            {
                Id = category.Id,
                Name = category.Name,
                Events = category.Events.Select(e => new OnlyEvent
                {
                    Id = e.Id,
                    Title = e.Title,
                    ImageUrl = e.ImageUrl,
                    StartTime = e.StartTime
                }).ToList()
            };

            return Ok(response);
        }



        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CategoryDto dto)
        {
            var category = new Category { Name = dto.Name };
            await _categoryRepo.Create(category);

            return Ok(new CategoryResponse
            {
                Id = category.Id,
                Name = category.Name
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _categoryRepo.Delete(id);
            return NoContent();
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CategoryDto dto)
        {
            var updated = new Category { Id = id, Name = dto.Name, ImageUrl=dto.ImageUrl };
            var result = await _categoryRepo.Update(id, updated);
            if (result == null) return NotFound();

            return Ok(new CategoryResponse { Id = result.Id, Name = result.Name, ImageUrl = dto.ImageUrl });
        }

        [HttpGet("sorted")]
        public async Task<IActionResult> GetSorted([FromQuery] string sortBy = "name", [FromQuery] bool descending = false)
        {
            var categories = await _categoryRepo.GetAll(q => q.Include(c => c.Events));

            // Example: sort dynamically
            var sorted = sortBy.ToLower() switch
            {
                "name" => descending ? categories.OrderByDescending(c => c.Name) : categories.OrderBy(c => c.Name),
                "events" => descending ? categories.OrderByDescending(c => c.Events.Count) : categories.OrderBy(c => c.Events.Count),
                _ => categories.OrderBy(c => c.Id)
            };

            var response = sorted.Select(c => new CategoryResponse
            {
                Id = c.Id,
                Name = c.Name,
                ImageUrl = c.ImageUrl,
                Events = c.Events.Select(e => new OnlyEvent
                {
                    Id = e.Id,
                    Title = e.Title,
                    ImageUrl = e.ImageUrl,
                    StartTime = e.StartTime
                }).ToList()
            });

            return Ok(response);
        }
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string? name)
        {
            var categories = await _categoryRepo.Find(
                c => string.IsNullOrEmpty(name) || c.Name.Contains(name),
                q => q.Include(c => c.Events)
            );

            var response = categories.Select(c => new CategoryResponse
            {
                Id = c.Id,
                Name = c.Name,
                ImageUrl = c.ImageUrl,
                Events = c.Events.Select(e => new OnlyEvent
                {
                    Id = e.Id,
                    Title = e.Title,
                    ImageUrl = e.ImageUrl,
                    StartTime = e.StartTime
                }).ToList()
            });

            return Ok(response);
        }
    }
}
