using System.Data;
using fed.cloud.menu.data.Entities;
using fed.cloud.menu.data.Factories;
using RepoDb;
using Models = fed.cloud.menu.data.Models;

namespace fed.cloud.menu.test.core.Data;

public class TestDatabaseManager
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public TestDatabaseManager(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task DeleteAllRecipesAsync(IDbTransaction transaction)
    {
        await DeleteAllRecordsFromTable(transaction, "recipes");
    }

    public async Task DeleteAllIngredientsAsync(IDbTransaction transaction)
    {
        await DeleteAllRecordsFromTable(transaction, "ingredients");
    }

    public async Task DeleteAllUnitsAsync(IDbTransaction transaction)
    {
        await DeleteAllRecordsFromTable(transaction, "units");
    }

    public async Task DeleteAllUsersAsync(IDbTransaction transaction)
    {
        await DeleteAllRecordsFromTable(transaction, "users");
    }
    
    public async Task InsertRecipeAsync(IDbTransaction transaction, Recipe recipe)
    {
        // insert recipe
        await transaction.Connection.InsertAsync("recipes", recipe, transaction: transaction);

        await transaction.Connection.InsertAllAsync("recipeingredients", recipe.Ingredients, transaction: transaction);

        await transaction.Connection.ExecuteQueryAsync(
            @"REFRESH MATERIALIZED VIEW mview_recipes; 
            REFRESH MATERIALIZED VIEW mview_recipeingredients;");
    }

    public async Task InsertDefaultIngredients(IDbTransaction transaction)
    {
        var availableUnits =
            (await transaction.Connection.QueryAllAsync<Unit>("units", transaction: transaction)).Take(3);
        var ingredients = PrepareDefaultIngredients(availableUnits);
        
        await transaction.Connection.InsertAllAsync("ingredients", ingredients, transaction: transaction);
    }

    public async Task<IEnumerable<Ingredient>> GetIngredientsAsync(IDbConnection conn, int maxTake)
    {
        return (await conn.QueryAllAsync<Ingredient>("ingredients")).Take(maxTake);
    }

    public async Task InsertDefaultUnits(IDbTransaction transaction)
    {
        var units = PrepareDefaultUnits();
        
        await transaction.Connection.InsertAllAsync("units", units, transaction: transaction);
    }

    public async Task InsertUserAsync(IDbTransaction tr, User user)
    {
        if (await tr.Connection.ExistsAsync<User>(u => u.AuthenticationId == user.AuthenticationId))
        {
            return;
        }
        
        await tr.Connection.InsertAsync("users", user, transaction: tr);
    }
    
    public async Task<User?> GetUserByAuthIdAsync(IDbConnection conn, string? authId)
    {
        return (await conn.QueryAsync<User>(u => u.AuthenticationId == authId)).FirstOrDefault();
    }

    public IDbConnection CreateConnection()
    {
        return _dbConnectionFactory.CreateDefaultConnection();
    }

    public IDbTransaction BeginTransaction(IDbConnection conn)
    {
        return conn.BeginTransaction();
    }

    private async Task DeleteAllRecordsFromTable(IDbTransaction tr, string tableName)
    {
        await tr.Connection.DeleteAllAsync(tableName, transaction: tr);
    }

    private IEnumerable<Ingredient> PrepareDefaultIngredients(IEnumerable<Unit> availableUnitIds)
    {
        var availableUnitsIdArr = availableUnitIds.ToArray(); 
        return new[]
        {
            new Ingredient
            {
                Id = Guid.NewGuid(),
                Name = "ingredient 1",
                ProductNumber = 1800569,
                UnitId = availableUnitsIdArr[0].Id,
            },
            new Ingredient
            {
                Id = Guid.NewGuid(),
                Name = "ingredient 2",
                ProductNumber = 693572159,
                UnitId = availableUnitsIdArr[1].Id
            },
            new Ingredient
            {
                Id = Guid.NewGuid(),
                Name = "ingredient 48",
                ProductNumber = 4711014,
                UnitId = availableUnitsIdArr[2].Id
            }
        };
    }

    private IEnumerable<Unit> PrepareDefaultUnits()
    {
        return new[]
        {
            new Unit
            {
                Rate = 1.0,
                TypeNumber = (int)Models.UnitType.Peace
            },
            new Unit
            {
                Rate = 0.1,
                TypeNumber = (int)Models.UnitType.Gram
            },
            new Unit
            {
                Rate = 0.01,
                TypeNumber = (int)Models.UnitType.Milliliters
            }
        };
    }
}