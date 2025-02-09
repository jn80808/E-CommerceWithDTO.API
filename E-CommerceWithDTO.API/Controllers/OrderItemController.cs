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
    public class OrderItemController : ControllerBase
    {
        private readonly ECommerceDbContext _context;

        public OrderItemController(ECommerceDbContext context)
        {
            _context = context;
        }

        // GET: api/orderitem
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderItemDto>>> GetOrderItems()
        {
            var orderItems = await _context.OrderItems.ToListAsync();
            var orderItemDtos = orderItems.Select(oi => new OrderItemDto
            {
                Id = oi.Id,
                OrderId = oi.OrderId,
                ProductId = oi.ProductId,
                Quantity = oi.Quantity,
                UnitPrice = oi.UnitPrice
            }).ToList();

            return Ok(orderItemDtos);
        }

        // GET: api/orderitem/5
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderItemDto>> GetOrderItem(int id)
        {
            var orderItem = await _context.OrderItems.FindAsync(id);
            if (orderItem == null)
            {
                return NotFound();
            }

            var orderItemDto = new OrderItemDto
            {
                Id = orderItem.Id,
                OrderId = orderItem.OrderId,
                ProductId = orderItem.ProductId,
                Quantity = orderItem.Quantity,
                UnitPrice = orderItem.UnitPrice
            };

            return Ok(orderItemDto);
        }
        // POST: api/orderitem
        [HttpPost]
        public async Task<ActionResult<OrderItemDto>> CreateOrderItem([FromBody] CreateOrderItemDto createOrderItemDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var order = await _context.Orders.FindAsync(createOrderItemDto.OrderId);
            if (order == null) return BadRequest("Invalid OrderId.");

            var product = await _context.Products.FindAsync(createOrderItemDto.ProductId);
            if (product == null) return BadRequest("Invalid ProductId.");

            var orderItem = new OrderItem
            {
                OrderId = createOrderItemDto.OrderId,
                ProductId = createOrderItemDto.ProductId,
                Quantity = createOrderItemDto.Quantity,
                UnitPrice = createOrderItemDto.UnitPrice
            };

            _context.OrderItems.Add(orderItem);
            await _context.SaveChangesAsync();

            var orderItemDto = new OrderItemDto
            {
                Id = orderItem.Id,
                OrderId = orderItem.OrderId,
                ProductId = orderItem.ProductId,
                Quantity = orderItem.Quantity,
                UnitPrice = orderItem.UnitPrice
            };

            return CreatedAtAction(nameof(GetOrderItem), new { id = orderItem.Id }, orderItemDto);
        }

        // PUT: api/orderitem/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrderItem(int id, [FromBody] UpdateOrderItemDto updateOrderItemDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var orderItem = await _context.OrderItems.FindAsync(id);
            if (orderItem == null) return NotFound();

            orderItem.Quantity = updateOrderItemDto.Quantity;
            orderItem.UnitPrice = updateOrderItemDto.UnitPrice;

            _context.Entry(orderItem).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }
    

    // DELETE: api/orderitem/5
    [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrderItem(int id)
        {
            var orderItem = await _context.OrderItems.FindAsync(id);
            if (orderItem == null)
            {
                return NotFound();
            }

            _context.OrderItems.Remove(orderItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}