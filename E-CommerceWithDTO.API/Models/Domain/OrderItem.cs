using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace E_Commerce.API.Models.Domain
{
    [Table("OrderItem")]
    public class OrderItem
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Order")]  // Reference to the navigation property (Order)
        public int OrderId { get; set; }

        public Order Order { get; set; } //navigation property

        [ForeignKey("Product")]
        public int ProductId { get; set; }
        public Product Product { get; set; }

        public int Quantity { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal UnitPrice { get; set; }
    }
}
