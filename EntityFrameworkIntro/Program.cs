using EntityFrameworkIntro;
using EntityFrameworkIntro.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

var factory = new CookBookContextFactory();
var context = factory.CreateDbContext(args);
//Add
Console.WriteLine("Add Porridge for breakfast");
var porridge = new Dish { Title = "Porridge", Notes = "Very good", Stars = 4 };
context.Dishes.Add(porridge);
await context.SaveChangesAsync();
Console.WriteLine($"Added Porridge successfully with Id = {porridge.Id}");

//Select (Read)
Console.WriteLine("Checking stars with porrigde");
var dishes = await context.Dishes
    .Where(d => d.Title.Contains("Porridge")).ToArrayAsync();
if (dishes.Length != 1) Console.Error.WriteLine("Something really bad happened");
Console.WriteLine($"Porridge has {dishes.First().Stars} stars");


//Update
Console.WriteLine("Change Porridge starts rating");
porridge.Stars = 5;
await context.SaveChangesAsync();
Console.WriteLine($"Porridge Updated");

//Remove
Console.WriteLine("Remove Porridge");
context.Dishes.Remove(porridge);
await context.SaveChangesAsync();
Console.WriteLine($"Porridge Removed");

//Experiments
var newDish = new Dish { Title = "Foo", Notes = "Bar" };
context.Dishes.Add(newDish);
await context.SaveChangesAsync();
newDish.Notes = "Baz";
await context.SaveChangesAsync();

//await EntityStates(factory);

static async Task EntityStates(CookBookContextFactory factory)
{
    using var dbcontext = factory.CreateDbContext();
    var newDish = new Dish { Title = "Foo", Notes = "Bar" };
    var state = dbcontext.Entry(newDish).State;// << Detached
    dbcontext.Dishes.Add(newDish);
    state = dbcontext.Entry(newDish).State; // << Added

    await dbcontext.SaveChangesAsync();
    state = dbcontext.Entry(newDish).State; // << Unchanged

    newDish.Notes = "Baz";
    state = dbcontext.Entry(newDish).State; // << Modified

    await dbcontext.SaveChangesAsync();
    state = dbcontext.Entry(newDish).State;// << Unchanged

    dbcontext.Dishes.Remove(newDish);
    state = dbcontext.Entry(newDish).State; // << Deleted

    await dbcontext.SaveChangesAsync();// << Unchanged
    state = dbcontext.Entry(newDish).State; // << Detached
}

//await ChangeTracking(factory);

static async Task ChangeTracking(CookBookContextFactory factory)
{
    using var dbcontext = factory.CreateDbContext();
    var newDish = new Dish { Title = "Foo", Notes = "Bar" };
    dbcontext.Dishes.Add(newDish);
    await dbcontext.SaveChangesAsync();
    newDish.Notes = "Baz";
    var entry = dbcontext.Entry(newDish);
    var originalNote = entry.OriginalValues[nameof(Dish.Notes)].ToString();
}

//await AttachEntities(factory);

static async Task AttachEntities(CookBookContextFactory factory)
{
    using var dbcontext = factory.CreateDbContext();
    var newDish = new Dish { Title = "Foo", Notes = "Bar" };
    dbcontext.Dishes.Add(newDish);

    dbcontext.Entry<Dish>(newDish).State = EntityState.Detached;
}

await RawSql(factory);

static async Task RawSql(CookBookContextFactory factory)
{
    using var dbcontext = factory.CreateDbContext();

    var dishes = await dbcontext.Dishes
        .FromSqlRaw("select * from Dishes")
        .ToListAsync();

    // To avoid SQL Injection
    var filter = "%z";
    var filteredDishes = await dbcontext.Dishes
        .FromSqlInterpolated($"select * from Dishes where Notes like {filter}")
        .ToListAsync();

    await dbcontext.Database.ExecuteSqlRawAsync("delete from Dishes where Id not in (select DishId from Ingredients)");
}

await Transaction(factory);

static async Task Transaction(CookBookContextFactory factory)
{
    using var dbcontext = factory.CreateDbContext();
    using var transaction = await dbcontext.Database.BeginTransactionAsync();
    try
    {
        var newDish = new Dish { Title = "Foo", Notes = "Bar" };
        dbcontext.Dishes.Add(newDish);

        await dbcontext.Database.ExecuteSqlRawAsync("select 1/0 as BAD");
        await transaction.CommitAsync();
    }
    catch (SqlException ex)
    {

        Console.Error.WriteLine($"Something bad happened {ex.Message}");
    }

    await ExpressionTree(factory);

    static async Task ExpressionTree(CookBookContextFactory factory)
    {
        using var dbcontext = factory.CreateDbContext();
        var newDish = new Dish { Title = "Foo", Notes = "Bar" };
        dbcontext.Dishes.Add(newDish);
        await dbcontext.SaveChangesAsync();

        var dishes = await dbcontext.Dishes
            .Where(d => d.Title.StartsWith("F"))
            .ToListAsync();

        Func<Dish, bool> lambda = d => d.Title.StartsWith("F");
        Expression<Func<Dish, bool>> expression = d => d.Title.StartsWith("F");
    }
}