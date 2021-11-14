using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EntityFrameworkIntro.Entities
{
    internal class Dish
    {
        public int Id { get; set; }
        [MaxLength(100)]
        public string Title { get; set; } = string.Empty;
        [MaxLength(1000)]
        public string? Notes { get; set; }
        public int? Stars { get; set; }

        public IList<DishIngredient> Igredients { get; set; }

        public Dish()
        {
            Igredients = new List<DishIngredient>();
        }
    }
}
