using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using E_Commerce.API.Models.Domain;
using E_Commerce.API.Models.DTO;
using ECommerceSystem;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Commerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ECommerceDbContext _context;

        public CategoryController(ECommerceDbContext context)
        {
            _context = context;
        }

        // GET: api/category
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> GetCategories()
        {
            var categories = await _context.Categories.ToListAsync();
            var categoryDtos = categories.Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description
            }).ToList();

            return Ok(categoryDtos);
        }

        // GET: api/category/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryDto>> GetCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            var categoryDto = new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description
            };

            return Ok(categoryDto);
        }

        // POST: api/category
        [HttpPost]
        public async Task<ActionResult<CategoryDto>> CreateCategory(CreateCategoryDto createCategoryDto)
        {
            var category = new Category
            {
                Name = createCategoryDto.Name,
                Description = createCategoryDto.Description
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            var categoryDto = new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description
            };

            return CreatedAtAction(nameof(GetCategory), new { id = category.Id }, categoryDto);
        }

        // PUT: api/category/5
        [HttpPut("{id}")]
        public async Task<ActionResult<object>> UpdateCategory(int id, UpdateCategoryDto updateCategoryDto)
        {
            // Validate the ID
            if (id <= 0)
            {
                return BadRequest(new { message = "Invalid category ID." });
            }

            // Validate input data
            if (updateCategoryDto == null || string.IsNullOrWhiteSpace(updateCategoryDto.Name))
            {
                return BadRequest(new { message = "Invalid category data. Name is required." });
            }

            var category = await _context.Categories.FindAsync(id);

            // If the category does not exist, return a 404 response
            if (category == null)
            {
                return NotFound(new { message = $"Category with ID {id} not found." });
            }

            // Update properties
            category.Name = updateCategoryDto.Name;
            category.Description = updateCategoryDto.Description;

            _context.Entry(category).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(new { message = $"Category with ID {id} has been successfully updated." });
        }


        // DELETE: api/category/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<object>> DeleteCategory(int id)
        {
            // Validate the ID
            if (id <= 0)
            {
                return BadRequest(new { message = "Invalid category ID." });
            }

            var category = await _context.Categories.FindAsync(id);

            // If the category does not exist, return a 404 response
            if (category == null)
            {
                return NotFound(new { message = $"Category with ID {id} not found." });
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            // Return a JSON response after successful deletion
            return Ok(new { message = $"Category with ID {id} has been successfully deleted." });
        }

    }
}