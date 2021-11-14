using EntityFrameworkIntro.Entities;
using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkIntro
{
    internal class CookBookContext : DbContext
    {
        public DbSet<Dish> Dishes { get; set; }
        public DbSet<DishIngredient> Ingredients { get; set; }
#pragma warning disable CS8618 // Un champ non-nullable doit contenir une valeur non-null lors de la fermeture du constructeur. Envisagez de déclarer le champ comme nullable.
        public CookBookContext(DbContextOptions<CookBookContext> options) : base(options)
#pragma warning restore CS8618 // Un champ non-nullable doit contenir une valeur non-null lors de la fermeture du constructeur. Envisagez de déclarer le champ comme nullable.
        {

        }
    }
}
