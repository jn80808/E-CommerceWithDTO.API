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
    public class ProductCategoryController : ControllerBase
    {
        private readonly ECommerceDbContext _context;

        public ProductCategoryController(ECommerceDbContext context)
        {
            _context = context;
        }

        // GET: api/productcategory
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductCategoryDto>>> GetProductCategories()
        {
            var productCategories = await _context.ProductCategories.ToListAsync();
            var productCategoryDtos = productCategories.Select(pc => new ProductCategoryDto
            {
                ProductId = pc.ProductId,
                CategoryId = pc.CategoryId
            }).ToList();

            return Ok(productCategoryDtos);
        }

        // GET: api/productcategory/5/10
        [HttpGet("{productId}/{categoryId}")]
        public async Task<ActionResult<ProductCategoryDto>> GetProductCategory(int productId, int categoryId)
        {
            var productCategory = await _context.ProductCategories
                .FirstOrDefaultAsync(pc => pc.ProductId == productId && pc.CategoryId == categoryId);

            if (productCategory == null)
            {
                return NotFound();
            }

            var productCategoryDto = new ProductCategoryDto
            {
                ProductId = productCategory.ProductId,
                CategoryId = productCategory.CategoryId
            };

            return Ok(productCategoryDto);
        }

        // POST: api/productcategory
        [HttpPost]
        public async Task<ActionResult<ProductCategoryDto>> CreateProductCategory(CreateProductCategoryDto createProductCategoryDto)
        {
            var productCategory = new ProductCategory
            {
                ProductId = createProductCategoryDto.ProductId,
                CategoryId = createProductCategoryDto.CategoryId
            };

            _context.ProductCategories.Add(productCategory);
            await _context.SaveChangesAsync();

            var productCategoryDto = new ProductCategoryDto
            {
                ProductId = productCategory.ProductId,
                CategoryId = productCategory.CategoryId
            };

            return CreatedAtAction(nameof(GetProductCategory), new { productId = productCategory.ProductId, categoryId = productCategory.CategoryId }, productCategoryDto);
        }

        // DELETE: api/productcategory/5/10
        [HttpDelete("{productId}/{categoryId}")]
        public async Task<IActionResult> DeleteProductCategory(int productId, int categoryId)
        {
            var productCategory = await _context.ProductCategories
                .FirstOrDefaultAsync(pc => pc.ProductId == productId && pc.CategoryId == categoryId);

            if (productCategory == null)
            {
                return NotFound();
            }

            _context.ProductCategories.Remove(productCategory);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}