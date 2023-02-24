using System.Data;
using fed.cloud.menu.data.Abstract;
using fed.cloud.menu.data.Entities;
using fed.cloud.menu.data.Models.Read;
using fed.cloud.recipe.application.Abstract;
using fed.cloud.recipe.application.Query;
using fed.cloud.recipe.application.Query.Queries;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace fed.cloud.menu.test.integration.Queries;

[TestFixture]
public class QueriesIntegrationTests : DatabaseTestBase
{
    private IQueryProcessor? _queryProcessor;
    private string? _defaultUserAuthId;

    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        await base.BaseSetUp(LoadServices);

        _defaultUserAuthId = "generalId-70015793";

        await ExecuteDatabaseChangingScripts(async tr =>
        {
            await DatabaseManager.InsertDefaultUnits(tr);
            await DatabaseManager.InsertDefaultIngredients(tr);
        });
    }

    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
        await ExecuteDatabaseChangingScripts(async tr =>
        {
            await DatabaseManager.DeleteAllRecipesAsync(tr);
            await DatabaseManager.DeleteAllIngredientsAsync(tr);
            await DatabaseManager.DeleteAllUnitsAsync(tr);
            await DatabaseManager.DeleteAllUsersAsync(tr);
        });
    }

    [SetUp]
    public void SetUp()
    {
        _queryProcessor = new QueryProcessor(ServiceProvider);
    }

    [Test]
    public async Task get_active_user_by_authId_query_test()
    {
        // PREPARE
        var userToGet = new User
        {
            Id = Guid.NewGuid(),
            AuthenticationId = _defaultUserAuthId!,
            IsActive = true
        };

        await ExecuteDatabaseChangingScripts(
            async tr => await DatabaseManager.InsertUserAsync(tr, userToGet));

        // TEST
        var userResult = await _queryProcessor!.Process(new GetUserByAuthIdQuery(userToGet.AuthenticationId));

        // ASSERT
        Assert.That(userResult, Is.Not.Null);
        Assert.That(userResult.AuthenticationId, Is.EqualTo(userToGet.AuthenticationId));
        Assert.That(userResult.Id, Is.EqualTo(userToGet.Id));
    }

    [Test]
    public async Task get_recipes_by_user_query_test()
    {
        // PREPARE
        User? user = null;
        await ExecuteDatabaseChangingScripts(
            async tr =>
            {
                user = await DatabaseManager.GetUserByAuthIdAsync(tr.Connection, _defaultUserAuthId);
                if (user is null)
                {
                    user = new User
                    {
                        Id = Guid.NewGuid(),
                        AuthenticationId = _defaultUserAuthId!,
                        IsActive = true
                    };

                    await DatabaseManager.InsertUserAsync(tr, user);
                }

                var nextUser = new User
                {
                    Id = Guid.NewGuid(),
                    AuthenticationId = "ggfh45fd1_123",
                    IsActive = true
                };

                await DatabaseManager.InsertUserAsync(tr, nextUser);

                var ingredients = await DatabaseManager.GetIngredientsAsync(tr.Connection, 5);

                var recipe = new Recipe
                {
                    Id = Guid.NewGuid(),
                    Content = "{{testcontent11}};{{testcontent12}}",
                    CookingTime = TimeSpan.Parse("01:10"),
                    Name = "TestRecipe1",
                    OwnerId = user.Id,
                    Tags = "tag1;tag2;tag3"
                };
                var recipeIngredients = ingredients.Select(ingredient => new RecipeIngredient
                    { Id = Guid.NewGuid(), RecipeId = recipe.Id, Quantity = 1.1, IngredientId = ingredient.Id }).ToList();

                recipe.Ingredients = recipeIngredients;
                
                await DatabaseManager.InsertRecipeAsync(tr, recipe);

                var nextRecipe = new Recipe
                {
                    Id = Guid.NewGuid(),
                    Content = "{{testcontent21}};{{testcontent22}}",
                    CookingTime = TimeSpan.Parse("00:20"),
                    Name = "TestRecipe2",
                    OwnerId = nextUser.Id,
                    Tags = "tag2;tag4"
                };

                var nextRecipeIngredients = ingredients.Select(i => new RecipeIngredient
                    { Id = Guid.NewGuid(), RecipeId = nextRecipe.Id, Quantity = 2.0, IngredientId = i.Id }).ToList();

                nextRecipe.Ingredients = nextRecipeIngredients;

                await DatabaseManager.InsertRecipeAsync(tr, nextRecipe);
            });
        
        // TEST

        var recipes = await _queryProcessor!.Process(new GetRecipesByUserQuery(user!.Id, 10, 0));
        
        Assert.Multiple(() =>
        {
            // ASSERT
            Assert.That(recipes.Any(), Is.True);
            Assert.That(recipes.All(x => x.OwnerId == user.Id), Is.True);
        });
    }

    private void LoadServices(IServiceCollection services)
    {
        services
            .AddTransient<IQueryHandler<GetRecipesByUserQuery, IEnumerable<RecipeModel>>,
                GetRecipesByUserQueryHandler>();
        services.AddTransient<IQueryHandler<GetUserByAuthIdQuery, User>, GetUserByAuthIdQueryHandler>();
    }
}