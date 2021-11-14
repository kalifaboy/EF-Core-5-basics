using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityFrameworkIntro.Entities
{
    internal class DishIngredient
    {
        public int Id { get; set; }
        [MaxLength(100)]
        public string Description { get; set; } = string.Empty;
        [MaxLength(10)]
        public string UnitOfMeasure { get; set; } = string.Empty;
        [Column(TypeName = "decimal(5, 2)")]
        public decimal Amount { get; set; }

        public Dish? Dish { get; set; }
        public int? DishId { get; set; }
    }
}
