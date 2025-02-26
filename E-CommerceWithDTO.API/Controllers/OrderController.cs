﻿using Microsoft.AspNetCore.Mvc;
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
    public class OrderController : ControllerBase
    {
        private readonly ECommerceDbContext _context;

        public OrderController(ECommerceDbContext context)
        {
            _context = context;
        }

        // GET: api/order
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrders()
        {
            var orders = await _context.Orders
                .Include(o => o.OrderItems)
                .ToListAsync();

            var orderDtos = orders.Select(o => new OrderDto
            {
                Id = o.Id,
                CustomerName = o.CustomerName,
                OrderDate = o.OrderDate,
                OrderItems = o.OrderItems.Select(oi => new OrderItemDto
                {
                    Id = oi.Id,
                    OrderId = oi.OrderId,
                    ProductId = oi.ProductId,
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice
                }).ToList()
            }).ToList();

            return Ok(orderDtos);
        }

        // GET: api/order/5
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDto>> GetOrder(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            var orderDto = new OrderDto
            {
                Id = order.Id,
                CustomerName = order.CustomerName,
                OrderDate = order.OrderDate,
                OrderItems = order.OrderItems.Select(oi => new OrderItemDto
                {
                    Id = oi.Id,
                    OrderId = oi.OrderId,
                    ProductId = oi.ProductId,
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice
                }).ToList()
            };

            return Ok(orderDto);
        }


        // POST: api/order
        [HttpPost]
        public async Task<ActionResult<OrderDto>> CreateOrder([FromBody] CreateOrderDto createOrderDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var order = new Order
                {
                    CustomerName = createOrderDto.CustomerName,
                    OrderDate = DateTime.UtcNow,
                    OrderItems = createOrderDto.OrderItems.Select(oi => new OrderItem
                    {
                        ProductId = oi.ProductId,
                        Quantity = oi.Quantity,
                        UnitPrice = oi.UnitPrice
                    }).ToList()
                };

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                var orderDto = new OrderDto
                {
                    Id = order.Id,
                    CustomerName = order.CustomerName,
                    OrderDate = order.OrderDate,
                    OrderItems = order.OrderItems.Select(oi => new OrderItemDto
                    {
                        Id = oi.Id,
                        OrderId = oi.OrderId,
                        ProductId = oi.ProductId,
                        Quantity = oi.Quantity,
                        UnitPrice = oi.UnitPrice
                    }).ToList()
                };

                return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, orderDto);
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, "An error occurred while creating the order.");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(int id, [FromBody] UpdateOrderDto updateOrderDto)
        {
            // Validate ID consistency
            if (updateOrderDto.Id != 0 && updateOrderDto.Id != id)
            {
                return BadRequest("The order ID in the URL does not match the order ID in the request body.");
            }

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            order.CustomerName = updateOrderDto.CustomerName;

            _context.Entry(order).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(new { id = order.Id, customerName = order.CustomerName });
        }



        // DELETE: api/order/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Order deleted successfully." });
        }
    }
}